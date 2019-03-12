// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UI/Procedural UI Image"
{
	Properties
	{
		[PerRendererData]_MainTex ("Base (RGB)", 2D) = "white" {}
		// required for UI.Mask
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        
        _ColorMask ("Color Mask", Float) = 15
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
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]
        
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
			#include "UnityCG.cginc"
			#include "UnityUI.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float2 uv2 : TEXCOORD2;
				float2 uv3 : TEXCOORD3;
			};

			struct v2f
			{
				float4 vertex   : POSITION;
				fixed4 color    : COLOR;
				float4 worldPosition : TEXCOORD0;
				float4 radius : TEXCOORD1;
				float2 uv0  : TEXCOORD2;
				float2 wh : TEXCOORD3;
				float lineWeight : TEXCOORD4;
				float pixelWorldScale : TEXCOORD5;
			};
			
			fixed4 _TextureSampleAdd;
	
			bool _UseClipRect;
			float4 _ClipRect;

			bool _UseAlphaClip;
			
			sampler2D _MainTex;

			float2 decode2(float value) {
				float2 kEncodeMul = float2(1.0, 65535.0f);
				float kEncodeBit = 1.0 / 65535.0f;
				float2 enc = kEncodeMul * value;
				enc = frac(enc);
				enc.x -= enc.y * kEncodeBit;
				return enc;
			}

			
			v2f vert(appdata_t IN){
				v2f OUT;
				
				OUT.worldPosition = IN.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
				
				OUT.wh = IN.uv1;
				OUT.uv0 = IN.uv0;

				float minside = min(OUT.wh.x, OUT.wh.y);

				OUT.lineWeight = IN.uv3.x*minside/2;

				OUT.radius = float4(decode2(IN.uv2.x),decode2(IN.uv2.y))*minside;

				OUT.pixelWorldScale = clamp(IN.uv3.y,1/2048,2048);
				
				OUT.color = IN.color*(1+_TextureSampleAdd);

				#ifdef UNITY_HALF_TEXEL_OFFSET
				OUT.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
				#endif
				return OUT;
			}
			
			half visible(float2 pos,float4 r,float2 wh){
				half4 p = half4(pos,wh.x-pos.x,wh.y-pos.y);
				half v = min(min(min(p.x,p.y),p.z),p.w);
				bool4 b = bool4(all(p.xw<r[0]),all(p.zw<r[1]),all(p.zy<r[2]),all(p.xy<r[3]));
				half4 vis = r-half4(length(p.xw-r[0]),length(p.zw-r[1]),length(p.zy-r[2]),length(p.xy-r[3]));
				half4 foo = min(b*max(vis,0),v)+(1-b)*v;
				v = any(b)*min(min(min(foo.x,foo.y),foo.z),foo.w)+v*(1-any(b));
				return v;
			}

			fixed4 frag (v2f IN) : SV_Target
			{
				half4 color = IN.color * tex2D(_MainTex,IN.uv0);
				if (_UseClipRect){
					color *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
				}
				if (_UseAlphaClip){
					clip (color.a - 0.001);
				}
				half v = visible(IN.uv0*IN.wh,IN.radius,IN.wh);
				
				float l = (IN.lineWeight+1/IN.pixelWorldScale)/2;
				color.a *= saturate((l-distance(v,l))*IN.pixelWorldScale);
				
				if(color.a <= 0){
					discard;
				}
				return color;
			}
			ENDCG
		}
	}
}

