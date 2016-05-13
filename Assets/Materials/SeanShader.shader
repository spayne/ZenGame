Shader "Custom/SeanShader" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_EmptyColor("EmptyColor", Color) = (1,1,1,1)
		_Fill("Fill", Range(0,1)) = 0.0
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Threshy("Threshy", Range(0,1)) = 0.0
	}
		subshader{

			Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float _Threshy;

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
			if (i.uv.y > 0.0 && i.uv.y < _Threshy //&&
				// i.uv.x > 0.0 && i.uv.x < _Threshy
				)
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

