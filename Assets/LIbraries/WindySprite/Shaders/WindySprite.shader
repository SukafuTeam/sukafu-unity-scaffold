Shader "Sprites/Wind"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[PerRendererData]_WindForce("Wind Force",float)=1
		[PerRendererData]_ShakeForce("Shake Force",float)=0.1
		[PerRendererData]_ShakeOffset("Shake Offset",float)=0
		[PerRendererData]_BottomOffset("Bottom Offset",float)=0
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
			float _WindForce;
			float _ShakeForce;
			float _ShakeSpeed;
			float _ShakeOffset;
			float _BottomOffset;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				float3 unit=UnityObjectToClipPos(float3(1,1,0))-UnityObjectToClipPos(float3(0,0,0));
				OUT.vertex=UnityObjectToClipPos(IN.vertex);
				float multiplier = IN.texcoord.y;
				
				float bend = IN.vertex.y * _WindForce * unit.x * multiplier;
				float shakeSin = sin((IN.vertex.y) - (_Time.a * _ShakeSpeed)) + _ShakeOffset;
				float shake = shakeSin * (_ShakeForce * unit.x * multiplier);
				
				if(IN.vertex.y > _BottomOffset) {
				    OUT.vertex.x += bend * shake;
				}
                
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;

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
				fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}