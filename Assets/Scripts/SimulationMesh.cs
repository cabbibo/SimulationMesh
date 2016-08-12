using UnityEngine;
using System.Collections;

public class SimulationMesh : MonoBehaviour {


  public GameObject VertBuffer;
  public GameObject AnchorBuffer;
  public GameObject AnchorBuffer2;
  public GameObject HandBuffer;
  public GameObject HeadBuffer;

  public ComputeShader computeShader;
  public Material material;

  private int _kernel;


  private int vertCount;
  private int size;

  private int activeMesh = 0;

	// Use this for initialization
	void Start () {

    VertBuffer.GetComponent<MeshVertBuffer>().PopulateBuffer();
    vertCount = VertBuffer.GetComponent<MeshVertBuffer>().vertexCount;

    size = VertBuffer.GetComponent<MeshVertBuffer>().SIZE;

    AnchorBuffer.GetComponent<MeshAnchorBuffer>().PopulateBuffer();
    AnchorBuffer2.GetComponent<MeshAnchorBuffer>().PopulateBuffer();


    //print( AnchorBuffers[0].GetComponent<MeshAnchorBuffer>()._anchorBuffer );

    _kernel = computeShader.FindKernel("CSMain");

    //EventManager.OnTouchpadDown += OnTouchpadDown;
    //EventManager.OnTouchpadUp += OnTouchpadUp;

    Camera.onPostRender += Render;
    Set();

	}

  void Render(Camera c){

    material.SetPass(0);
    material.SetBuffer("buf_Points", VertBuffer.GetComponent<MeshVertBuffer>()._vertBuffer);

    Graphics.DrawProcedural(MeshTopology.Triangles, vertCount - ( vertCount %3 ));

  }

  void OnDisable(){
    Camera.onPostRender -= Render;
  }

  void Set(){
    computeShader.SetInt( "_Set" , 1 );
    computeShader.SetInt( "_NumberHands" ,HandBuffer.GetComponent<HandBuffer>().numberHands  );

    computeShader.SetBuffer( _kernel , "vertBuffer"     , VertBuffer.GetComponent<MeshVertBuffer>()._vertBuffer );
    computeShader.SetBuffer( _kernel , "anchorBuffer"   , AnchorBuffer.GetComponent<MeshAnchorBuffer>()._anchorBuffer );
    computeShader.SetBuffer( _kernel , "anchorBuffer2"   , AnchorBuffer2.GetComponent<MeshAnchorBuffer>()._anchorBuffer );

    computeShader.SetBuffer( _kernel , "handBuffer"     , HandBuffer.GetComponent<HandBuffer>()._handBuffer );
    computeShader.SetBuffer( _kernel , "headBuffer"     , HeadBuffer.GetComponent<HeadBuffer>()._headBuffer );

    computeShader.Dispatch( _kernel, size , size , size );
  }

    // Update is called once per frame
  void FixedUpdate () {
    

    computeShader.SetInt( "_Set" , 0 );
    computeShader.SetInt( "_NumberHands" ,HandBuffer.GetComponent<HandBuffer>().numberHands  );

    computeShader.SetBuffer( _kernel , "vertBuffer"     , VertBuffer.GetComponent<MeshVertBuffer>()._vertBuffer );
    computeShader.SetBuffer( _kernel , "anchorBuffer"   , AnchorBuffer.GetComponent<MeshAnchorBuffer>()._anchorBuffer );
    computeShader.SetBuffer( _kernel , "anchorBuffer2"  , AnchorBuffer2.GetComponent<MeshAnchorBuffer>()._anchorBuffer );

    computeShader.SetBuffer( _kernel , "handBuffer"     , HandBuffer.GetComponent<HandBuffer>()._handBuffer );
    computeShader.SetBuffer( _kernel , "headBuffer"     , HeadBuffer.GetComponent<HeadBuffer>()._headBuffer );

    computeShader.Dispatch( _kernel, size , size , size );

  }
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnTouchpadUp( GameObject g ){

    activeMesh ++;
    // if( activeMesh == AnchorBuffers.Length){ activeMesh = 0;}
    // print( "ACTIVE");

  }

  void OnTouchpadDown( GameObject g ){
   

  }







}
