using System;

namespace AngleBridge.Sdk.Interop
{
    public readonly struct GlShaderHandle : IEquatable<GlShaderHandle>
    {
        public GlShaderHandle(int value) { Value = value; }
        public int Value { get; }
        public bool IsNull { get { return Value == 0; } }
        public bool Equals(GlShaderHandle other) { return Value == other.Value; }
        public override bool Equals(object? obj) { return obj is GlShaderHandle other && Equals(other); }
        public override int GetHashCode() { return Value; }
        public override string ToString() { return Value.ToString(); }
        public static bool operator ==(GlShaderHandle left, GlShaderHandle right) { return left.Equals(right); }
        public static bool operator !=(GlShaderHandle left, GlShaderHandle right) { return !left.Equals(right); }
    }

    public readonly struct GlProgramHandle : IEquatable<GlProgramHandle>
    {
        public GlProgramHandle(int value) { Value = value; }
        public int Value { get; }
        public bool IsNull { get { return Value == 0; } }
        public bool Equals(GlProgramHandle other) { return Value == other.Value; }
        public override bool Equals(object? obj) { return obj is GlProgramHandle other && Equals(other); }
        public override int GetHashCode() { return Value; }
        public override string ToString() { return Value.ToString(); }
        public static bool operator ==(GlProgramHandle left, GlProgramHandle right) { return left.Equals(right); }
        public static bool operator !=(GlProgramHandle left, GlProgramHandle right) { return !left.Equals(right); }
    }

    public readonly struct GlBufferHandle : IEquatable<GlBufferHandle>
    {
        public GlBufferHandle(int value) { Value = value; }
        public int Value { get; }
        public bool IsNull { get { return Value == 0; } }
        public bool Equals(GlBufferHandle other) { return Value == other.Value; }
        public override bool Equals(object? obj) { return obj is GlBufferHandle other && Equals(other); }
        public override int GetHashCode() { return Value; }
        public override string ToString() { return Value.ToString(); }
        public static bool operator ==(GlBufferHandle left, GlBufferHandle right) { return left.Equals(right); }
        public static bool operator !=(GlBufferHandle left, GlBufferHandle right) { return !left.Equals(right); }
    }

    public readonly struct GlVertexArrayHandle : IEquatable<GlVertexArrayHandle>
    {
        public GlVertexArrayHandle(int value) { Value = value; }
        public int Value { get; }
        public bool IsNull { get { return Value == 0; } }
        public bool Equals(GlVertexArrayHandle other) { return Value == other.Value; }
        public override bool Equals(object? obj) { return obj is GlVertexArrayHandle other && Equals(other); }
        public override int GetHashCode() { return Value; }
        public override string ToString() { return Value.ToString(); }
        public static bool operator ==(GlVertexArrayHandle left, GlVertexArrayHandle right) { return left.Equals(right); }
        public static bool operator !=(GlVertexArrayHandle left, GlVertexArrayHandle right) { return !left.Equals(right); }
    }

    public readonly struct GlTextureHandle : IEquatable<GlTextureHandle>
    {
        public GlTextureHandle(int value) { Value = value; }
        public int Value { get; }
        public bool IsNull { get { return Value == 0; } }
        public bool Equals(GlTextureHandle other) { return Value == other.Value; }
        public override bool Equals(object? obj) { return obj is GlTextureHandle other && Equals(other); }
        public override int GetHashCode() { return Value; }
        public override string ToString() { return Value.ToString(); }
        public static bool operator ==(GlTextureHandle left, GlTextureHandle right) { return left.Equals(right); }
        public static bool operator !=(GlTextureHandle left, GlTextureHandle right) { return !left.Equals(right); }
    }

    public readonly struct GlFramebufferHandle : IEquatable<GlFramebufferHandle>
    {
        public GlFramebufferHandle(int value) { Value = value; }
        public int Value { get; }
        public bool IsNull { get { return Value == 0; } }
        public bool Equals(GlFramebufferHandle other) { return Value == other.Value; }
        public override bool Equals(object? obj) { return obj is GlFramebufferHandle other && Equals(other); }
        public override int GetHashCode() { return Value; }
        public override string ToString() { return Value.ToString(); }
        public static bool operator ==(GlFramebufferHandle left, GlFramebufferHandle right) { return left.Equals(right); }
        public static bool operator !=(GlFramebufferHandle left, GlFramebufferHandle right) { return !left.Equals(right); }
    }

