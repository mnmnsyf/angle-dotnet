using AngleBridge.Sdk.HotReload;
using AngleBridge.Sdk.Interop;
using AngleBridge.Sdk.Runtime;
using System;
using System.IO;

namespace Demo16_Multiview.Plugin
{
    public sealed class MultiviewRenderer : IRenderer, IRendererWithHostContext
    {
        private const int ViewCount = 2;

        private AngleGl? _gl;
        private AngleBuffer? _cubeBuffer;
        private AngleBuffer? _cubeIndexBuffer;
        private AngleBuffer? _quadBuffer;
        private AngleVertexArray? _cubeVao;
        private AngleVertexArray? _quadVao;
        private AngleTexture? _colorArray;
        private AngleTexture? _depthArray;
        private AngleFramebuffer? _fbo;
        private ShaderProgramReloader? _multiviewProgramReloader;
        private ShaderProgramReloader? _blitProgramReloader;
        private Exception? _lastMvError;
        private Exception? _lastBlitError;
        private int _mvPerspectiveLoc;
        private int _mvLeftEyeLoc;
        private int _mvRightEyeLoc;
        private int _mvTranslationLoc;
        private int _combineMultiviewTexLoc;
        private int _combinePositionLoc; // vIn
        private bool _useVao;
        private bool _initialized;
        private int _viewWidth;
        private int _viewHeight;

        private static readonly float[] s_cubeVertices =
        {
            // position           normal
            // Front
            -1.0f, -1.0f,  1.0f,   0f,  0f,  1f,
             1.0f, -1.0f,  1.0f,   0f,  0f,  1f,
             1.0f,  1.0f,  1.0f,   0f,  0f,  1f,
            -1.0f,  1.0f,  1.0f,   0f,  0f,  1f,
            // Back
            -1.0f, -1.0f, -1.0f,   0f,  0f, -1f,
             1.0f, -1.0f, -1.0f,   0f,  0f, -1f,
             1.0f,  1.0f, -1.0f,   0f,  0f, -1f,
            -1.0f,  1.0f, -1.0f,   0f,  0f, -1f,
            // Left
            -1.0f, -1.0f, -1.0f,  -1f,  0f,  0f,
            -1.0f, -1.0f,  1.0f,  -1f,  0f,  0f,
            -1.0f,  1.0f,  1.0f,  -1f,  0f,  0f,
            -1.0f,  1.0f, -1.0f,  -1f,  0f,  0f,
            // Right
             1.0f, -1.0f, -1.0f,   1f,  0f,  0f,
             1.0f, -1.0f,  1.0f,   1f,  0f,  0f,
             1.0f,  1.0f,  1.0f,   1f,  0f,  0f,
             1.0f,  1.0f, -1.0f,   1f,  0f,  0f,
            // Top
            -1.0f,  1.0f, -1.0f,   0f,  1f,  0f,
            -1.0f,  1.0f,  1.0f,   0f,  1f,  0f,
             1.0f,  1.0f,  1.0f,   0f,  1f,  0f,
             1.0f,  1.0f, -1.0f,   0f,  1f,  0f,
            // Bottom
            -1.0f, -1.0f, -1.0f,   0f, -1f,  0f,
            -1.0f, -1.0f,  1.0f,   0f, -1f,  0f,
             1.0f, -1.0f,  1.0f,   0f, -1f,  0f,
             1.0f, -1.0f, -1.0f,   0f, -1f,  0f,
        };

        private static readonly ushort[] s_cubeIndices =
        {
            0, 1, 2, 0, 2, 3,
            4, 5, 6, 4, 6, 7,
            8, 9,10, 8,10,11,
           12,13,14,12,14,15,
           16,17,18,16,18,19,
           20,21,22,20,22,23,
        };

        private static readonly float[] s_quad =
        {
            1f, -1f,
            1f,  1f,
           -1f, -1f,
           -1f,  1f,
        };

