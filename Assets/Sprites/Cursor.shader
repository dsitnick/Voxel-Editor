Shader "Unlit/Cursor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color("Tint Color", Color) = (1,1,1,1)
		_Shade("Shade Amount", Range(0, 1)) = 0.5
		_Power("Power", Range(0, 10)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };
     
            struct v2f {
                float4 pos : SV_POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };
			
            sampler2D _MainTex;
            float4 _MainTex_ST;
     
            v2f vert(appdata v) {
                v2f o;
     
                o.pos = UnityObjectToClipPos(v.vertex);
             
                o.normal = normalize(v.normal);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
     
                return o;
            }
			
			float _Shade;
			float _Power;
			float4 _Color;
     
            half4 frag(v2f i) : COLOR
            {
                float3 normalDir = i.normal;

                float3 viewDir = normalize(mul(UNITY_MATRIX_V, float4(0, 0, 1, 0)).xyz);

				float shade = 1 - pow(saturate(dot(viewDir, normalDir)), _Power);
				shade = 1 - (shade * _Shade);

				float4 result = tex2D(_MainTex, i.uv) * _Color;
				return float4(result.rgb * shade, result.a);
            }
 
            ENDCG
        }
    }
}
