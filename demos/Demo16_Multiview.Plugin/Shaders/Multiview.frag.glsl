#version 300 es
#extension GL_OVR_multiview2 : require
precision mediump float;
out vec4 color;
in vec3 oNormal;
void main()
{
   vec3 col = 0.5 * oNormal + vec3(0.5);
   color = vec4(col, 1.);
}
