// Shader created with Shader Forge v1.16 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.16;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.1280277,fgcg:0.1953466,fgcb:0.2352941,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:7810,x:33339,y:32022,varname:node_7810,prsc:2|normal-591-RGB,custl-4674-OUT;n:type:ShaderForge.SFN_Dot,id:8500,x:31913,y:31731,varname:node_8500,prsc:2,dt:1|A-5243-OUT,B-3862-OUT;n:type:ShaderForge.SFN_HalfVector,id:4266,x:31934,y:32259,varname:node_4266,prsc:2;n:type:ShaderForge.SFN_Dot,id:7768,x:32131,y:32269,varname:node_7768,prsc:2,dt:1|A-4266-OUT,B-3144-OUT;n:type:ShaderForge.SFN_Power,id:168,x:32410,y:32222,varname:node_168,prsc:2|VAL-7768-OUT,EXP-6053-OUT;n:type:ShaderForge.SFN_Vector1,id:6053,x:32241,y:32313,varname:node_6053,prsc:2,v1:100;n:type:ShaderForge.SFN_Add,id:1890,x:32914,y:32054,varname:node_1890,prsc:2|A-5844-OUT,B-7554-OUT,C-1022-OUT;n:type:ShaderForge.SFN_AmbientLight,id:3784,x:32118,y:31159,varname:node_3784,prsc:2;n:type:ShaderForge.SFN_Multiply,id:7554,x:32451,y:31732,varname:diff,prsc:2|A-3175-RGB,B-8500-OUT,C-5178-RGB,D-1792-OUT;n:type:ShaderForge.SFN_LightVector,id:3862,x:31649,y:31755,varname:node_3862,prsc:2;n:type:ShaderForge.SFN_Reflect,id:853,x:32104,y:31990,varname:node_853,prsc:2|A-3226-OUT,B-5953-OUT;n:type:ShaderForge.SFN_Dot,id:2556,x:32259,y:32034,varname:node_2556,prsc:2,dt:1|A-853-OUT,B-7750-OUT;n:type:ShaderForge.SFN_ViewVector,id:7750,x:32088,y:32124,varname:node_7750,prsc:2;n:type:ShaderForge.SFN_Negate,id:3226,x:31886,y:31916,varname:node_3226,prsc:2|IN-3862-OUT;n:type:ShaderForge.SFN_Power,id:6975,x:32410,y:32086,varname:node_6975,prsc:2|VAL-2556-OUT,EXP-4910-OUT;n:type:ShaderForge.SFN_Vector1,id:4910,x:32229,y:32183,varname:node_4910,prsc:2,v1:25;n:type:ShaderForge.SFN_LightColor,id:5178,x:32043,y:31808,varname:node_5178,prsc:2;n:type:ShaderForge.SFN_LightAttenuation,id:1792,x:32165,y:31865,varname:node_1792,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:591,x:33023,y:31802,ptovrint:False,ptlb:BumpMap,ptin:_BumpMap,varname:_BumpMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:76c4cf83a5d8a1e46947bb86e4340444,ntxv:2,isnm:False|UVIN-5538-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:7513,x:32116,y:32404,varname:node_7513,prsc:2,uv:0;n:type:ShaderForge.SFN_Tex2d,id:3175,x:32060,y:31542,ptovrint:False,ptlb:TexMap,ptin:_TexMap,varname:_TexMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:fb0583218571c2c41a220640a541938a,ntxv:0,isnm:False|UVIN-7551-UVOUT;n:type:ShaderForge.SFN_Cubemap,id:1727,x:32644,y:32421,ptovrint:False,ptlb:CubeMap,ptin:_CubeMap,varname:_CubeMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,cube:919af242229647f42b9fe2428ec9f049,pvfc:0|DIR-9248-OUT;n:type:ShaderForge.SFN_ViewReflectionVector,id:9248,x:32465,y:32421,varname:node_9248,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:9110,x:31652,y:32588,prsc:2,pt:True;n:type:ShaderForge.SFN_Vector1,id:1342,x:32619,y:32287,varname:node_1342,prsc:2,v1:0;n:type:ShaderForge.SFN_Dot,id:6946,x:31872,y:32698,varname:node_6946,prsc:2,dt:0|A-9110-OUT,B-518-OUT;n:type:ShaderForge.SFN_ViewVector,id:518,x:31652,y:32757,varname:node_518,prsc:2;n:type:ShaderForge.SFN_Clamp01,id:8132,x:32044,y:32698,varname:node_8132,prsc:2|IN-6946-OUT;n:type:ShaderForge.SFN_OneMinus,id:132,x:32211,y:32698,varname:node_132,prsc:2|IN-8132-OUT;n:type:ShaderForge.SFN_Power,id:7960,x:32421,y:32748,varname:reflection,prsc:2|VAL-132-OUT,EXP-4960-OUT;n:type:ShaderForge.SFN_Vector1,id:4960,x:32238,y:32831,varname:node_4960,prsc:2,v1:5;n:type:ShaderForge.SFN_Tex2d,id:3769,x:32293,y:32404,ptovrint:False,ptlb:SpecMap,ptin:_SpecMap,varname:_SpecMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:6fc41b185e725774285bfc37e5b386dc,ntxv:0,isnm:False|UVIN-7513-UVOUT;n:type:ShaderForge.SFN_Multiply,id:1022,x:32629,y:32072,varname:node_1022,prsc:2|A-168-OUT,B-3769-RGB,C-5178-RGB,D-1792-OUT;n:type:ShaderForge.SFN_Vector1,id:989,x:32644,y:32222,varname:node_989,prsc:2,v1:1;n:type:ShaderForge.SFN_Code,id:160,x:31892,y:31297,varname:shl,prsc:1,code:cgBlAHQAdQByAG4AIABTAGgAYQBkAGUAUwBIADkAKABoAGEAbABmADQAKABuAG8AcgBtAGEAbAAsACAAMQAuACkAKQA7AA==,output:2,fname:SHLight,width:308,height:132,input:6,input_1_label:normal|A-6161-OUT;n:type:ShaderForge.SFN_Lerp,id:4674,x:33074,y:32293,varname:node_4674,prsc:2|A-1890-OUT,B-1727-RGB,T-7960-OUT;n:type:ShaderForge.SFN_Multiply,id:5844,x:32353,y:31310,varname:node_5844,prsc:2|A-160-OUT,B-3175-RGB;n:type:ShaderForge.SFN_NormalVector,id:6161,x:31693,y:31295,prsc:2,pt:True;n:type:ShaderForge.SFN_TexCoord,id:7551,x:31873,y:31542,varname:node_7551,prsc:2,uv:0;n:type:ShaderForge.SFN_TexCoord,id:5538,x:32835,y:31802,varname:node_5538,prsc:2,uv:0;n:type:ShaderForge.SFN_NormalVector,id:5243,x:31592,y:31618,prsc:2,pt:True;n:type:ShaderForge.SFN_NormalVector,id:5953,x:31784,y:32029,prsc:2,pt:True;n:type:ShaderForge.SFN_NormalVector,id:3144,x:31775,y:32336,prsc:2,pt:True;proporder:3175-591-3769-1727;pass:END;sub:END;*/

