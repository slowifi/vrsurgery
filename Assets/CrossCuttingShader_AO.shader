﻿// Upgrade NOTE: replaced 'glstate.matrix.invtrans.modelview[0]' with 'UNITY_MATRIX_IT_MV'
// Upgrade NOTE: replaced 'glstate.matrix.mvp' with 'UNITY_MATRIX_MVP'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Fast Edition - Single Pass

Shader "CrossCuttingShader_AO"
{
	Properties
	{
	  _MainTex("Texture", 2D) = "red" {}
	  _section("Section plane (x angle, y angle, z angle)", vector) = (0,0,0)
	  _distance("Section plane (w displacement)", float) = 0
	  _Color("Section Color", Color) = (1,1,1,0)
	  _clipping("clip", int) = 0
	}

		SubShader
	  {
		Tags { "RenderType" = "Opaque" }
		Cull Off

		CGPROGRAM
		#pragma surface surf Lambert

		struct Input
		{
			float2 uv_MainTex;
			float3 worldPos;
			float3 viewDir;
			float3 worldNormal;
		};

		sampler2D _MainTex;
		sampler2D _BumpMap;
		float3 _section;
		float _distance;
		fixed4 _Color;
		int _clipping;

		void surf(Input IN, inout SurfaceOutput o)
		{
		  float toClip = -_section.x * IN.worldPos.x +
						 -_section.y * IN.worldPos.y +
						 -_section.z * IN.worldPos.z +
						 _distance;
		  if (_clipping == 1)
		  clip(toClip);

		  float fd = dot(IN.viewDir, IN.worldNormal);

		  if (fd.x > 0)
		  {
			  o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
			  return;
		  }

		  o.Emission = _Color;
		}
		ENDCG
	  }
		  Fallback "Diffuse"
}