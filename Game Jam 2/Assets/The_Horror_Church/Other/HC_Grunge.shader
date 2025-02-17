// Made with Amplify Shader Editor v1.9.6.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AE/Grunge"
{
	Properties
	{
		_Grunge("Grunge", 2D) = "white" {}
		_Base_Color("Base_Color", 2D) = "white" {}
		_Metallic_Smoothness("Metallic_Smoothness", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_AmbientOcclusion("Ambient Occlusion", 2D) = "white" {}
		_Tiling_Main_Texture("Tiling_Main_Texture", Vector) = (1,1,0,0)
		_Tint("Tint", Color) = (1,1,1,0)
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.2765529
		_Normal_Scale("Normal_Scale", Range( 0 , 15)) = 0
		[Toggle(_GRUNGE_ON_ON)] _Grunge_ON("Grunge_ON", Float) = 1
		[Toggle(_GRUNGE_TEXTURE_ON)] _Grunge_Texture("Grunge_Texture", Float) = 0
		_Base_Color_Grtunge("Base_Color_Grtunge", 2D) = "white" {}
		_GrungeScale("GrungeScale", Range( 1 , 64)) = 1
		[KeywordEnum(R,G,B)] _GrungeChannel("GrungeChannel", Float) = 1
		_Grunge_Opacity("Grunge_Opacity", Range( 0 , 1)) = 0.6238509
		_Grunge_Texture_Opacity("Grunge_Texture_Opacity", Range( 0 , 1)) = 1
		_Grunge_Max("Grunge_Max", Range( 0 , 1)) = 0.3000374
		_Grunge_Min("Grunge_Min", Range( 0 , 1)) = 0.06147235
		_Grunge_Tint("Grunge_Tint", Color) = (0.5566038,0.5566038,0.5566038,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _GRUNGE_ON_ON
		#pragma shader_feature_local _GRUNGE_TEXTURE_ON
		#pragma shader_feature_local _GRUNGECHANNEL_R _GRUNGECHANNEL_G _GRUNGECHANNEL_B
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _Normal;
		uniform float2 _Tiling_Main_Texture;
		uniform float _Normal_Scale;
		uniform float4 _Tint;
		uniform sampler2D _Base_Color;
		uniform float4 _Grunge_Tint;
		uniform sampler2D _Base_Color_Grtunge;
		uniform float _Grunge_Texture_Opacity;
		uniform float _Grunge_Opacity;
		uniform sampler2D _Grunge;
		uniform float _GrungeScale;
		uniform float _Grunge_Min;
		uniform float _Grunge_Max;
		uniform float _Smoothness;
		uniform sampler2D _Metallic_Smoothness;
		uniform sampler2D _AmbientOcclusion;


inline float4 TriplanarSampling3( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
{
	float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
	projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
	float3 nsign = sign( worldNormal );
	half4 xNorm; half4 yNorm; half4 zNorm;
	xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
	yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
	zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
	return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TexCoord48 = i.uv_texcoord * _Tiling_Main_Texture;
			o.Normal = UnpackScaleNormal( tex2D( _Normal, uv_TexCoord48 ), _Normal_Scale );
			float4 temp_output_18_0 = ( _Tint * tex2D( _Base_Color, uv_TexCoord48 ) );
			float4 temp_cast_0 = (1.0).xxxx;
			float temp_output_55_0 = ( 1.0 - _Grunge_Texture_Opacity );
			float4 temp_cast_1 = (temp_output_55_0).xxxx;
			float4 lerpResult53 = lerp( tex2D( _Base_Color_Grtunge, uv_TexCoord48 ) , temp_cast_1 , temp_output_55_0);
			#ifdef _GRUNGE_TEXTURE_ON
				float4 staticSwitch49 = lerpResult53;
			#else
				float4 staticSwitch49 = _Grunge_Tint;
			#endif
			float2 temp_cast_2 = (( 2.0 / _GrungeScale )).xx;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float4 triplanar3 = TriplanarSampling3( _Grunge, ase_worldPos, ase_worldNormal, 1.0, temp_cast_2, 1.0, 0 );
			#if defined( _GRUNGECHANNEL_R )
				float staticSwitch20 = triplanar3.x;
			#elif defined( _GRUNGECHANNEL_G )
				float staticSwitch20 = triplanar3.g;
			#elif defined( _GRUNGECHANNEL_B )
				float staticSwitch20 = triplanar3.b;
			#else
				float staticSwitch20 = triplanar3.g;
			#endif
			float clampResult24 = clamp( (0.0 + (staticSwitch20 - _Grunge_Min) * (1.0 - 0.0) / (_Grunge_Max - _Grunge_Min)) , 0.0 , 1.0 );
			float4 lerpResult26 = lerp( temp_cast_0 , staticSwitch49 , ( _Grunge_Opacity * clampResult24 ));
			#ifdef _GRUNGE_ON_ON
				float4 staticSwitch28 = ( temp_output_18_0 * lerpResult26 );
			#else
				float4 staticSwitch28 = temp_output_18_0;
			#endif
			o.Albedo = staticSwitch28.rgb;
			o.Smoothness = ( _Smoothness * tex2D( _Metallic_Smoothness, uv_TexCoord48 ).a );
			o.Occlusion = tex2D( _AmbientOcclusion, uv_TexCoord48 ).r;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19603
Node;AmplifyShaderEditor.RangedFloatNode;10;-1379.331,-118.185;Inherit;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-1503.009,76.38765;Inherit;False;Property;_GrungeScale;GrungeScale;12;0;Create;True;0;0;0;False;0;False;1;8.2;1;64;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;2;-1410.406,-525.2164;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleDivideOpNode;8;-1145.977,-204.9319;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;4;-1472,-304;Inherit;True;Property;_Grunge;Grunge;0;0;Create;True;0;0;0;False;0;False;455561dd6a13a8c4988acefe2b84ec31;455561dd6a13a8c4988acefe2b84ec31;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TriplanarNode;3;-979.209,-421.1032;Inherit;True;Spherical;World;False;Top Texture 0;_TopTexture0;white;-1;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;41;-1520,256;Inherit;False;Property;_Tiling_Main_Texture;Tiling_Main_Texture;5;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;21;-627.4119,-712.2289;Inherit;False;Property;_Grunge_Min;Grunge_Min;17;0;Create;True;0;0;0;False;0;False;0.06147235;0.06147235;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-625.3651,-607.8414;Inherit;False;Property;_Grunge_Max;Grunge_Max;16;0;Create;True;0;0;0;False;0;False;0.3000374;0.253;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;20;-544,-384;Inherit;False;Property;_GrungeChannel;GrungeChannel;13;0;Create;True;0;0;0;False;0;False;0;1;1;True;;KeywordEnum;3;R;G;B;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;48;-1232,224;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;54;-512,-1168;Inherit;False;Property;_Grunge_Texture_Opacity;Grunge_Texture_Opacity;15;0;Create;True;0;0;0;False;0;False;1;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;23;-224,-416;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;50;-432,-1520;Inherit;True;Property;_Base_Color_Grtunge;Base_Color_Grtunge;11;0;Create;True;0;0;0;False;0;False;-1;273db00228b23ff4881ea95a862346d2;d72fa1e9b63bedc48851f022016b4c59;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.OneMinusNode;55;-256,-1248;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;24;-16,-416;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-160,-528;Inherit;False;Property;_Grunge_Opacity;Grunge_Opacity;14;0;Create;True;0;0;0;False;0;False;0.6238509;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;25;-288,-1008;Inherit;False;Property;_Grunge_Tint;Grunge_Tint;18;0;Create;True;0;0;0;False;0;False;0.5566038,0.5566038,0.5566038,0;0.8396226,0.8396226,0.8396226,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.LerpOp;53;-48,-1296;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;17;-819.3578,-190.0614;Inherit;False;Property;_Tint;Tint;6;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode;11;-883.4305,41.68097;Inherit;True;Property;_Base_Color;Base_Color;1;0;Create;True;0;0;0;False;0;False;-1;d72fa1e9b63bedc48851f022016b4c59;d72fa1e9b63bedc48851f022016b4c59;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;224,-512;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-144,-640;Inherit;False;Constant;_Float1;Float 1;12;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;49;256,-1120;Inherit;True;Property;_Grunge_Texture;Grunge_Texture;10;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-446.4662,-88.62373;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;26;464,-736;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;12;-890.7635,254.2428;Inherit;True;Property;_Metallic_Smoothness;Metallic_Smoothness;2;0;Create;True;0;0;0;False;0;False;-1;None;8ca39db3b331b984ea490ff03978f2f8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;35;-1173.498,523.3627;Inherit;False;Property;_Normal_Scale;Normal_Scale;8;0;Create;True;0;0;0;False;0;False;0;1.2;0;15;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-497.0637,141.413;Inherit;False;Property;_Smoothness;Smoothness;7;0;Create;True;0;0;0;False;0;False;0.2765529;0.2765529;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;608,-400;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;13;-889.6592,454.5734;Inherit;True;Property;_Normal;Normal;3;0;Create;True;0;0;0;False;0;False;-1;None;2d1ac98a92c0e5648895aae83306273e;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode;14;-900.4413,661.6786;Inherit;True;Property;_AmbientOcclusion;Ambient Occlusion;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;298.8624,114.6535;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;28;592,-64;Inherit;True;Property;_Grunge_ON;Grunge_ON;9;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;994.8015,44.92145;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;AE/Grunge;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;8;0;10;0
WireConnection;8;1;9;0
WireConnection;3;0;4;0
WireConnection;3;9;2;0
WireConnection;3;3;8;0
WireConnection;20;1;3;1
WireConnection;20;0;3;2
WireConnection;20;2;3;3
WireConnection;48;0;41;0
WireConnection;23;0;20;0
WireConnection;23;1;21;0
WireConnection;23;2;22;0
WireConnection;50;1;48;0
WireConnection;55;0;54;0
WireConnection;24;0;23;0
WireConnection;53;0;50;0
WireConnection;53;1;55;0
WireConnection;53;2;55;0
WireConnection;11;1;48;0
WireConnection;51;0;52;0
WireConnection;51;1;24;0
WireConnection;49;1;25;0
WireConnection;49;0;53;0
WireConnection;18;0;17;0
WireConnection;18;1;11;0
WireConnection;26;0;34;0
WireConnection;26;1;49;0
WireConnection;26;2;51;0
WireConnection;12;1;48;0
WireConnection;27;0;18;0
WireConnection;27;1;26;0
WireConnection;13;1;48;0
WireConnection;13;5;35;0
WireConnection;14;1;48;0
WireConnection;36;0;33;0
WireConnection;36;1;12;4
WireConnection;28;1;18;0
WireConnection;28;0;27;0
WireConnection;0;0;28;0
WireConnection;0;1;13;0
WireConnection;0;4;36;0
WireConnection;0;5;14;0
ASEEND*/
//CHKSM=31BE3AD97FB1F7479EFD357E78184DE57FBE3F5D