        public void Draw(AngleGl gl, int width, int height)
        {
            if (gl == null) throw new ArgumentNullException(nameof(gl));
            if (_gl == null)
                _gl = gl;
            else if (!ReferenceEquals(_gl, gl))
                throw new InvalidOperationException("Renderer cannot be shared across different AngleGl instances.");

            if (!_initialized)
                Initialize(gl, width, height);

            ReloadPrograms(gl);
            EnsureFramebuffer(gl, width, height);

            // Draw to multiview fbo.
            {
                // Generate the perspective projection matrix.
                var viewWidth = width / 2;
                var viewHeight = height;
                const float kFOV = 90.0f;
                const float kNear = 1.0f;
                const float kFar = 100.0f;
                var kPlaneDifference = kFar - kNear;
                var kXYScale = 1.0f / (float)Math.Tan(kFOV * Math.PI / 360.0f); // tan(fov/2)
                var kAspectRatio = (float)viewWidth / viewHeight;
                
                var kPerspectiveProjectionMatrix = new float[16];
                kPerspectiveProjectionMatrix[0] = kXYScale / kAspectRatio;
                kPerspectiveProjectionMatrix[1] = 0.0f;
                kPerspectiveProjectionMatrix[2] = 0.0f;
                kPerspectiveProjectionMatrix[3] = 0.0f;

                kPerspectiveProjectionMatrix[4] = 0.0f;
                kPerspectiveProjectionMatrix[5] = kXYScale;
                kPerspectiveProjectionMatrix[6] = 0.0f;
                kPerspectiveProjectionMatrix[7] = 0.0f;

                kPerspectiveProjectionMatrix[8] = 0.0f;
                kPerspectiveProjectionMatrix[9] = 0.0f;
                kPerspectiveProjectionMatrix[10] = -kFar / kPlaneDifference;
                kPerspectiveProjectionMatrix[11] = -kFar * kNear / kPlaneDifference;

                kPerspectiveProjectionMatrix[12] = 0.0f;
                kPerspectiveProjectionMatrix[13] = 0.0f;
                kPerspectiveProjectionMatrix[14] = -1.0f;
                kPerspectiveProjectionMatrix[15] = 0.0f;

                // Generate the camera matrices for the left and right eye.
                const float kXOffset = 1.5f;
                const float kYOffset = 1.5f;
                const float kZOffset = 5.0f;
                var kLeftCameraMatrix = new float[16];
                FillTranslationMatrix(kXOffset, -kYOffset, -kZOffset, kLeftCameraMatrix);
                var kRightCameraMatrix = new float[16];
                FillTranslationMatrix(-kXOffset, -kYOffset, -kZOffset, kRightCameraMatrix);

                // Bind and clear the multiview framebuffer.
                gl.Framebuffers.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo ?? throw new InvalidOperationException("FBO not initialized."));
                gl.State.ClearColor(0f, 0f, 0f, 1f);
                gl.State.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                // Set the viewport to be the size as one of the views.
                gl.State.Viewport(0, 0, viewWidth, viewHeight);

                // Bind multiview program and set matrices.
                var mvProgram = _multiviewProgramReloader?.Current ?? throw new InvalidOperationException("Multiview program not loaded.");
                gl.Programs.UseProgram(mvProgram);
                gl.Programs.UniformMatrix4fv(_mvPerspectiveLoc, false, kPerspectiveProjectionMatrix);
                gl.Programs.UniformMatrix4fv(_mvLeftEyeLoc, false, kLeftCameraMatrix);
                gl.Programs.UniformMatrix4fv(_mvRightEyeLoc, false, kRightCameraMatrix);

                BindCube(gl);

                // Draw first cube.
                var kTranslationMatrix = new float[16];
                FillTranslationMatrix(0.0f, 0.0f, 0.0f, kTranslationMatrix);
                gl.Programs.UniformMatrix4fv(_mvTranslationLoc, false, kTranslationMatrix);
                gl.Draw.DrawElements(PrimitiveType.Triangles, s_cubeIndices.Length, IndexElementType.UnsignedShort, 0);

                // Draw second cube.
                FillTranslationMatrix(1.0f, 1.0f, -2.0f, kTranslationMatrix);
                gl.Programs.UniformMatrix4fv(_mvTranslationLoc, false, kTranslationMatrix);
                gl.Draw.DrawElements(PrimitiveType.Triangles, s_cubeIndices.Length, IndexElementType.UnsignedShort, 0);

                // gl.VertexArrays.BindVertexArray(new GlVertexArrayHandle(0)); // Cpp does this, optional but good for safety
            }

