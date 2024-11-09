using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rudder : MonoBehaviour, ShipMechanism
{
    public FloatingShip parentShip{get; set;}
    public Rigidbody shipRb {get; set;}
    public ShipPart part {get; set;}

    // Update is called once per frame
    void FixedUpdate()
    {
        //Calculate the turning force based on speed
        if(parentShip.turn != 0.0f){
            Vector3 turningForce = 5.0f* parentShip.turn *part.transform.up * Mathf.Clamp(Vector3.Project( shipRb.velocity, shipRb.transform.forward).magnitude / 5.0f, -1.0f,1.0f );
            shipRb.AddTorque(turningForce);
        }   
    }
}
