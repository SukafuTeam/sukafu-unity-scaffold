Shader "Sprites/Blur"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[PerRendererData] _Size ("Size", float) = 0.5
		[PerRendererData] _Strenght ("Strenght", float) = 0.5
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;
			float _Size;
			float _Strenght;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
				if (_AlphaSplitEnabled)
					color.a = tex2D (_AlphaTex, uv).r;
#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
                half4 texcol = half4(0.0, 0.0, 0.0, 0.0);
                if(SampleSpriteTexture(IN.texcoord).a < 0.5) {
                    return texcol;
                }
                float remaining = 1.0;
                float coef = _Strenght;
                float fI = 0;
                float size = _Size * 0.01; 
                for (int j = 0; j < 20; j++) {
                    fI++;
                    coef *= 0.32;
                    texcol += SampleSpriteTexture(float2(IN.texcoord.x, IN.texcoord.y - fI * size)) * coef;
                    texcol += SampleSpriteTexture(float2(IN.texcoord.x - fI * size, IN.texcoord.y)) * coef;
                    texcol += SampleSpriteTexture(float2(IN.texcoord.x + fI * size, IN.texcoord.y)) * coef;
                    texcol += SampleSpriteTexture(float2(IN.texcoord.x, IN.texcoord.y + fI * size)) * coef;
                    
                    remaining -= 4 * coef;
                }
                texcol += tex2D(_MainTex, IN.texcoord) * remaining;
                return texcol * IN.color;
			}
		ENDCG
		}
	}
}