            // Combine both views.
            {
                // Bind the default framebuffer object and clear.
                gl.Framebuffers.BindFramebuffer(FramebufferTarget.Framebuffer, new GlFramebufferHandle(0));
                gl.State.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                // Set the viewport to cover the whole screen.
                gl.State.Viewport(0, 0, width, height);

                var combineProgram = _blitProgramReloader?.Current ?? throw new InvalidOperationException("Combine program not loaded.");
                gl.Programs.UseProgram(combineProgram);

                // Bind the 2D array texture to be used as a sampler.
                gl.Programs.Uniform1i(_combineMultiviewTexLoc, 0);
                gl.Textures.ActiveTexture(TextureUnit.Texture0);
                gl.Textures.BindTexture(TextureTarget.Texture2DArray, _colorArray ?? throw new InvalidOperationException("Texture not initialized."));

                // Draw a quad which covers the whole screen. Layer and texture coordinates are
                // calculated in the vertex shader based on the UV coordinates of the quad.
                BindQuad(gl);
                gl.Draw.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
                
                // gl.VertexArrays.BindVertexArray(new GlVertexArrayHandle(0));
            }
        }

        public void Dispose()
        {
            _colorArray?.Dispose();
            _depthArray?.Dispose();
            _fbo?.Dispose();
            _cubeVao?.Dispose();
            _quadVao?.Dispose();
            _cubeBuffer?.Dispose();
            _cubeIndexBuffer?.Dispose();
            _quadBuffer?.Dispose();
            _multiviewProgramReloader?.Dispose();
            _blitProgramReloader?.Dispose();

            _colorArray = null;
            _depthArray = null;
            _fbo = null;
            _cubeVao = null;
            _quadVao = null;
            _cubeBuffer = null;
            _cubeIndexBuffer = null;
            _quadBuffer = null;
            _gl = null;
            _initialized = false;
            _useVao = false;
            _viewWidth = 0;
            _viewHeight = 0;
            _multiviewProgramReloader = null;
            _blitProgramReloader = null;
            _lastBlitError = null;
            _lastMvError = null;
        }

        public void SetHostContext(RendererHostContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            var roots = BuildShaderRoots(context, "Demo16_Multiview.Plugin.csproj");
            _multiviewProgramReloader = new ShaderProgramReloader(
                Path.Combine("Shaders", "Multiview.vert.glsl"),
                Path.Combine("Shaders", "Multiview.frag.glsl"),
                roots);
            _blitProgramReloader = new ShaderProgramReloader(
                Path.Combine("Shaders", "MultiviewBlit.vert.glsl"),
                Path.Combine("Shaders", "MultiviewBlit.frag.glsl"),
                roots);
        }

        private void Initialize(AngleGl gl, int width, int height)
        {
            if (!gl.Framebuffers.IsMultiviewAvailable)
                throw new NotSupportedException("GL_OVR_multiview2 不可用，无法运行 multiview。");

            EnsureProgramReloaders();
            _multiviewProgramReloader?.Load(gl);
            _blitProgramReloader?.Load(gl);
            UpdateProgramBindings(gl);

            _cubeBuffer = gl.Buffers.CreateBufferObject();
            gl.Buffers.BindBuffer(BufferTarget.ArrayBuffer, _cubeBuffer);
            gl.Buffers.BufferData(BufferTarget.ArrayBuffer, s_cubeVertices, BufferUsage.StaticDraw);

            _cubeIndexBuffer = gl.Buffers.CreateBufferObject();
            gl.Buffers.BindBuffer(BufferTarget.ElementArrayBuffer, _cubeIndexBuffer);
            gl.Buffers.BufferData(BufferTarget.ElementArrayBuffer, s_cubeIndices, BufferUsage.StaticDraw);

            _quadBuffer = gl.Buffers.CreateBufferObject();
            gl.Buffers.BindBuffer(BufferTarget.ArrayBuffer, _quadBuffer);
            gl.Buffers.BufferData(BufferTarget.ArrayBuffer, s_quad, BufferUsage.StaticDraw);

            try
            {
                _cubeVao = gl.VertexArrays.CreateVertexArrayObject();
                _quadVao = gl.VertexArrays.CreateVertexArrayObject();
                _useVao = true;
                BindCube(gl);
                BindQuad(gl);
            }
            catch (NotSupportedException)
            {
                _useVao = false;
                _cubeVao = null;
                _quadVao = null;
            }

            _fbo = gl.Framebuffers.CreateFramebufferObject();
            _colorArray = gl.Textures.CreateTextureObject();
            _depthArray = gl.Textures.CreateTextureObject();
            gl.State.Enable(EnableCap.DepthTest);
            gl.Textures.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            _initialized = true;
            BindGeometry(gl);
            EnsureFramebuffer(gl, width, height);
        }

