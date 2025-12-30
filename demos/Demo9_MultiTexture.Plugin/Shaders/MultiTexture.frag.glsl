#version 300 es
precision mediump float;
in vec2 v_texCoord;
uniform sampler2D s_baseMap;
uniform sampler2D s_lightMap;
out vec4 o_color;
void main()
{
    vec4 baseColor = texture(s_baseMap, v_texCoord);
    vec4 lightColor = texture(s_lightMap, v_texCoord);
    o_color = baseColor * (lightColor + 0.25);
}
