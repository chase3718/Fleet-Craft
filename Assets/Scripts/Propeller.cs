using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propeller : MonoBehaviour, ShipMechanism
{
    public Ship ship {get; set;}
    public Rigidbody shipRb {get; set;}
    public ShipPart part {get; set;}
    public Vector3 forceVector => part.transform.forward * ship.enginePower * part.propellerSpin;

    void Start()
    {
        
    }

    public void Update(){
        if( Input.GetKey( "w" ) ){
            shipRb.AddForce( forceVector );
        } else if( Input.GetKey( "s" ) ){
            shipRb.AddForce( -forceVector );
        }
    }
}
