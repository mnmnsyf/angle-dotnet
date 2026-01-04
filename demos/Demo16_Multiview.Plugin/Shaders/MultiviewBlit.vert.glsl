#version 300 es
in vec2 vIn;
out vec2 uv;
void main()
{
   gl_Position = vec4(vIn, 0., 1.);
   uv = vIn * .5 + vec2(.5);
}
