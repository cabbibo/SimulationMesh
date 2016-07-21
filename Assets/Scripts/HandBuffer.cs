using UnityEngine;
 

public class HandBuffer : MonoBehaviour
{
    public GameObject[] Hands; 
    public int numberHands{ get { return Hands.Length; } }
    public ComputeBuffer _handBuffer; 
    private float[] handValues;

  

 
    //We initialize the buffers and the material used to draw.
    void Awake (){

      handValues = new float[numberHands * AssignStructs.HandStructSize];

      createBuffers();

    }

    void FixedUpdate(){
      AssignStructs.AssignHandBuffer( Hands , handValues , _handBuffer );
    }
 
    //When this GameObject is disabled we must release the buffers or else Unity complains.
    private void OnDisable(){
      ReleaseBuffer();
    }


    private void createBuffers() {

      _handBuffer = new ComputeBuffer( numberHands , AssignStructs.HandStructSize * sizeof(float));      

    }

    //Remember to release buffers and destroy the material when play has been stopped.
    void ReleaseBuffer(){
      _handBuffer.Release(); 
    }

}