Shader "Custom/SeanShader" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_EmptyColor("EmptyColor", Color) = (1,1,1,1)
		_Fill("Fill", Range(0,1)) = 0.0
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_FillEW("FillEW", Range(0,1)) = 0.0
		_FillWE("FillWE", Range(0,1)) = 0.0
		_FillNS("FillNS", Range(0,1)) = 0.0
		_FillSN("FillSN", Range(0,1)) = 0.0
	}
		subshader{

			Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			
			float _FillEW;
			float _FillWE;
			float _FillNS;
			float _FillSN;

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
			};


			v2f vert(
				float4 vertex : POSITION, // vertex position input
				float2 uv : TEXCOORD0 // first texture coordinate input
				)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, vertex);
				o.uv = uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 c = 0;

				// these tests work, but are different than you'd expect.  I'm
				// going to assume the uvs are just upside down
				bool x_test = (i.uv.x < _FillWE || i.uv.x >(1.0 - _FillEW));
				bool y_test = (i.uv.y < _FillSN || i.uv.y >(1.0 - _FillNS));
				
			if (y_test || x_test)
			{
				c.rgb = fixed4(0.0, 1.0, 0.0, 1.0); // green
			}
			else
			{
				c.rgb = fixed4(1.0, 1.0, 1.0, 1.0);
			}

				return c;
			}





			ENDCG
				}
		}
}

