Shader "Unlit/Rim"
{
    Properties
    {
        _InnerColor("Inner Color", Color) = (1,1,1,1)
		_OuterColor("Outer Color", Color) = (1,1,1,1)
		_Power("Lerp Power", Range(0, 10)) = 1
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
                fixed4 color : COLOR;
                float3 normal : NORMAL;
            };
     
            struct v2f {
                float4 pos : SV_POSITION;
                float3 normal : NORMAL;
                float4 posWorld : TEXCOORD0;
                fixed4 color : COLOR;
            };
     
            v2f vert(appdata v) {
                v2f o;
     
                o.pos = UnityObjectToClipPos(v.vertex);
             
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.normal = normalize( mul ( float4(v.normal, 0.0), unity_WorldToObject).xyz);
     
                o.color = v.color;
     
                return o;
            }

			float4 _InnerColor, _OuterColor;
			float _Power;
     
            half4 frag(v2f i) : COLOR
            {
                float3 normalDir = i.normal;
                float3 viewDir = normalize( _WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                 
                float rim = 1 - saturate ( dot(viewDir, normalDir) );
				rim = pow(rim, _Power);

				return float4(i.color.rgb,1) * lerp(_InnerColor, _OuterColor, rim);
            }
 
            ENDCG
        }
    }
}
