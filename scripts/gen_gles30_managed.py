import argparse
import re
import sys
import xml.etree.ElementTree as ET
from pathlib import Path


TYPE_MAP = {
    "void": "void",
    "GLvoid": "void",
    "GLenum": "uint",
    "GLbitfield": "uint",
    "GLuint": "uint",
    "GLint": "int",
    "GLsizei": "int",
    "GLfloat": "float",
    "GLclampf": "float",
    "GLdouble": "double",
    "GLboolean": "byte",
    "GLbyte": "sbyte",
    "GLubyte": "byte",
    "GLshort": "short",
    "GLushort": "ushort",
    "GLint64": "long",
    "GLuint64": "ulong",
    "GLintptr": "nint",
    "GLsizeiptr": "nint",
    "GLsync": "IntPtr",
}

CS_KEYWORDS = {
    "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char",
    "checked", "class", "const", "continue", "decimal", "default", "delegate",
    "do", "double", "else", "enum", "event", "explicit", "extern", "false",
    "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit",
    "in", "int", "interface", "internal", "is", "lock", "long", "namespace",
    "new", "null", "object", "operator", "out", "override", "params", "private",
    "protected", "public", "readonly", "ref", "return", "sbyte", "sealed",
    "short", "sizeof", "stackalloc", "static", "string", "struct", "switch",
    "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked",
    "unsafe", "ushort", "using", "virtual", "void", "volatile", "while",
}


def safe_name(name: str) -> str:
    return f"@{name}" if name in CS_KEYWORDS else name


def split_csharp_parameters(text: str):
    parts = []
    start = 0
    bracket_depth = 0
    for index, char in enumerate(text):
        if char == "[":
            bracket_depth += 1
        elif char == "]":
            bracket_depth -= 1
        elif char == "," and bracket_depth == 0:
            parts.append(text[start:index].strip())
            start = index + 1
    tail = text[start:].strip()
    if tail:
        parts.append(tail)
    return parts


def parse_delegate_parameters(delegate_text: str):
    match = re.search(r"\((.*)\)\s*;", delegate_text, re.DOTALL)
    if not match:
        raise ValueError(f"Delegate has no parameter list: {delegate_text}")

    parameters = []
    for raw_parameter in split_csharp_parameters(match.group(1)):
        parameter = re.sub(r"\[[^\]]+\]\s*", "", raw_parameter).strip()
        parameter_match = re.match(r"(?:(ref|out|in)\s+)?(.+?)\s+(@?[A-Za-z_][A-Za-z0-9_]*)$", parameter)
        if not parameter_match:
            raise ValueError(f"Cannot parse delegate parameter: {raw_parameter}")
        modifier, type_name, name = parameter_match.groups()
        parameters.append((type_name, name, f"{modifier} " if modifier else ""))
    return parameters


def format_wrapper_parameters(parameters):
    return ", ".join(f"{modifier}{type_name} {name}" for type_name, name, modifier in parameters)


def format_wrapper_call_arguments(parameters):
    return ", ".join(f"{modifier}{name}" for _, name, modifier in parameters)


def is_path_under(path: Path, root: Path) -> bool:
    try:
        path.resolve().relative_to(root.resolve())
        return True
    except ValueError:
        return False


def write_text_if_changed(path: Path, content: str):
    if path.exists() and path.read_text(encoding="utf-8") == content:
        return False
    path.write_text(content, encoding="utf-8")
    return True


def parse_existing_gles(gles_dir: Path, excluded_dirs=()):
    excluded_dirs = tuple(Path(directory).resolve() for directory in excluded_dirs)
    consts = set()
    methods = set()
    fields = set()
    delegates = set()
    delegate_signatures = {}

    for path in gles_dir.rglob("*.cs"):
        if any(is_path_under(path, excluded_dir) for excluded_dir in excluded_dirs):
            continue
        text = path.read_text(encoding="utf-8")
        consts.update(re.findall(r"\bconst\s+\w+\s+(GL_[A-Za-z0-9_]+)\b", text))
        for match in re.finditer(r"\b(?:public|internal)\s+static\s+[^\s]+\s+([A-Za-z0-9_]+)\s*\(", text):
            methods.add(match.group(1))
        for match in re.finditer(r"\binternal\s+static\s+gl\w+_t\?\s+(_gl\w+)\b", text):
            fields.add(match.group(1))
        for match in re.finditer(
            r"(internal\s+delegate\s+[^;]+?\s+(gl\w+_t)\s*\([^;]*\);)",
            text,
            re.DOTALL,
        ):
            delegate_text, delegate_name = match.groups()
            delegates.add(delegate_name)
            delegate_signatures[delegate_name] = parse_delegate_parameters(delegate_text)

    return consts, methods, fields, delegates, delegate_signatures


