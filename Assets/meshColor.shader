Shader "Custom/MeshColor"
{
    Properties
    {
        _maxHeight("Max height", Float) = 5
        _hue("Hue", Range(0.0, 1.0)) = 0.8
        _saturation("Saturation", Range(0.0, 1.0)) = 0.9
        _value("Value (Brightness)", Range(0.0, 1.0)) = 0.9
        _ambient("Ambient", Range(0.0, 1.0)) = 0.2
         _diffuse("Diffuse", Range(0.0, 1.0)) = 1.0
         _specular("Specular", Range(0.0, 1.0)) = 0.2
         _specularPower("Specular power", Int) = 10
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "LightMode" = "ForwardBase" }
        Cull off
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            float _hue, _saturation, _value;
            float _maxHeight, _diffuse, _specular, _ambient;
            int _specularPower;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 height : TEXCOORD0; 
                float3 normal : NORMAL;
                float4 vertex : SV_POSITION;
                float3 lightDir : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };

            float3 HSVToRGB(float3 c)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
            }
            

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.height = mul(unity_ObjectToWorld, v.vertex).yy;
                o.normal = UnityObjectToWorldNormal(v.normal);
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
                o.viewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos.xyz);
                return o;
                
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 materialColor = HSVToRGB(float3(i.height.y / _maxHeight * _hue, _saturation, _value));
                // ambient color
                float3 ambient = materialColor * _ambient;
                // diffuse color
                float3 normal = normalize(i.normal);
                float3 lightDir = normalize(i.lightDir);
                float3 diffuse = materialColor * max(0,dot(normal, lightDir)) * _diffuse * _LightColor0.rgb;
                // specular color
                float3 reflection = reflect(-lightDir, normal);
                float3 specular = _LightColor0.rgb * pow(max(0,dot(normalize(i.viewDir), reflection)), _specularPower)* _specular;
                return fixed4((ambient + diffuse + specular), 1.0);
            }
            ENDCG
        }
    }
}
