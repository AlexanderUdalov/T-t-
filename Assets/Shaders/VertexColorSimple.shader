Shader "Custom/VertexColorSimple" {
	SubShader{
		Tags { 
		"Queue" = "Transparent" 
		"IgnoreProjector" = "True" 
		"RenderType" = "Transparent" 
		}

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				fixed4 color : COLOR;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;
			};

			v2f vert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex); 
				//half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				//half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				o.color = v.color;//nl  * v.color;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target { return i.color; }
			ENDCG
		}
	}
}