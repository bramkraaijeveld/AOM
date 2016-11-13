Shader "AOM/Terrain"{
	Properties{
		_Color("Color", Color) = (0.5, 0.5, 0.5, 1)
	}
	SubShader{
		Tags{ "RenderType" = "Opaque" }

		CGPROGRAM
		#pragma surface surf Standard addshadow fullforwardshadows vertex:vert nolightmap nometa
		#pragma target 3.0

		// Script Variables
		sampler2D terrain;
		float height;
		float size;

		// Property Variables
		fixed4 _Color;

		// Structs and Functions
		struct Input { 
			float3 normal;
		};

		float3 normal(float2 coord, float delta) {
			float h00 = tex2Dlod(terrain, float4(coord.x, coord.y, 0, 0)).r * height;
			float h01 = tex2Dlod(terrain, float4(coord.x, coord.y + delta, 0, 0)).r * height;
			float h10 = tex2Dlod(terrain, float4(coord.x + delta, coord.y, 0, 0)).r * height;
			return cross(float3(0, h01 - h00, delta), float3(delta, h10 - h00, 0));
		}

		void vert(inout appdata_full v, out Input o) {
			v.vertex.y = tex2Dlod(terrain, float4(v.texcoord.xy, 0, 0)).r * height * size;
			v.normal = normal(v.texcoord1.xy, 0.0002);

			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.normal = v.normal;
		}

		void surf(Input IN, inout SurfaceOutputStandard o) {
			o.Albedo = _Color;
		}

		ENDCG
	}
}