def parse_existing_loader(loader_path: Path):
    text = loader_path.read_text(encoding="utf-8")
    return set(re.findall(r"Gles\.(\_gl\w+)\s*=", text))


def extract_proto_info(proto_elem):
    raw = "".join(proto_elem.itertext())
    ptype = proto_elem.findtext("ptype")
    base_type = ptype if ptype is not None else "void"
    pointer_level = raw.count("*")
    is_const = "const" in raw.split()
    name = proto_elem.findtext("name")
    return name, base_type, pointer_level, is_const, raw


def extract_param_info(param_elem):
    raw = "".join(param_elem.itertext())
    ptype = param_elem.findtext("ptype")
    base_type = ptype if ptype is not None else "void"
    pointer_level = raw.count("*")
    is_const = "const" in raw.split()
    name = param_elem.findtext("name")
    return name, base_type, pointer_level, is_const, raw


def map_return_type(base_type: str, pointer_level: int):
    if pointer_level > 0:
        return "IntPtr"
    if base_type in TYPE_MAP:
        return TYPE_MAP[base_type]
    raise ValueError(f"Unknown return type: {base_type}")


def map_param_type(base_type: str, pointer_level: int, is_const: bool):
    if pointer_level > 0:
        if pointer_level == 1 and base_type == "GLchar" and is_const:
            return "string", True
        return "IntPtr", False
    if base_type in TYPE_MAP:
        return TYPE_MAP[base_type], False
    raise ValueError(f"Unknown param type: {base_type}")


def sanitize_enum_value(value: str) -> str:
    return re.sub(r"(?i)[uUlL]+$", "", value)


def enum_csharp_type(value: str) -> str:
    clean = sanitize_enum_value(value)
    try:
        numeric = int(clean, 0)
    except ValueError as exc:
        raise ValueError(f"Unsupported enum value: {value}") from exc
    if numeric < 0:
        return "int"
    if numeric > 0xFFFFFFFF:
        return "ulong"
    return "uint"


