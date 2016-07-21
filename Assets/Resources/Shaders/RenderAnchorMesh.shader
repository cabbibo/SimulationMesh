Shader "Custom/RenderAnchorMesh" {
	Properties {
        _NormalMap( "Normal Map" , 2D ) = "white" {}
        _CubeMap( "Cube Map" , Cube ) = "white" {}
        _SpriteMap( "Sprite Map" , 2D ) = "white" {}
        _SizeMultiplier( "Size Multiplier" , float ) = 1
    }
  SubShader{

  	


    Cull off
    Pass{


      CGPROGRAM
      #pragma target 5.0

      #pragma vertex vert
      #pragma fragment frag

      #include "UnityCG.cginc"

      uniform sampler2D _NormalMap;
      uniform sampler2D _SpriteMap;
      uniform samplerCUBE _CubeMap;
      uniform float _SizeMultiplier;


      

struct Anchor{
  float3 pos;
  float3 nor;
  float3 tang;
  float3 col;
  float2 uv;
  float id;
  float triID;
  float used;
};

      StructuredBuffer<Anchor> buf_Points;

      //A simple input struct for our pixel shader step containing a position.
      struct varyings {
          float4 pos 			: SV_POSITION;
          float3 nor 			: TEXCOORD0;
          float2 uv  			: TEXCOORD1;
          //float2 suv 			: TEXCOORD2;
          //float3 col 			: TEXCOORD3;
          //float  lamb 		: TEXCOORD4;
          //float3 eye      : TEXCOORD5;
          //float3 worldPos : TEXCOORD6;

      };

			float3 uvNormalMap( sampler2D normalMap , float3 pos , float2 uv , float3 norm , float texScale , float normalScale ){
			             
			 	float3 q0 = ddx( pos.xyz );
			  float3 q1 = ddy( pos.xyz );
			  float2 st0 = ddx( uv.xy );
			  float2 st1 = ddy( uv.xy );

			  float3 S = normalize(  q0 * st1.y - q1 * st0.y );
			  float3 T = normalize( -q0 * st1.x + q1 * st0.x );
			  float3 N = normalize( norm );

			  float3 mapN = tex2D( normalMap, uv*texScale ).xyz * 2.0 - 1.0;
			  mapN.xy = normalScale * mapN.xy;
			 
			  float3x3 tsn = transpose( float3x3( S, T, N ) );
			  float3 fNorm =  normalize( mul(tsn , mapN) ); 

			  return fNorm;

			}

			float hash( float n ){
			  return frac(sin(n)*43758.5453);
			}

      varyings vert (uint id : SV_VertexID){

        varyings o;


        Anchor v = buf_Points[id];

				o.pos = mul (UNITY_MATRIX_VP, float4(v.pos,1.0f));
	
				o.nor = v.nor;
				o.uv = v.uv;

        return o;


      }
      //Pixel function returns a solid color for each point.
      float4 frag (varyings i) : COLOR {

        float3 col = i.nor * .5 + .5;//i.debug;

        return float4( col , 1. );


      }

      ENDCG

    }
  }

  Fallback Off
  
}