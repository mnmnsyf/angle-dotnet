#version 300 es
precision highp float;

layout(location = 0) in vec4 a_position;
layout(location = 1) in vec2 a_texCoord;

uniform vec4 u_offset;
out vec2 v_texCoord;

void main()
{
    vec4 pos = a_position;
    pos.x += u_offset.x;
    gl_Position = pos;
    v_texCoord = a_texCoord;
}
