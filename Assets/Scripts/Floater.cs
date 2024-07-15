using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class Floater : MonoBehaviour
{
    public Ship ship;
    public Rigidbody shipRb;
    public ShipPart part;

    void FixedUpdate()
    {
        //Gravity
        shipRb.AddForceAtPosition(Physics.gravity / ship.shipParts.Count, shipRb.centerOfMass);

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
            float colliderDisplacement = colliderVolume / colliderMass;
            float colliderBoyantForce = colliderDisplacement * Physics.gravity.y * -1f;
            totalBoyantForce += colliderBoyantForce;

            //dampening
            totalBoyantForce *= Mathf.Abs( Mathf.Clamp( (waterHeight - collider.bounds.min.y), 0f, 1f ));
        }
        if( totalBoyantForce != totalBoyantForce ){ //if null
            return;
        }
        Vector3 boyantForce = new Vector3(0f, totalBoyantForce / part.boxColliders.Count, 0f);

        shipRb.AddForceAtPosition(boyantForce / ship.shipParts.Count, shipRb.centerOfMass);

        //Drag
        shipRb.AddForceAtPosition( Vector3.zero - shipRb.velocity.normalized * (shipRb.velocity.magnitude * 0.02f), shipRb.centerOfMass );  

        Debug.Log( "angular: "+shipRb.angularVelocity+" | velocity: "+shipRb.velocity );
    }
}
