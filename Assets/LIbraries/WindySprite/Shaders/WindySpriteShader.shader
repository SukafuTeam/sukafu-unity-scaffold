Shader "Custom/WindySprite"{
	Properties{
		_MainTex("Texture",2D)="white"{}
		_Color("Tint",Color)=(1,1,1,1)
		_WindForce("Wind Force",float)=1
		_ShakeForce("Shake Force",float)=0.1
		_ShakeSpeed("Shake Speed",float)=1
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

			struct appdata{
				float4 pos:POSITION;
				float2 uv:TEXCOORD0;
			};

			struct v2f{
				float4 pos:SV_POSITION;
				float2 uv:TEXCOORD0;
			};

			sampler2D _MainTex;
			float _WindForce;
			float _ShakeForce;
			float _ShakeSpeed;

			v2f vert(appdata v){
				v2f o;
				float3 unit=UnityObjectToClipPos(float3(1,1,0))-UnityObjectToClipPos(float3(0,0,0));
				o.pos=UnityObjectToClipPos(v.pos);
				
				float multiplier=v.uv.y;
				
				float bend = v.pos.y * _WindForce*unit.x * multiplier; 
				float shake = sin((v.pos.y)-(_Time.a*_ShakeSpeed))*(_ShakeForce*unit.x*multiplier);
				
                o.pos.x += bend + shake;

				o.uv=v.uv;
				return o;
			}

			fixed4 _Color;
			
			fixed4 frag (v2f i):SV_Target{
				fixed4 col=tex2D(_MainTex,i.uv)*_Color;
				return col;
			}
			ENDCG
		}
	}
}