using UnityEngine;
using System.Collections;

public class MeshAnchorBuffer : MonoBehaviour {

  private Mesh mesh;
  public GameObject[] meshObjects;
  private Mesh[] meshes;


  public int SIZE = 8;

  int threadX;
  int threadY;
  int threadZ;

  int strideX;
  int strideY;
  int strideZ;
  
  private int gridX { get { return threadX * strideX; } }
  private int gridY { get { return threadY * strideY; } }
  private int gridZ { get { return threadZ * strideZ; } }

  public int vertexCount { get { return gridX * gridY * gridZ; } }

  private int[]     triangles;
  private Vector4[] tangents;
  private Vector3[] normals;
  private Vector2[] uvs;
  private Vector3[] positions;
  private Color[]   colors;

  private int[]     t_triangles;
  private Vector4[] t_tangents;
  private Vector3[] t_normals;
  private Vector2[] t_uvs;
  private Vector3[] t_positions;
  private Color[]   t_colors;

  public ComputeBuffer _anchorBuffer;

  struct Anchor{

    public Vector3 pos;
    public Vector3 norm;
    public Vector3 tan;
    public Vector3 color;
    public Vector2 uv;
    public float id;
    public float triID;
    public float used;
    
  };

  private int AnchorStructSize = 3+3+3+3+2+1+1+1;


  // Use this for initialization
  void Start () {




  
  }
  
  // Update is called once per frame
  void Update () {
  
  }



  public void PopulateBuffer(){

    threadX = SIZE;
    threadY = SIZE;
    threadZ = SIZE;

    strideX = SIZE;
    strideY = SIZE;
    strideZ = SIZE;



    meshes = new Mesh[ meshObjects.Length ];

    for( int i = 0; i < meshObjects.Length; i++ ){
      meshes[i] = meshObjects[i].GetComponent<MeshFilter>().mesh;
    }


    //triangles = mesh.triangles; 
    //positions = mesh.vertices; 
    //normals   = mesh.normals; 
    //tangents  = mesh.tangents; 
    //colors    = mesh.colors; 
    //uvs       = mesh.uv; 
//


    int triLength = 0;
    int posLength = 0;
    int norLength = 0;
    int tanLength = 0;
    int colLength = 0;
    int uvsLength = 0;


    for( int i  = 0; i < meshes.Length; i ++ ){

      triLength += meshes[i].triangles.Length;
      posLength += meshes[i].vertices.Length;
      norLength += meshes[i].normals.Length;
      tanLength += meshes[i].tangents.Length;
      colLength += meshes[i].colors.Length;
      uvsLength += meshes[i].uv.Length;

    }

    if( triLength > vertexCount ){ print( "NOT POSSIBLE BB"); }else{ print( "possible bb : " + triLength ); print( vertexCount );}

    triangles   = new int[ triLength ];
    tangents    = new Vector4[ tanLength ];
    normals     = new Vector3[ norLength ];
    uvs         = new Vector2[ uvsLength ];
    positions   = new Vector3[ posLength ];
    colors      = new Color[ colLength ];

    int b_tri = 0;
    int b_tan = 0;
    int b_nor = 0;
    int b_uvs = 0;
    int b_pos = 0;
    int b_col = 0;

    for( int i = 0; i < meshes.Length; i++ ){

      t_triangles = meshes[i].triangles; 
      t_positions = meshes[i].vertices; 
      t_normals   = meshes[i].normals; 
      t_tangents  = meshes[i].tangents; 
      t_colors    = meshes[i].colors; 
      t_uvs       = meshes[i].uv; 

      for( int j = 0; j < t_positions.Length; j++ ){
        positions[b_pos + j] = meshObjects[i].transform.TransformPoint(t_positions[j]);
      }

      for( int j = 0; j < t_triangles.Length; j++ ){
        triangles[b_tri + j] = t_triangles[j] + b_pos;
      }

      for( int j = 0; j < t_normals.Length; j++ ){
        normals[b_nor + j] = meshObjects[i].transform.TransformDirection(t_normals[j]);
      }

      for( int j = 0; j < t_tangents.Length; j++ ){
        tangents[b_tan + j] = meshObjects[i].transform.TransformDirection(t_tangents[j]);
      }

      for( int j = 0; j < t_colors.Length; j++ ){
        colors[b_col + j] = t_colors[j];
      }

      for( int j = 0; j < t_uvs.Length; j++ ){
        uvs[b_uvs + j] = t_uvs[j];
      }

      b_tri +=  t_triangles.Length; 
      b_pos +=  t_positions.Length; 
      b_nor +=  t_normals.Length;   
      b_tan +=  t_tangents.Length;  
      b_col +=  t_colors.Length;    
      b_uvs +=  t_uvs.Length;       


    }




    createAnchorBuffer();

  }




  public void ReleaseBuffer(){
    _anchorBuffer.Release(); 
  }

  Vector3 ToV3(this Vector4 parent){
    return new Vector3(parent.x, parent.y, parent.z);
  }

  void createAnchorBuffer(){

    _anchorBuffer = new ComputeBuffer( vertexCount , AnchorStructSize * sizeof(float) );    
    //print( AnchorStructSize );
    //print( AnchorStructSize * vc ); 
    float[] inValues = new float[ AnchorStructSize * vertexCount ]; 

          // Used for assigning to our buffer;
    int index = 0;

    for (int i = 0; i < triangles.Length; i++) {

      int id           = triangles[i];

      Vector3 position = positions[id];//transform.TransformPoint(positions[ id ]); 
      Vector3 normal   = normals[id];//transform.TransformDirection(normals[ id ]); 
      Vector4 tangent  = tangents[id];//transform.TransformDirection(tangents[ id ]); 

//      if( i  % 100 == 0 ){ print( tangent ); }

      Vector3 color    = ToV3( Color.white );
      Vector2 uv       = new Vector2(Random.value, Random.value); 

      if( colors.Length > 0 ){ color = ToV3(colors[id]); }   
      if( uvs.Length > 0 ){ uv = uvs[ id ]; }   


      inValues[index++] = position.x;
      inValues[index++] = position.y;
      inValues[index++] = position.z;
      inValues[index++] = normal.x;
      inValues[index++] = normal.y;
      inValues[index++] = normal.z;
      inValues[index++] = tangent.x;
      inValues[index++] = tangent.y;
      inValues[index++] = tangent.z;
      inValues[index++] = color.x;
      inValues[index++] = color.y;
      inValues[index++] = color.z;
      inValues[index++] = uv.x;
      inValues[index++] = uv.y;

      inValues[index++] = i;

      inValues[index++] = (float)i % 3;

      inValues[index++] = 1;

    }

    _anchorBuffer.SetData(inValues);

      
  }


}
