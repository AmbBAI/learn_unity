//https://www.shadertoy.com/view/XtjGzc

Shader "Custom/shadertoy_raytracer" {
    Properties {
        iChannel0 ("iChannel0", 2D) = "black" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        Pass {

        GLSLPROGRAM
    
        #include "UnityCG.glslinc"

        vec4 iResolution = _ScreenParams;
        float iGlobalTime = _Time.y;
        uniform sampler2D iChannel0;


#ifdef VERTEX
        void main()
        {
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
        }
#endif

#ifdef FRAGMENT
        const int MAX_REFLECT = 5;
        const vec3 black = vec3(0.,0.,0.);
        const vec3 white = vec3(1.,1.,1.);
        const vec3 red = vec3(1.,0.,0.);
        const vec3 blue = vec3(0.,0.,1.);

        struct Ray {
            vec3 origin;
            vec3 direction;
        };

        struct Light {
            int type;
 
            vec3 position;
            vec3 direction;
            float range;

            vec3 color;
            float intensity;
        };

        struct Sphere {
            vec3 center;
            float radius;

            int material;
            vec3 color;
            float reflectiveness;
        };

        struct Plane {
            vec3 normal;
            float distance;

            int material;
            vec3 color;
            float reflectiveness;
        };
        
        struct IntersectionResult {
            float distance;
            vec3 position;
            vec3 normal;

            int material;
            vec3 color;
            float reflectiveness;
        };

        vec3 theEye = vec3(sin(iGlobalTime*0.2) * 0.2, sin(iGlobalTime*0.3) * 0.2, -1.);
        
        Light theLight = Light(3, vec3(5.,5.,0.), normalize(vec3(-5.,-7.,5.)), 50., white, 1.);
        //Light theLight = Light(2, vec3(0.,5.,5.), normalize(vec3(0.,-1.,0.)), 50., white, 1.);

        float spotLightCosHalfPhi = cos(radians(60.));
        float spotLightCosHalfTheta = cos(radians(45.));
        float spotLightFalloff = 1.;


        Sphere redSphere = Sphere(vec3(-2.5, 0, 5), 2., 11, red, 0.25);
        Sphere blueSphere = Sphere(vec3(2.5, 0, 5), 2., 11, blue, 0.25);
        Plane thePlane = Plane(vec3(0.,1.,0.), -2., 1, white, 0.5);

        vec3 specColor = white;
        float shininess = 10.;


        IntersectionResult minIntersection(IntersectionResult a, IntersectionResult b)
        {
            if (a.distance < 0.) return b;
            if (b.distance < 0.) return a;
            return (a.distance < b.distance) ? a : b;
        }

        float intersectPlane(Ray ray, Plane plane) {
            float a = dot(ray.direction, plane.normal);
            if (a >= 0.0) return -1.;
            
            vec3 position = plane.normal * plane.distance;
            float b = dot(plane.normal, ray.origin - position);
            if (b <= 0.0) return -1.;
            
            return -b / a;
        }

        float intersectSphere(Ray ray, Sphere sphere) {
            vec3 co = ray.origin - sphere.center;
            float discriminant = dot(co, ray.direction) * dot(co, ray.direction)
                    - (dot(co, co) - sphere.radius * sphere.radius);

            if (discriminant >= 0.0)
                return -dot(ray.direction, co) - sqrt(discriminant);
            else
                return -1.;
        }

        IntersectionResult intersectScene(Ray ray)
        {
            float dis = -1.;

            dis = intersectSphere(ray, redSphere);
            IntersectionResult redSphereIntersection;
            redSphereIntersection.distance = dis;
            if (dis >= 0.)
            {
                redSphereIntersection.position = ray.origin + redSphereIntersection.distance * ray.direction;
                redSphereIntersection.normal = normalize(redSphereIntersection.position - redSphere.center);
                redSphereIntersection.material = redSphere.material;
                redSphereIntersection.color = redSphere.color;
                redSphereIntersection.reflectiveness = redSphere.reflectiveness;
            }

            dis = intersectSphere(ray, blueSphere);
            IntersectionResult blueSphereIntersection;
            blueSphereIntersection.distance = dis;
            if (dis >= 0.)
            {
                blueSphereIntersection.position = ray.origin + blueSphereIntersection.distance * ray.direction;
                blueSphereIntersection.normal = normalize(blueSphereIntersection.position - blueSphere.center);
                blueSphereIntersection.material = blueSphere.material;
                blueSphereIntersection.color = blueSphere.color;
                blueSphereIntersection.reflectiveness = blueSphere.reflectiveness;
            }

            dis = intersectPlane(ray, thePlane);
            IntersectionResult planeIntersection;
            planeIntersection.distance = dis;
            if (dis >= 0.)
            {
                planeIntersection.position = ray.origin + planeIntersection.distance * ray.direction;
                planeIntersection.normal = thePlane.normal;
                planeIntersection.material = thePlane.material;
                planeIntersection.color = thePlane.color;
                planeIntersection.reflectiveness = thePlane.reflectiveness;
            }

            IntersectionResult ret = minIntersection(redSphereIntersection, blueSphereIntersection);
            ret = minIntersection(planeIntersection, ret);
            return ret;
        }

        float calcShadow(vec3 position, vec3 lightDir)
        {
            Ray shadowRay = Ray(position, lightDir);
            IntersectionResult ret = intersectScene(shadowRay);
            if (ret.distance >= 0.) return 0.;
            else return 1.;
        }

        void calcLight(out vec3 lightDir, out vec3 lightColor, vec3 position)
        {
            float intensity = theLight.intensity;
            if (theLight.type == 1)
            {
                lightDir = -theLight.direction;
                lightColor = theLight.color * intensity;
            }
            else if (theLight.type == 2 || theLight.type == 3)
            {
                lightDir = theLight.position - position;
                float distance = length(lightDir);
                lightDir /= distance;

                float atten = theLight.range / (0.5 * distance * distance + 2. * distance + 1.);

                if (theLight.type == 3)
                {
                    float rio = dot(-theLight.direction, lightDir);
                    float factor = 1.;
                    if (rio <= spotLightCosHalfPhi) factor = 0.;
                    else if (rio > spotLightCosHalfTheta) factor = 1.;
                    else
                    {
                        factor = (rio - spotLightCosHalfPhi) / (spotLightCosHalfTheta - spotLightCosHalfPhi);
                        factor = clamp(pow(factor, spotLightFalloff), 0., 1.);
                    }
                    intensity *= factor;
                }

                lightColor = theLight.color * (intensity * atten);
            }

            lightColor *= calcShadow(position, lightDir);
        }

        vec3 calcColor(int material, vec3 position, vec3 normal, vec3 color)
        {
            if (material == 0) return black;

            vec3 lightDir;
            vec3 lightColor;
            calcLight(lightDir, lightColor, position);

            if (material == 1)
            {
                float tmp = abs(floor(position.x * 1.) + floor(position.z * 1.));
                return ((mod(tmp, 2.) < 1.) ? black : white) * lightColor;
            }
            else if (material == 2)
            {
                vec2 texCoord = vec2(position.x * 0.2, position.z * 0.2);
                return texture2D(iChannel0, texCoord).xyz * lightColor;
            }
            else if (material == 10)
            {
                return color * (max(0.0, dot(normal, lightDir)) * .8 + .2) * lightColor;
            }
            else if (material == 11)
            {
                float lambertian = max(0.0, dot(normal, lightDir));
                float specular = 0.0;
                if (lambertian > 0.0)
                {
                    vec3 reflectDir = normalize(normal * dot(normal, lightDir) * 2. - lightDir);
                    float specAngle = max(dot(reflectDir, normalize(theEye - position)), 0.);
                    specular = pow(specAngle, shininess);
                }
                
                return (color * lambertian + specColor * specular) * lightColor;
            }
            else if (material == 12)
            {
                float lambertian = max(0.0, dot(normal, lightDir));
                float specular = 0.0;
                if (lambertian > 0.0)
                {
                    vec3 halfDir = normalize(lightDir + normalize(theEye - position));
                    float specAngle = max(dot(halfDir, normal), 0.);
                    specular = pow(specAngle, shininess * 4.);
                }
                
                return (color * lambertian + specColor * specular) * lightColor;
            }
            return black;
        }

        vec3 rayTrace(Ray ray)
        {
            vec3 color = black;
            float reflectiveness = 1.0;
            for (int i=0; i<MAX_REFLECT; ++i)
            {
                IntersectionResult ret = intersectScene(ray);
                if (ret.distance >= 0.)
                {
                    vec3 sampleColor = calcColor(ret.material, ret.position, ret.normal, ret.color);
                    color += sampleColor * (1. - ret.reflectiveness) * reflectiveness;
                    reflectiveness *= ret.reflectiveness;

                    if (reflectiveness > 0.)
                    {
                        vec3 reflectDir = normalize(ret.normal * (-2. * dot(ret.normal, ray.direction)) + ray.direction);
                        ray = Ray(ret.position, reflectDir);
                    }
                    else break;
                }
                else break;
            }
            return color;
        }

        void mainImage( out vec4 fragColor, in vec2 fragCoord ) {

            float posPixelX = fragCoord.x / iResolution.x * 2.0 - 1.0;
            float posPixelY = fragCoord.y / iResolution.y * 2.0 - 1.0;
            float radio = iResolution.x / iResolution.y;

            vec3 rayDir = normalize(vec3(posPixelX * radio, posPixelY, 0) - theEye);
            Ray theRay = Ray(theEye, rayDir);

            fragColor.xyz = rayTrace(theRay);
        }

        void main()
        {
            mainImage(gl_FragColor, gl_FragCoord.xy);
        }
#endif
        ENDGLSL
        }
    }
}
