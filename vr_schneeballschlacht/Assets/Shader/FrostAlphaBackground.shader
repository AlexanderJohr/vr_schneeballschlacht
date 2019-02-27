Shader "vr_schneeballschlacht/FrostAlphaBackground" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	}
		SubShader{
			Cull Off
		 Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
	    Blend SrcAlpha OneMinusSrcAlpha

	    ZWrite Off
		Offset -10000,0

		CGPROGRAM

		#pragma surface surf Lambert  alpha:fade

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color;

		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 c = _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}

		ENDCG
	}
	FallBack "Diffuse"
}
