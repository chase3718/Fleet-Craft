using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class Floater : MonoBehaviour, ShipMechanism
{
    public FloatingShip parentShip{get; set;}
    public Rigidbody shipRb {get; set;}
    public ShipPart part {get; set;}
    public Vector3 floatPoint =>  part.transform.position+part.centerOfMass;
    public Vector3 massPoint =>  part.transform.position+part.centerOfMass;

    void FixedUpdate()
    {
        //Gravity
        shipRb.AddForceAtPosition( Physics.gravity * part.mass /5000, massPoint);

        //Buoyancy
        float waterHeight = 0f;
        float totalBoyantForce = 0f;

        foreach (BoxCollider collider in part.boxColliders)
        {
            if (collider.bounds.min.y > waterHeight)
            {
                continue;
            }
            float colliderMass = part.mass / part.boxColliders.Count;
            float colliderVolume = part.volume / part.boxColliders.Count;
            //displacement is the amount of the collider that's underwater
            float colliderDisplacement = colliderVolume * Mathf.Clamp( Math.Abs( waterHeight - collider.bounds.min.y ), 0f, collider.bounds.max.y );;
            float colliderBoyantForce = colliderDisplacement * Physics.gravity.y * -1f;
            totalBoyantForce += colliderBoyantForce;
        }
        // if( totalBoyantForce != totalBoyantForce ){ //if null
        //     return;
        // }
        Vector3 boyantForce = new Vector3(0f, -totalBoyantForce, 0f);
        shipRb.AddForceAtPosition(boyantForce/5000, floatPoint);
    

        //Debug
        //Debug.Log( "angular: "+shipRb.angularVelocity+" | velocity: "+shipRb.velocity );
        //Debug.DrawLine( shipRb.transform.TransformPoint(shipRb.centerOfMass), massPoint, new Color( 1.0f, 1.0f, 1.0f ) );

    }
}
