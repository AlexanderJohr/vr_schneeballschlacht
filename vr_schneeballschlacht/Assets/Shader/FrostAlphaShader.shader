Shader "vr_schneeballschlacht/FrostAlphaShader" {
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
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			c.a = c.a * _Color.a;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}

		ENDCG
	}
	FallBack "Diffuse"
}
