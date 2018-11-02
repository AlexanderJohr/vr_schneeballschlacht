Shader "MyShaders/Rim2" {
	Properties {
		_RimColor ("Rim Color", Color) = (0,0,0,0)
		_RimColor2 ("Rim Color2", Color) = (0,0,0,0)
		_RimColor3 ("Rim Color3", Color) = (0,0,0,0)
		
		_RimPower ("Rim Power", Range(0,10)) = 0.0
		_RimPower2 ("Rim Power 2", Range(0,10)) = 0.0
		_RimPower3 ("Rim Power 3", Range(0,10)) = 0.0
	}
	SubShader {
		CGPROGRAM
		
		#pragma surface surf Lambert 



		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
		};

		half _RimPower;
		half _RimPower2;
		half _RimPower3;
		float4 _RimColor;
		float4 _RimColor2;
		float4 _RimColor3;


		void surf (Input IN, inout SurfaceOutput o) {
			half rim = 1- saturate(dot(normalize(IN.viewDir), o.Normal));
			
			o.Emission= rim > 0.7 ? _RimColor.rgb * pow(rim,_RimPower) : rim > 0.4 ? _RimColor2.rgb * pow(rim,_RimPower2) : _RimColor3.rgb * pow(rim,_RimPower3);
			
		}
		ENDCG
	}
	FallBack "Diffuse"
}
