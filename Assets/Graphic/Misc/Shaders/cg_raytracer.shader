Shader "Custom/cg_raytracer" {
    Properties {
        _MaxReflect("Max Reflecte", Int) = 5
        
        _PlaneReflectiveness("Plane Reflectiveness", Range(0., 1.)) = .5

        _LeftSphereMat ("Left Spere Material (1 lambertian 2 phong, 3 blinn-phong)", Int) = 3
        _LeftSphereColor ("Left Sphere Color", Color) = (1.,0.,0.,1.)
        _LeftSphereShininess ("Left Sphere Shininess", Float) = 10.
        _LeftSphereReflectiveness ("Left Sphere Reflectiveness", Range(0., 1.)) = .25

        _RightSphereMat ("Right Spere Material (1 lambertian 2 phong, 3 blinn-phong)", Int) = 3
        _RightSphereColor ("Right Sphere Color", Color) = (0.,0.,1.,1.)
        _RightSphereShininess ("Right Sphere Shininess", Float) = 10.
        _RightSphereReflectiveness ("Right Sphere Reflectiveness", Range(0., 1.)) = .25

        _LightType ("Light Type", Int) = 3
        _LightPosition ("Light Position", Vector) = (5.,5.,0.,1.)
        _LightDirection ("Light Dirtection", Vector) = (-5.,-7.,5.,0.)
        _LightColor ("Light Color", Color) = (1.,1.,1.,1.)
        _LightIntensity ("Light Intensity", Range(0.,10.)) = 1.
        _LightAttenParameter ("Light Atten Parameter (atten0, atten1, atten2, range)", Vector) = (1.,2.,.5,50.)
        _SpotLightParameter ("Spot Light Parameter (phi, theta, falloff, 0.)", Vector) = (120.,20.,2.,0.)
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        Pass{
        CGPROGRAM

        #pragma vertex vert
        #pragma fragment frag
        #pragma target 3.0
        #pragma only_renderers d3d9 d3d11 opengl gles gles3 
        #pragma glsl

        float4 vert(float4 v:POSITION) : SV_POSITION {
            return mul (UNITY_MATRIX_MVP, v);
        }

        uniform fixed _PlaneReflectiveness;
        uniform int _MaxReflect;
        uniform int _LeftSphereMat;
        uniform fixed4 _LeftSphereColor;
        uniform fixed _LeftSphereShininess;
        uniform fixed _LeftSphereReflectiveness;
        uniform int _RightSphereMat;
        uniform fixed4 _RightSphereColor;
        uniform fixed _RightSphereShininess;
        uniform fixed _RightSphereReflectiveness;
        uniform int _LightType;
        uniform float3 _LightPosition;
        uniform float3 _LightDirection;
        uniform fixed4 _LightColor;
        uniform fixed _LightIntensity;
        uniform fixed4 _LightAttenParameter;
        uniform fixed3 _SpotLightParameter;
        fixed4 _ColorBlack;
        fixed4 _ColorWhite;

        struct Ray {
            float3 origin;
            float3 direction;
        };

        struct Sphere {
            float3 center;
            fixed radius;
        };

        struct Plane {
            float3 normal;
            float distance;
        };

        struct IntersectionResult {
            fixed distance;
            float3 position;
            float3 normal;

            int object;
            int material;
        };

        Ray theRay;

        float3 lightDirection;
        float spotLightCosHalfPhi;
        float spotLightCosHalfTheta;

        Plane thePlane;
        Sphere leftSphere;
        Sphere rightSphere;

        IntersectionResult minIntersection(IntersectionResult a, IntersectionResult b)
        {
            if (a.distance < 0.) return b;
            if (b.distance < 0.) return a;
            if (a.distance < b.distance) return a;
            else return b;
        }

        float intersectSphere(Ray ray, Sphere sphere) {
            float3 co = ray.origin - sphere.center;
            float discriminant = dot(co, ray.direction) * dot(co, ray.direction)
                    - (dot(co, co) - sphere.radius * sphere.radius);

            if (discriminant >= 0.0)
                return -dot(ray.direction, co) - sqrt(discriminant);
            else
                return -1.;
        }

        float intersectPlane(Ray ray, Plane plane) {
            float a = dot(ray.direction, plane.normal);
            if (a >= 0.) return -1.;
            
            float3 position = plane.normal * plane.distance;
            float b = dot(plane.normal, ray.origin - position);
            if (b <= 0.) return -1.;
            
            return -b / a;
        }

        IntersectionResult intersectScene(Ray ray)
        {
            float dis = -1.;

            dis = intersectSphere(ray, leftSphere);
            IntersectionResult leftSphereIR;
            leftSphereIR.distance = dis;
            if (dis >= 0.)
            {
                leftSphereIR.position = ray.origin + leftSphereIR.distance * ray.direction;
                leftSphereIR.normal = normalize(leftSphereIR.position - leftSphere.center);
                leftSphereIR.object = 2;
                leftSphereIR.material = _LeftSphereMat;
            }

            dis = intersectSphere(ray, rightSphere);
            IntersectionResult rightSphereIR;
            rightSphereIR.distance = dis;
            if (dis >= 0.)
            {
                rightSphereIR.position = ray.origin + rightSphereIR.distance * ray.direction;
                rightSphereIR.normal = normalize(rightSphereIR.position - rightSphere.center);
                rightSphereIR.object = 3;
                rightSphereIR.material = _RightSphereMat;
            }

            dis = intersectPlane(ray, thePlane);
            IntersectionResult planeIR;
            planeIR.distance = dis;
            if (dis >= 0.)
            {
                planeIR.position = ray.origin + planeIR.distance * ray.direction;
                planeIR.normal = thePlane.normal;
                planeIR.object = 1;
                planeIR.material = 0;
            }

            IntersectionResult ret = minIntersection(leftSphereIR, rightSphereIR);
            ret = minIntersection(planeIR, ret);
            return ret;
        }


        float calcShadow(float3 position, float3 lightDir)
        {
            Ray shadowRay;
            shadowRay.origin = position;
            shadowRay.direction = lightDir;
            IntersectionResult ret = intersectScene(shadowRay);
            if (ret.distance >= 0.) return 0.;
            else return 1.;
        }

        void calcLight(out float3 lightDir, out fixed4 lightColor, float3 position)
        {
            if (_LightType == 1)
            {
                lightDir = -lightDirection;
                lightColor = _LightColor * _LightIntensity;
            }
            else if (_LightType == 2 || _LightType == 3)
            {
                lightDir = _LightPosition - position;
                float distance = length(lightDir);
                lightDir /= distance;

                float atten = _LightAttenParameter.w / (_LightAttenParameter.z * distance * distance + _LightAttenParameter.y * distance + _LightAttenParameter.x);

                if (_LightType == 3)
                {
                    float rio = dot(-lightDirection, lightDir);
                    float factor = 1.;
                    if (rio <= spotLightCosHalfPhi) factor = 0.;
                    else if (rio > spotLightCosHalfTheta) factor = 1.;
                    else
                    {
                        factor = (rio - spotLightCosHalfPhi) / (spotLightCosHalfTheta - spotLightCosHalfPhi);
                        factor = clamp(pow(factor, _SpotLightParameter.z), 0., 1.);
                    }
                    lightColor = _LightColor * (_LightIntensity * factor * atten);
                }
                else lightColor = _LightColor * (_LightIntensity * atten);
            }

            lightColor *= calcShadow(position, lightDir);
        }

        void getDrawParameter(int object, out fixed4 color, out fixed shininess)
        {
            if (object == 2)
            {
                color = _LeftSphereColor;
                shininess = _LeftSphereShininess;
            }
            else if (object == 3)
            {
                color = _RightSphereColor;
                shininess = _RightSphereShininess;
            }
        }

        fixed4 calcColor(int object, int material, float3 position, float3 normal)
        {
            float3 lightDir;
            fixed4 lightColor;
            calcLight(lightDir, lightColor, position);

            fixed4 color = _ColorWhite;
            fixed shininess = 10.;
            getDrawParameter(object, color, shininess);

            if (material == 0)
            {
                float tmp = abs(floor(position.x * 1.) + floor(position.z * 1.));
                return ((fmod(tmp, 2.) < 1.) ? _ColorBlack : _ColorWhite) * lightColor;
            }
            else if (material == 1)
            {
                return color * (max(0.0, dot(normal, lightDir)) * .8 + .2) * lightColor;
            }
            else if (material == 2)
            {
                float lambertian = max(0.0, dot(normal, lightDir));
                float specular = 0.0;
                if (lambertian > 0.0)
                {
                    float3 reflectDir = normalize(normal * dot(normal, lightDir) * 2. - lightDir);
                    float specAngle = max(dot(reflectDir, normalize(theRay.origin - position)), 0.);
                    specular = pow(specAngle, shininess);
                }
                
                return (color * lambertian + _ColorWhite * specular) * lightColor;
            }
            else if (material == 3)
            {
                float lambertian = max(0.0, dot(normal, lightDir));
                float specular = 0.0;
                if (lambertian > 0.0)
                {
                    float3 halfDir = normalize(lightDir + normalize(theRay.origin - position));
                    float specAngle = max(dot(halfDir, normal), 0.);
                    specular = pow(specAngle, shininess * 4.);
                }
                
                return (color * lambertian + _ColorWhite * specular) * lightColor;
            }
            return _ColorBlack;
        }

        float getReflectiveness(int object)
        {
            if (object == 1) return _PlaneReflectiveness;
            else if (object == 2) return _LeftSphereReflectiveness;
            else if (object == 3) return _RightSphereReflectiveness;
            else return 0.5;
        }

        fixed4 rayTrace(Ray ray)
        {
            fixed4 color = _ColorBlack;
#if SHADER_API_D3D9 || SHADER_API_GLES || SHADER_API_GLES3
            float reflectiveness = 1.0;
            IntersectionResult ret = intersectScene(ray);
            if (ret.distance >= 0.)
            {
                float objReflectiveness = getReflectiveness(ret.object);
                fixed4 sampleColor = calcColor(ret.object, ret.material, ret.position, ret.normal);
                color += sampleColor * (1. - objReflectiveness) * reflectiveness;

                if (_MaxReflect >= 1)
                {
                    reflectiveness *= objReflectiveness;

                    if (reflectiveness > 0.)
                    {
                        float3 reflectDir = normalize(ret.normal * (-2. * dot(ret.normal, ray.direction)) + ray.direction);
                        ray.origin = ret.position;
                        ray.direction = reflectDir;

                        ret = intersectScene(ray);
                        if (ret.distance >= 0.)
                        {
                            objReflectiveness = getReflectiveness(ret.object);
                            sampleColor = calcColor(ret.object, ret.material, ret.position, ret.normal);
                            color += sampleColor * (1. - objReflectiveness) * reflectiveness;
                        }
                    }
                }
            }
#else
            float reflectiveness = 1.0;
            for (int i=0; i <= _MaxReflect; ++i)
            {
                IntersectionResult ret = intersectScene(ray);
                if (ret.distance >= 0.)
                {
                    float objReflectiveness = getReflectiveness(ret.object);
                    fixed4 sampleColor = calcColor(ret.object, ret.material, ret.position, ret.normal);
                    color += sampleColor * (1. - objReflectiveness) * reflectiveness;
                    reflectiveness *= objReflectiveness;

                    if (reflectiveness > 0.)
                    {
                        float3 reflectDir = normalize(ret.normal * (-2. * dot(ret.normal, ray.direction)) + ray.direction);
                        ray.origin = ret.position;
                        ray.direction = reflectDir;
                    }
                    else break;
                }
                else break;
            }
#endif
            return color;
        }

        fixed4 frag(float4 fragCoord:VPOS) : COLOR {

            _ColorBlack = fixed4(0.,0.,0.,1.);
            _ColorWhite = fixed4(1.,1.,1.,1.);

            lightDirection = normalize(_LightDirection);
            spotLightCosHalfPhi = cos(radians(_SpotLightParameter.x / 2));
            spotLightCosHalfTheta = cos(radians(_SpotLightParameter.y / 2));

            thePlane.normal = float3(0.,1.,0.);
            thePlane.distance = -2.;

            leftSphere.center = float3(-2.5,0.,5.);
            leftSphere.radius = 2.;

            rightSphere.center = float3(2.5,0.,5.);
            rightSphere.radius = 2.;

            float2 posPixel = fragCoord.xy / _ScreenParams.xy * 2. - 1.;
#if SHADER_API_D3D9  || SHADER_API_D3D11
            posPixel.y = -posPixel.y;
#endif
            float radio = _ScreenParams.x / _ScreenParams.y;

            theRay.origin = float3(sin(_Time.y*0.2) * 0.2, sin(_Time.y*0.3) * 0.2, -1.);
            theRay.direction = normalize(float3(posPixel.x * radio, posPixel.y, 0) - theRay.origin);

            return rayTrace(theRay);
        }

        ENDCG
        }
    }
}

