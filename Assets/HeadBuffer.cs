using UnityEngine;
using System.Collections;

public class HeadBuffer : MonoBehaviour {

	public ComputeBuffer _headBuffer; 
	private float[] headValues;

//We initialize the buffers and the material used to draw.
    void Awake (){

      headValues = new float[AssignStructs.HeadStructSize];

      createBuffers();

    }

    void FixedUpdate(){
      AssignStructs.AssignMat4Buffer( transform.localToWorldMatrix , headValues , _headBuffer );
    }
 
    //When this GameObject is disabled we must release the buffers or else Unity complains.
    private void OnDisable(){
      ReleaseBuffer();
    }


    private void createBuffers() {

      _headBuffer = new ComputeBuffer( 1 , AssignStructs.HeadStructSize * sizeof(float));      

    }

    //Remember to release buffers ead destroy the material when play has been stopped.
    void ReleaseBuffer(){
      _headBuffer.Release(); 
    }
}