Shader "Custom/sf_test" {
    Properties {
        _TexMap ("TexMap", 2D) = "white" {}
        _BumpMap ("BumpMap", 2D) = "black" {}
        _SpecMap ("SpecMap", 2D) = "white" {}
        _CubeMap ("CubeMap", Cube) = "_Skybox" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform sampler2D _TexMap; uniform float4 _TexMap_ST;
            uniform samplerCUBE _CubeMap;
            uniform sampler2D _SpecMap; uniform float4 _SpecMap_ST;
            float3 SHLight( half3 normal ){
            return ShadeSH9(half4(normal, 1.));
            }
            
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
                UNITY_FOG_COORDS(7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 _BumpMap_var = tex2D(_BumpMap,TRANSFORM_TEX(i.uv0, _BumpMap));
                float3 normalLocal = _BumpMap_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                half3 shl = SHLight( normalDirection );
                float4 _TexMap_var = tex2D(_TexMap,TRANSFORM_TEX(i.uv0, _TexMap));
                float3 diff = (_TexMap_var.rgb*max(0,dot(normalDirection,lightDirection))*_LightColor0.rgb*attenuation);
                float4 _SpecMap_var = tex2D(_SpecMap,TRANSFORM_TEX(i.uv0, _SpecMap));
                float4 _CubeMap_var = texCUBE(_CubeMap,viewReflectDirection);
                float reflection = pow((1.0 - saturate(dot(normalDirection,viewDirection))),5.0);
                float3 finalColor = lerp(((shl*_TexMap_var.rgb)+diff+(pow(max(0,dot(halfDirection,normalDirection)),100.0)*_SpecMap_var.rgb*_LightColor0.rgb*attenuation)),_CubeMap_var.rgb,reflection);
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
