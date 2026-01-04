#version 300 es
layout(location = 0) in vec3 a_position;
layout(location = 1) in vec3 a_normal;
uniform mat4 u_model;
uniform mat4 u_view;
uniform mat4 u_proj;
out vec3 v_normal;
void main()
{
    mat4 mv = u_view * u_model;
    gl_Position = u_proj * mv * vec4(a_position, 1.0);
    v_normal = mat3(mv) * a_normal;
}