        private void EnsureFramebuffer(AngleGl gl, int width, int height)
        {
            var viewW = Math.Max(1, width / 2);
            var viewH = Math.Max(1, height);
            if (_colorArray != null && viewW == _viewWidth && viewH == _viewHeight)
                return;

            _viewWidth = viewW;
            _viewHeight = viewH;

            gl.Textures.ActiveTexture(TextureUnit.Texture0);
            gl.Textures.BindTexture(TextureTarget.Texture2DArray, _colorArray ?? throw new InvalidOperationException("Texture not initialized."));
            gl.Textures.TexImage3D(
                TextureTarget.Texture2DArray,
                level: 0,
                internalFormat: TextureInternalFormat.Rgba8,
                width: viewW,
                height: viewH,
                depth: ViewCount,
                border: 0,
                format: PixelFormat.Rgba,
                type: PixelType.UnsignedByte,
                pixels: null);
            gl.Textures.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            gl.Textures.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            gl.Textures.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            gl.Textures.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            gl.Framebuffers.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo ?? throw new InvalidOperationException("FBO not initialized."));
            gl.Framebuffers.FramebufferTextureMultiview(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, _colorArray, 0, 0, ViewCount);
            if (_depthArray != null)
            {
                gl.Textures.BindTexture(TextureTarget.Texture2DArray, _depthArray);
                gl.Textures.TexImage3D(
                    TextureTarget.Texture2DArray,
                    level: 0,
                    internalFormat: TextureInternalFormat.DepthComponent24,
                    width: viewW,
                    height: viewH,
                    depth: ViewCount,
                    border: 0,
                    format: PixelFormat.DepthComponent,
                    type: PixelType.UnsignedInt,
                    pixels: null);
                gl.Textures.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                gl.Textures.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                gl.Textures.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                gl.Textures.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                gl.Framebuffers.FramebufferTextureMultiview(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, _depthArray, 0, 0, ViewCount);
            }

