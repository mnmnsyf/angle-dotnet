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
}
