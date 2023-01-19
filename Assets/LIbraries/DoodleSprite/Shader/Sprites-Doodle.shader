Shader "Sprites/Doodle"{
	Properties{
		_MainTex("Texture",2D)="white"{}
		_Color("Tint",Color)=(1,1,1,1)
		_NoiseScale ("Noise Scale", Range(0.0, 0.08)) = 0.01
        _NoiseSnap ("Noise Snap", Range(0.001, 0.01)) = 0.005
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
	}
	SubShader
	{
		Tags{
			"Queue"="Transparent" 
			"IgnoreProjector"="True"
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
		}
		ZWrite Off 
		Lighting Off 
		Cull Off 
		Fog { Mode Off } 
		Blend One Zero
		Pass{
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM

			#pragma exclude_renderers xbox360 ps3 flash
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct Input
	        {
	            float2 uv_MainTex;
	            fixed4 color;
	        };

			sampler2D _MainTex;
			float4 _MainTex_ST;

			fixed4 _Color;
			float _NoiseScale;
			float _NoiseSnap;

			float rand(float n)
	        {
	            return frac(sin(n) * 43758.5453123);
	        }

	        float snap (float x, float snap)
	        {
	            return snap * round(x / snap);
	        }

			struct appdata{
			    float4 vertex : POSITION;
			    float2 uv : TEXCOORD0;
			    fixed4 color : COLOR;
			};

			struct v2f{
			    float4 position : SV_POSITION;
			    float2 uv : TEXCOORD0;
			    fixed4 color : COLOR;
			};

			v2f vert(appdata v){
			    v2f o;
			    o.position = UnityObjectToClipPos(v.vertex);
				float2 noise;

				const float time = snap(_Time, _NoiseSnap);
				
	            noise.x = rand(v.vertex.x + time);
	            noise.y = rand(v.vertex.y + time);
	            noise *= _NoiseScale;
	            o.position.xy += noise;
			    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
			    o.color = v.color;
			    return o;
			}

			fixed4 frag(v2f i) : SV_TARGET{
			    fixed4 col = tex2D(_MainTex, i.uv);
			    col *= _Color;
			    col *= i.color;
			    return col;
			}
			
			ENDCG
		}
	}
}