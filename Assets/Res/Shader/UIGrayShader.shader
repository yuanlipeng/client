// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UI/Gray"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white"{}
		_SubTex("Texture", 2D) = "white"{}
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}

		

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Back

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;

			struct vertextData 
			{
				float4 vertex: POSITION;
				float2 uv:TEXCOORD0;
			};

			struct fragData
			{
				float4 vertex:SV_POSITION;
				float2 uv:TEXCOORD0;
			};

			fragData vert(vertextData data)
			{
				fragData o;
				o.vertex = UnityObjectToClipPos(data.vertex);
				o.uv = data.uv;
				return o;
			}

			fixed4 frag(fragData data): SV_Target
			{
				fixed4 col = tex2D(_MainTex, data.uv);
				/*float grey = dot(col.rgb, fixed3(0.22, 0.707, 0.071));
				return half4(grey, grey, grey, col.a);*/
				return col;
			}
			
			ENDCG
		}

		Pass
		{
				Blend SrcAlpha OneMinusSrcAlpha
				Cull Front

				CGPROGRAM

				#include "UnityCG.cginc"
				#pragma vertex vert
				#pragma fragment frag

				sampler2D _SubTex;

				struct vertextData
				{
					float4 vertex: POSITION;
					float2 uv:TEXCOORD0;
				};

				struct fragData
				{
					float4 vertex:SV_POSITION;
					float2 uv:TEXCOORD0;
				};

				fragData vert(vertextData data)
				{
					fragData o;
					o.vertex = UnityObjectToClipPos(data.vertex);
					o.uv = data.uv;
					return o;
				}

				fixed4 frag(fragData data) : SV_Target
				{
					fixed4 col = tex2D(_SubTex, data.uv);
				/*float grey = dot(col.rgb, fixed3(0.22, 0.707, 0.071));
				return half4(grey, grey, grey, col.a);*/
					return col;
				}

			ENDCG
		}
	}
}