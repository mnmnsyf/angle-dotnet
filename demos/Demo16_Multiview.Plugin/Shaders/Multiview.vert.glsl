#version 300 es
#extension GL_OVR_multiview2 : require
layout(num_views = 2) in;
layout(location=0) in vec3 posIn;
layout(location=1) in vec3 normalIn;
uniform mat4 uPerspective;
uniform mat4 uCameraLeftEye;
uniform mat4 uCameraRightEye;
uniform mat4 uTranslation;
out vec3 oNormal;
void main()
{
   vec4 p = uTranslation * vec4(posIn,1.);
   if (gl_ViewID_OVR == 0u) {
       p = uCameraLeftEye * p;
   } else {
       p = uCameraRightEye * p;
   }
   oNormal = normalIn;
   gl_Position = uPerspective * p;
}
