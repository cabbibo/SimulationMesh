Shader "Custom/RenderSimMesh" {
	Properties {
        _NormalMap( "Normal Map" , 2D ) = "white" {}
        _CubeMap( "Cube Map" , Cube ) = "white" {}
        _TexMap( "Tex Map" , 2D ) = "white" {}
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
      uniform sampler2D _TexMap;
      uniform samplerCUBE _CubeMap;
      uniform float _SizeMultiplier;



struct Vert{

  float3 pos;
  float3 nor;
  float3 vel;
  float3 col;
  float2 uv;

  // should always be defined by righthand rule
  float id;
  float id1;
  float id2;

  float3 debug;
  
};


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
      StructuredBuffer<Vert> buf_Points;

      //A simple input struct for our pixel shader step containing a position.
      struct varyings {
          float4 pos 			: SV_POSITION;
          float3 nor 			: TEXCOORD0;
          float2 uv  			: TEXCOORD1;
          //float2 suv 			: TEXCOORD2;
          //float3 col 			: TEXCOORD3;
          //float  lamb 		: TEXCOORD4;
          float3 eye      : TEXCOORD5;
          float3 worldPos : TEXCOORD6;
          float3 debug    : TEXCOORD7;

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


        Vert v = buf_Points[id];
        Vert v1 = buf_Points[v.id1];
        Vert v2 = buf_Points[v.id2];



				o.pos = mul (UNITY_MATRIX_VP, float4(v.pos,1.0f));
				o.worldPos = v.pos;
				o.eye = _WorldSpaceCameraPos - o.worldPos;
	
				o.nor = normalize( cross( normalize( v.pos-v1.pos) , normalize( v.pos-v2.pos)));
				o.uv = v.uv;
        o.debug = v.debug;

        return o;


      }
      //Pixel function returns a solid color for each point.
      float4 frag (varyings v) : COLOR {

      	float3 fNorm = v.nor; //uvNormalMap( _NormalMap , v.worldPos , v.uv ,v.nor , 10 ,14);

        float3 col = fNorm * .5 + .5;//i.debug;

        float3 fRefl = reflect( -normalize(v.eye) , fNorm );
       	float3 cubeCol = texCUBE(_CubeMap,fRefl ).rgb;
        //col = float3( v.uv.x , 0 , v.uv.y );
        //col = tex2D( _TexMap , v.uv ).xyz * (fRefl * .5 + .5) * cubeCol * 2;
        //col = normalize( (fRefl * .5 + .5) );

       // col = v.debug + float3( 0 , .1 , 0 );
       col = cubeCol;

        return float4( col , 1. );


      }

      ENDCG

    }
  }

  Fallback Off
  
}