def main():
    parser = argparse.ArgumentParser(description="Generate GLES 3.0 managed bindings from gl.xml.")
    parser.add_argument("--gl-xml", default="external/angle/third_party/dawn/third_party/khronos/OpenGL-Registry/xml/gl.xml")
    parser.add_argument("--gles-dir", default="cs/AngleBridge.Managed/GLES")
    parser.add_argument("--gles-loader", default="cs/AngleBridge.Managed/GLES/GlesLoader.cs")
    parser.add_argument("--out-dir", default="cs/AngleBridge.Managed/GLES/Generated")
    args = parser.parse_args()

    gl_xml_path = Path(args.gl_xml)
    gles_dir = Path(args.gles_dir)
    gles_loader_path = Path(args.gles_loader)
    out_dir = Path(args.out_dir)

    if not gl_xml_path.exists():
        print(f"gl.xml not found: {gl_xml_path}", file=sys.stderr)
        return 1
    if not gles_dir.exists():
        print(f"GLES dir not found: {gles_dir}", file=sys.stderr)
        return 1

    existing_consts, existing_methods, existing_fields, existing_delegates, existing_delegate_signatures = parse_existing_gles(
        gles_dir,
        excluded_dirs=(out_dir, gles_dir / "Generated"),
    )
    existing_loader_fields = parse_existing_loader(gles_loader_path) if gles_loader_path.exists() else set()

    root = ET.parse(gl_xml_path).getroot()

    all_enums = {}
    for enum in root.findall(".//enums/enum"):
        name = enum.get("name")
        value = enum.get("value")
        if name and value:
            all_enums[name] = value

    command_defs = {}
    for command in root.find("commands").findall("command"):
        proto = command.find("proto")
        if proto is None:
            continue
        cmd_name, ret_base, ret_ptr, ret_const, _ = extract_proto_info(proto)
        if not cmd_name:
            continue
        params = []
        for param in command.findall("param"):
            pname, pbase, pptr, pconst, praw = extract_param_info(param)
            params.append((pname, pbase, pptr, pconst, praw))
        command_defs[cmd_name] = (ret_base, ret_ptr, ret_const, params)

    required_commands = {}
    required_enums = set()
    for feature in root.findall("./feature[@api='gles2']"):
        number = feature.get("number")
        if number is None:
            continue
        try:
            version = float(number)
        except ValueError:
            continue
        if version > 3.0:
            continue
        for require in feature.findall("require"):
            for cmd in require.findall("command"):
                name = cmd.get("name")
                if not name:
                    continue
                prev = required_commands.get(name)
                required_commands[name] = version if prev is None else min(prev, version)
            for enum in require.findall("enum"):
                name = enum.get("name")
                if name:
                    required_enums.add(name)

    missing_defs = [name for name in required_commands if name not in command_defs]
    if missing_defs:
        print("Missing command definitions:", ", ".join(sorted(missing_defs)), file=sys.stderr)
        return 1

    out_dir.mkdir(parents=True, exist_ok=True)

    enums_lines = []
    for name in sorted(required_enums):
        if name in existing_consts:
            continue
        value = all_enums.get(name)
        if value is None:
            print(f"Enum value not found: {name}", file=sys.stderr)
            return 1
        cs_type = enum_csharp_type(value)
        enums_lines.append(f"        public const {cs_type} {name} = {sanitize_enum_value(value)};")

    enums_path = out_dir / "Gles.Enums.GLES30.cs"
    enums_content = "\n".join([
        "using System;",
        "",
        "namespace AngleBridge.Managed.GLES",
        "{",
        "    internal static partial class Gles",
        "    {",
        *enums_lines,
        "    }",
        "}",
        "",
    ])
    write_text_if_changed(enums_path, enums_content)

    fields_lines = []
    methods_lines = []
    delegates_lines = []
    loader_lines = []

    for cmd_name in sorted(required_commands):
        ret_base, ret_ptr, _, params = command_defs[cmd_name]
        min_version = required_commands[cmd_name]
        method_name = cmd_name[2:] if cmd_name.startswith("gl") else cmd_name
        field_name = f"_gl{method_name}"
        delegate_name = f"{cmd_name}_t"

        if field_name not in existing_fields:
            fields_lines.append(f"        internal static {delegate_name}? {field_name};")

        if delegate_name not in existing_delegates:
            param_decl = []
            for pname, pbase, pptr, pconst, _ in params:
                cs_type, is_utf8 = map_param_type(pbase, pptr, pconst)
                safe = safe_name(pname)
                if is_utf8:
                    param_decl.append(f"[MarshalAs(UnmanagedType.LPUTF8Str)] {cs_type} {safe}")
                else:
                    param_decl.append(f"{cs_type} {safe}")
            ret_cs = map_return_type(ret_base, ret_ptr)
            delegates_lines.append("        [UnmanagedFunctionPointer(CallingConvention.Winapi)]")
            delegates_lines.append(f"        internal delegate {ret_cs} {delegate_name}({', '.join(param_decl)});")
            delegates_lines.append("")

        if method_name not in existing_methods:
            if delegate_name in existing_delegate_signatures:
                wrapper_parameters = existing_delegate_signatures[delegate_name]
            else:
                wrapper_parameters = []
                for pname, pbase, pptr, pconst, _ in params:
                    cs_type, _ = map_param_type(pbase, pptr, pconst)
                    wrapper_parameters.append((cs_type, safe_name(pname), ""))

            param_decl = format_wrapper_parameters(wrapper_parameters)
            param_names = format_wrapper_call_arguments(wrapper_parameters)
            ret_cs = map_return_type(ret_base, ret_ptr)
            loader_call = "NotLoaded" if min_version <= 2.0 else "NotSupported"
            reason = "" if min_version <= 2.0 else ", \"Requires GLES3.0.\""
            if ret_cs == "void":
                methods_lines.append(
                    f"        public static void {method_name}({param_decl}) =>\n"
                    f"            ({field_name} ?? throw {loader_call}(nameof({field_name}){reason}))({param_names});"
                )
            else:
                methods_lines.append(
                    f"        public static {ret_cs} {method_name}({param_decl}) =>\n"
                    f"            ({field_name} ?? throw {loader_call}(nameof({field_name}){reason}))({param_names});"
                )
            methods_lines.append("")

        if field_name not in existing_loader_fields:
            load_call = "LoadFunction" if min_version <= 2.0 else "TryLoadFunction"
            loader_lines.append(
                f"            Gles.{field_name} = {load_call}<Gles.{delegate_name}>(\"{cmd_name}\");"
            )

    commands_path = out_dir / "Gles.Commands.GLES30.cs"
    commands_content = "\n".join([
        "using System;",
        "using System.Runtime.InteropServices;",
        "",
        "namespace AngleBridge.Managed.GLES",
        "{",
        "    internal static partial class Gles",
        "    {",
        *fields_lines,
        "",
        *methods_lines,
        *delegates_lines,
        "    }",
        "}",
        "",
    ])
    write_text_if_changed(commands_path, commands_content)

    loader_path = out_dir / "GlesLoader.GLES30.cs"
    loader_content = "\n".join([
        "namespace AngleBridge.Managed.GLES",
        "{",
        "    internal static partial class GlesLoader",
        "    {",
        "        private static void LoadGenerated()",
        "        {",
        *loader_lines,
        "        }",
        "    }",
        "}",
        "",
    ])
    write_text_if_changed(loader_path, loader_content)

    print(f"Generated {enums_path}")
    print(f"Generated {commands_path}")
    print(f"Generated {loader_path}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
