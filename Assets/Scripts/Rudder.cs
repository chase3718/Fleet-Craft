using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rudder : MonoBehaviour, ShipMechanism
{
    public Ship ship {get; set;}
    public Rigidbody shipRb {get; set;}
    public ShipPart part {get; set;}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Calculate the turning force based on speed
        Vector3 turningForce = part.transform.up * ( Mathf.Max(Mathf.Min( (Vector3.Project( shipRb.velocity, ship.transform.forward).magnitude) / 5.0f, 1.0f),-1.0f) );

        if( Input.GetKey( "a" ) ){
            shipRb.AddTorque( (-turningForce) );
        } else if( Input.GetKey( "d" ) ){
            shipRb.AddTorque( (turningForce) );
        }
    }
}
