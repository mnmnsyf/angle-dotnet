#version 300 es
precision mediump float;
in vec3 v_normal;
out vec4 o_color;
void main()
{
    vec3 n = normalize(v_normal);
    vec3 lightDir = normalize(vec3(0.3, 0.7, 0.6));
    float diff = max(dot(n, lightDir), 0.0);
    vec3 base = vec3(0.3, 0.6, 1.0);
    vec3 color = base * (0.2 + 0.8 * diff);
    o_color = vec4(color, 1.0);
}
