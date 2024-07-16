using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propeller : MonoBehaviour, ShipMechanism
{
    public Ship ship {get; set;}
    public Rigidbody shipRb {get; set;}
    public ShipPart part {get; set;}
    public Vector3 fullForce => part.transform.forward * ship.enginePower * part.propellerSpin;
    public Vector3 force = Vector3.zero;
    public float throttle = 0.0f;

    public void FixedUpdate(){
        if( Input.GetKey( "w" )){ 
            throttle = Mathf.MoveTowards(throttle, 1.0f, 0.01f);
            Debug.Log( "Forward : "+ throttle );
        } else if( Input.GetKey( "s" )){
            throttle = Mathf.MoveTowards(throttle, -1.0f, 0.01f);
            Debug.Log( "Backward : "+ throttle );
        } else if( throttle != 0.0f ){ //spool to zero
            throttle = Mathf.MoveTowards( throttle, 0.0f, 0.01f ); 
            Debug.Log( "Zeroing : "+ throttle );
        }
    }

    public void Update(){
        if( throttle != 0.0f ){
            force = Vector3.LerpUnclamped( Vector3.zero, fullForce, throttle );
            shipRb.AddForce( force );
        }
    }
}
