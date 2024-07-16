using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    public Ship ship;
    public Rigidbody shipRb;
    public ShipPart part;
    public float horsepower;
    public Vector3 forceVector => ship.transform.forward * horsepower;

    public void Update(){
        if( Input.GetKey( "w" ) ){
            shipRb.AddForce( forceVector );
        } else if( Input.GetKey( "s" ) ){
            shipRb.AddForce( -forceVector );
        }
    }
}
