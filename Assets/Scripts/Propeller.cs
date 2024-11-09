using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propeller : MonoBehaviour, ShipMechanism
{
    public FloatingShip parentShip{get; set;}
    public Rigidbody shipRb {get; set;}
    public ShipPart part {get; set;}
    public Vector3 fullForce => part.transform.forward * parentShip.enginePower * part.propellerSpin;
    public Vector3 force = Vector3.zero;
    public float drive = 0.0f;

    public void FixedUpdate(){
        drive = Mathf.MoveTowards( drive, parentShip.throttle, 0.01f ); 
        if( drive != 0.0f ){
            force = Vector3.LerpUnclamped( Vector3.zero, fullForce, drive );
            shipRb.AddForce( force );
        }
    }
}
