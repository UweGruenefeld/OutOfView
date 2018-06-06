/*
* MIT License
*
* Copyright (c) 2018 Uwe Gruenefeld, Dag Ennenga
* University of Oldenburg (GERMANY)
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*/
Shader "Custom/Line"
{
	Properties
	{
		_Dotted("Dotted", float) = 0
		_Repeat("Repeat", float) = 5
		_Spacing("Spacing", float) = 0.5
	}
	
	SubShader
	{
		Tags
		{
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
		}

		LOD 100

		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			float _Dotted;
			float _Repeat;
			float _Spacing;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 pos : TEXCOORD1;
				float4 obj : TEXCOORD2;
				fixed4 color : COLOR0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = v.vertex;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.obj = mul(UNITY_MATRIX_MV, v.vertex);
				o.uv = v.uv;
				o.uv.x = o.uv.x * _Repeat * (1.0f + _Spacing);
				o.color = v.color;
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target
			{
				i.uv.x = fmod(i.uv.x, 1.0f + _Spacing);
				
				float r = length(i.uv - float2(1.0f + _Spacing, 1.0f) * 0.5f) * 2.0f;

				fixed4 color = i.color;
				color.a *= saturate((0.99f - r) * 100.0f);

				if (!_Dotted)
					color.a = 1;

				// Fix: Distance between visualization and camera is hard coded
				// switch to use object space instead
				if (i.obj.z + 10.12 < 0)
					color.a = 0;

				return color;
			}
			ENDCG
		}
	}
}
