import hashlib
import importlib.util
import subprocess
import sys
import tempfile
import unittest
from pathlib import Path

SCRIPT = Path(__file__).with_name("gen_gles30_managed.py").resolve()
REPO = SCRIPT.parent.parent
GLES_DIR = REPO / "cs" / "AngleBridge.Managed" / "GLES"
GL_XML = REPO / "external" / "angle" / "third_party" / "dawn" / "third_party" / "khronos" / "OpenGL-Registry" / "xml" / "gl.xml"
LOADER = GLES_DIR / "GlesLoader.cs"

spec = importlib.util.spec_from_file_location("gen_gles30_managed", SCRIPT)
if spec is None or spec.loader is None:
    raise RuntimeError(f"Unable to load generator module from {SCRIPT}")
module = importlib.util.module_from_spec(spec)
spec.loader.exec_module(module)


class DelegateParameterTests(unittest.TestCase):
    def test_parse_ref_out_delegate_parameters_for_generated_wrapper(self):
        text = "internal delegate void glGetTransformFeedbackVarying_t(uint program, uint index, int bufSize, out int length, out int size, out uint type, IntPtr name);"
        parsed = module.parse_delegate_parameters(text)
        self.assertEqual(
            parsed,
            [
                ("uint", "program", ""),
                ("uint", "index", ""),
                ("int", "bufSize", ""),
                ("int", "length", "out "),
                ("int", "size", "out "),
                ("uint", "type", "out "),
                ("IntPtr", "name", ""),
            ],
        )

    def test_wrapper_call_uses_ref_and_out_modifiers(self):
        params = [("int", "n", ""), ("uint", "buffers", "ref ")]
        self.assertEqual(module.format_wrapper_parameters(params), "int n, ref uint buffers")
        self.assertEqual(module.format_wrapper_call_arguments(params), "n, ref buffers")


class GeneratorIntegrationTests(unittest.TestCase):
    def test_custom_output_directory_is_complete_and_idempotent(self):
        with tempfile.TemporaryDirectory(prefix="generator_test_", dir=GLES_DIR) as temp_dir:
            output_dir = Path(temp_dir) / "Generated2"
            command = [
                sys.executable,
                str(SCRIPT),
                "--gl-xml",
                str(GL_XML),
                "--gles-dir",
                str(GLES_DIR),
                "--gles-loader",
                str(LOADER),
                "--out-dir",
                str(output_dir),
            ]
            subprocess.run(command, cwd=REPO, check=True, capture_output=True, text=True)
            first_hashes = self._hash_outputs(output_dir)

            commands = (output_dir / "Gles.Commands.GLES30.cs").read_text(encoding="utf-8")
            loader = (output_dir / "GlesLoader.GLES30.cs").read_text(encoding="utf-8")
            existing_loader = LOADER.read_text(encoding="utf-8")
            self.assertIn("out int length, out int size, out uint type", commands)
            self.assertIn("out length, out size, out type", commands)
            self.assertIn("Gles._glGetTransformFeedbackVarying =", existing_loader + loader)

            subprocess.run(command, cwd=REPO, check=True, capture_output=True, text=True)
            self.assertEqual(first_hashes, self._hash_outputs(output_dir))

    @staticmethod
    def _hash_outputs(output_dir):
        return {
            path.name: hashlib.sha256(path.read_bytes()).hexdigest()
            for path in sorted(output_dir.glob("*.cs"))
        }


if __name__ == "__main__":
    unittest.main()
