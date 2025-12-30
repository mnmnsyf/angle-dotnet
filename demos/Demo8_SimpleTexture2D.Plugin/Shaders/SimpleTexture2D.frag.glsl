#version 300 es
precision mediump float;
in vec2 v_texCoord;
uniform sampler2D s_texture;
out vec4 o_color;
void main()
{
    o_color = texture(s_texture, v_texCoord);
}
