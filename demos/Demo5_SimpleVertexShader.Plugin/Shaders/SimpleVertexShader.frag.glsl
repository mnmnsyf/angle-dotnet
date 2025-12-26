#version 300 es
precision mediump float;
in vec2 v_texcoord;
out vec4 o_color;
void main()
{
    o_color = vec4(v_texcoord.x, v_texcoord.y, 0.5, 1.0);
}
