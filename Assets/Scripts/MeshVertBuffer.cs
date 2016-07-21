using UnityEngine;
using System.Collections;

public class MeshVertBuffer : MonoBehaviour {

  public int SIZE = 10;

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


  public ComputeBuffer _vertBuffer;

  struct Vert{

    public Vector3 pos;
    public Vector3 norm;
    public Vector3 vel;
    public Vector3 color;
    public Vector2 uv;

    // should always be defined by righthand rule
    public float id;
    public float id1;
    public float id2;

    public Vector3 debug;
    
  };

  private int VertStructSize = 3+3+3+3+2+1+1+1+3;


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

    createVertBuffer();

  }




  public void ReleaseBuffer(){
    _vertBuffer.Release(); 
  }

  Vector3 ToV3(this Vector4 parent){
    return new Vector3(parent.x, parent.y, parent.z);
  }

  void createVertBuffer(){

    _vertBuffer = new ComputeBuffer( vertexCount , VertStructSize * sizeof(float) );    
    //print( VertStructSize );
    //print( VertStructSize * vc ); 
    float[] inValues = new float[ VertStructSize * vertexCount ]; 

          // Used for assigning to our buffer;
    int index = 0;

    for (int i = 0; i < vertexCount; i++) {

     

      inValues[index++] = 0;
      inValues[index++] = 0;
      inValues[index++] = 0;

      inValues[index++] = 0;
      inValues[index++] = 0;
      inValues[index++] = 0;
      
      inValues[index++] = 0;
      inValues[index++] = 0;
      inValues[index++] = 0;
      
      inValues[index++] = 0;
      inValues[index++] = 0;
      inValues[index++] = 0;

      inValues[index++] = 0;
      inValues[index++] = 0;

      int idVal = i % 3;

      if( idVal == 0 ){
        inValues[index++] = i;
        inValues[index++] = i+1;
        inValues[index++] = i+2;
      }else if( idVal == 1 ){
        inValues[index++] = i;
        inValues[index++] = i+1;
        inValues[index++] = i-1;
      }else if( idVal == 2 ){
        inValues[index++] = i;
        inValues[index++] = i-2;
        inValues[index++] = i-1;
      }

      inValues[index++] = 0;
      inValues[index++] = 0;
      inValues[index++] = 0;


    }

    _vertBuffer.SetData(inValues);

      
  }


}
