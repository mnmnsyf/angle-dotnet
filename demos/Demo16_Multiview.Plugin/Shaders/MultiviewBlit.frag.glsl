#version 300 es
precision mediump float;
precision mediump sampler2DArray;
uniform sampler2DArray uMultiviewTex;
in vec2 uv;
out vec4 color;
void main()
{
   float scaledX = 2.0 * uv.x;
   float layer = floor(scaledX);
   vec2 adjustedUV = vec2(fract(scaledX), uv.y);
   vec3 texColor = texture(uMultiviewTex, vec3(adjustedUV, layer)).rgb;
   color = vec4(texColor, 1.);
}
