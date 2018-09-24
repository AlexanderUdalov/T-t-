// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/VertexColorTransparency"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags {"Queue"="Transparent" "RenderType"="Transparent" }
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex wfiVertCol
			#pragma fragment passThrough

			#include "UnityCG.cginc"

			struct VertOut
			 {
				 float4 position : POSITION;
				 float4 color : COLOR;
			 };

			 struct VertIn
			 {
				 float4 vertex : POSITION;
				 float4 color : COLOR;
			 };

			 VertOut wfiVertCol(VertIn input)
			 {
				 VertOut output;
				 output.position = UnityObjectToClipPos(input.vertex);
				 output.color = input.color;
				 return output;
			 }

			 struct FragOut
			 {
				 float4 color : COLOR;
			 };

			 float4 _Color;

			 FragOut passThrough(float4 color : COLOR)
			 {
				 FragOut output;
				 output.color = float4(color.r, color.g, color.b, color.a);
				 return output;
			 }
			 ENDCG
		}
	}
}
