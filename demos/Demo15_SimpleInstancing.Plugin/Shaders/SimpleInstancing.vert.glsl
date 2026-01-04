#version 100
attribute vec3 a_position;
attribute vec2 a_texCoord;
attribute vec3 a_instancePos;
varying vec2 v_texCoord;
void main()
{
    gl_Position = vec4(a_position + a_instancePos, 1.0);
    v_texCoord = a_texCoord;
}