            var status = gl.Framebuffers.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferStatus.FramebufferComplete)
                throw new InvalidOperationException("Framebuffer incomplete: " + status);
            gl.Framebuffers.BindFramebuffer(FramebufferTarget.Framebuffer, new GlFramebufferHandle(0));
        }

        private void ReloadPrograms(AngleGl gl)
        {
            var mv = _multiviewProgramReloader ?? throw new InvalidOperationException("Multiview program reloader not initialized.");
            var blit = _blitProgramReloader ?? throw new InvalidOperationException("Blit program reloader not initialized.");
            var updated = mv.TryReload(gl);
            updated |= blit.TryReload(gl);

            var mvError = mv.LastError;
            if (mvError != null && !ReferenceEquals(mvError, _lastMvError))
            {
                _lastMvError = mvError;
                Console.Error.WriteLine(mvError);
            }

            var blitError = blit.LastError;
            if (blitError != null && !ReferenceEquals(blitError, _lastBlitError))
            {
                _lastBlitError = blitError;
                Console.Error.WriteLine(blitError);
            }

            if (updated)
                UpdateProgramBindings(gl);
        }

        private void BindGeometry(AngleGl gl)
        {
            BindCube(gl);
            BindQuad(gl);
        }

        private static void FillTranslationMatrix(float xOffset, float yOffset, float zOffset, float[] matrix)
        {
            matrix[0] = 1.0f;
            matrix[1] = 0.0f;
            matrix[2] = 0.0f;
            matrix[3] = 0.0f;

            matrix[4] = 0.0f;
            matrix[5] = 1.0f;
            matrix[6] = 0.0f;
            matrix[7] = 0.0f;

            matrix[8] = 0.0f;
            matrix[9] = 0.0f;
            matrix[10] = 1.0f;
            matrix[11] = 0.0f;

            matrix[12] = xOffset;
            matrix[13] = yOffset;
            matrix[14] = zOffset;
            matrix[15] = 1.0f;
        }

        private void UpdateProgramBindings(AngleGl gl)
        {
            var mv = _multiviewProgramReloader?.Current ?? throw new InvalidOperationException("Multiview program not loaded.");
            var blit = _blitProgramReloader?.Current ?? throw new InvalidOperationException("Blit program not loaded.");

            _mvPerspectiveLoc = gl.Programs.GetUniformLocation(mv, "uPerspective");
            _mvLeftEyeLoc = gl.Programs.GetUniformLocation(mv, "uCameraLeftEye");
            _mvRightEyeLoc = gl.Programs.GetUniformLocation(mv, "uCameraRightEye");
            _mvTranslationLoc = gl.Programs.GetUniformLocation(mv, "uTranslation");
            
            _combineMultiviewTexLoc = gl.Programs.GetUniformLocation(blit, "uMultiviewTex");
            _combinePositionLoc = gl.Programs.GetAttribLocation(blit, "vIn");

            if (_mvPerspectiveLoc < 0 || _mvLeftEyeLoc < 0 || _mvRightEyeLoc < 0 || _mvTranslationLoc < 0 || _combineMultiviewTexLoc < 0 || _combinePositionLoc < 0)
                throw new InvalidOperationException("Failed to resolve shader bindings.");

            if (_cubeBuffer != null && _cubeIndexBuffer != null && _quadBuffer != null)
                BindGeometry(gl);
        }

        private void BindCube(AngleGl gl)
        {
            if (_useVao && _cubeVao != null)
                gl.VertexArrays.BindVertexArray(_cubeVao);
            gl.Buffers.BindBuffer(BufferTarget.ArrayBuffer, _cubeBuffer ?? throw new InvalidOperationException("Cube buffer not initialized."));
            gl.Buffers.BindBuffer(BufferTarget.ElementArrayBuffer, _cubeIndexBuffer ?? throw new InvalidOperationException("Index buffer not initialized."));
            
            // pos (loc 0)
            gl.VertexArrays.EnableVertexAttribArray(0);
            gl.VertexArrays.VertexAttribPointer(0, 3, VertexAttribType.Float, false, 6 * sizeof(float), 0);
            
            // normal (loc 1)
            gl.VertexArrays.EnableVertexAttribArray(1);
            gl.VertexArrays.VertexAttribPointer(1, 3, VertexAttribType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        }

        private void BindQuad(AngleGl gl)
        {
            if (_useVao && _quadVao != null)
                gl.VertexArrays.BindVertexArray(_quadVao);
            gl.Buffers.BindBuffer(BufferTarget.ArrayBuffer, _quadBuffer ?? throw new InvalidOperationException("Quad buffer not initialized."));
            
            if (_combinePositionLoc >= 0)
            {
                gl.VertexArrays.EnableVertexAttribArray(_combinePositionLoc);
                gl.VertexArrays.VertexAttribPointer(_combinePositionLoc, 2, VertexAttribType.Float, false, 2 * sizeof(float), 0);
            }
        }

        private void EnsureProgramReloaders()
        {
            if (_multiviewProgramReloader != null && _blitProgramReloader != null) return;
            var roots = new[]
            {
                AppContext.BaseDirectory,
                Directory.GetCurrentDirectory(),
            };
            _multiviewProgramReloader ??= new ShaderProgramReloader(
                Path.Combine("Shaders", "Multiview.vert.glsl"),
                Path.Combine("Shaders", "Multiview.frag.glsl"),
                roots);
            _blitProgramReloader ??= new ShaderProgramReloader(
                Path.Combine("Shaders", "MultiviewBlit.vert.glsl"),
                Path.Combine("Shaders", "MultiviewBlit.frag.glsl"),
                roots);
        }

        private static string[] BuildShaderRoots(RendererHostContext context, string projectFileName)
        {
            var roots = new[]
            {
                context.PluginDirectory,
                AppContext.BaseDirectory,
                Directory.GetCurrentDirectory(),
            };

            var projectRoot = TryGetProjectRootFromPluginOutput(context.PluginDirectory, projectFileName);
            if (!string.IsNullOrWhiteSpace(projectRoot))
            {
                return new[]
                {
                    projectRoot,
                    context.PluginDirectory,
                    AppContext.BaseDirectory,
                    Directory.GetCurrentDirectory(),
                };
            }

            return roots;
        }

        private static string? TryGetProjectRootFromPluginOutput(string pluginDir, string projectFileName)
        {
            try
            {
                var di = new DirectoryInfo(pluginDir);
                for (var i = 0; i < 5 && di != null; i++)
                {
                    var candidate = Path.Combine(di.FullName, projectFileName);
                    if (File.Exists(candidate))
                        return di.FullName;
                    di = di.Parent;
                }
            }
            catch
            {
            }

            return null;
        }
    }
}