    public readonly struct GlRenderbufferHandle : IEquatable<GlRenderbufferHandle>
    {
        public GlRenderbufferHandle(int value) { Value = value; }
        public int Value { get; }
        public bool IsNull { get { return Value == 0; } }
        public bool Equals(GlRenderbufferHandle other) { return Value == other.Value; }
        public override bool Equals(object? obj) { return obj is GlRenderbufferHandle other && Equals(other); }
        public override int GetHashCode() { return Value; }
        public override string ToString() { return Value.ToString(); }
        public static bool operator ==(GlRenderbufferHandle left, GlRenderbufferHandle right) { return left.Equals(right); }
        public static bool operator !=(GlRenderbufferHandle left, GlRenderbufferHandle right) { return !left.Equals(right); }
    }

    public readonly struct GlSamplerHandle : IEquatable<GlSamplerHandle>
    {
        public GlSamplerHandle(int value) { Value = value; }
        public int Value { get; }
        public bool IsNull { get { return Value == 0; } }
        public bool Equals(GlSamplerHandle other) { return Value == other.Value; }
        public override bool Equals(object? obj) { return obj is GlSamplerHandle other && Equals(other); }
        public override int GetHashCode() { return Value; }
        public override string ToString() { return Value.ToString(); }
        public static bool operator ==(GlSamplerHandle left, GlSamplerHandle right) { return left.Equals(right); }
        public static bool operator !=(GlSamplerHandle left, GlSamplerHandle right) { return !left.Equals(right); }
    }

    public readonly struct GlQueryHandle : IEquatable<GlQueryHandle>
    {
        public GlQueryHandle(int value) { Value = value; }
        public int Value { get; }
        public bool IsNull { get { return Value == 0; } }
        public bool Equals(GlQueryHandle other) { return Value == other.Value; }
        public override bool Equals(object? obj) { return obj is GlQueryHandle other && Equals(other); }
        public override int GetHashCode() { return Value; }
        public override string ToString() { return Value.ToString(); }
        public static bool operator ==(GlQueryHandle left, GlQueryHandle right) { return left.Equals(right); }
        public static bool operator !=(GlQueryHandle left, GlQueryHandle right) { return !left.Equals(right); }
    }

    public readonly struct GlSyncHandle : IEquatable<GlSyncHandle>
    {
        public GlSyncHandle(IntPtr value) { Value = value; }
        public IntPtr Value { get; }
        public bool IsNull { get { return Value == IntPtr.Zero; } }
        public bool Equals(GlSyncHandle other) { return Value == other.Value; }
        public override bool Equals(object? obj) { return obj is GlSyncHandle other && Equals(other); }
        public override int GetHashCode() { return Value.GetHashCode(); }
        public override string ToString() { return Value.ToString(); }
        public static bool operator ==(GlSyncHandle left, GlSyncHandle right) { return left.Equals(right); }
        public static bool operator !=(GlSyncHandle left, GlSyncHandle right) { return !left.Equals(right); }
    }

    public readonly struct GlTransformFeedbackHandle : IEquatable<GlTransformFeedbackHandle>
    {
        public GlTransformFeedbackHandle(int value) { Value = value; }
        public int Value { get; }
        public bool IsNull { get { return Value == 0; } }
        public bool Equals(GlTransformFeedbackHandle other) { return Value == other.Value; }
        public override bool Equals(object? obj) { return obj is GlTransformFeedbackHandle other && Equals(other); }
        public override int GetHashCode() { return Value; }
        public override string ToString() { return Value.ToString(); }
        public static bool operator ==(GlTransformFeedbackHandle left, GlTransformFeedbackHandle right) { return left.Equals(right); }
        public static bool operator !=(GlTransformFeedbackHandle left, GlTransformFeedbackHandle right) { return !left.Equals(right); }
    }
}
