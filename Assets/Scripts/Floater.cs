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
        shipRb.MovePosition( new Vector3( 0f,1.0f,0f ) );
        //shipRb.AddForceAtPosition( new Vector3( 0f,1f,0f ), part.centerOfMass );
        //shipRb.AddForceAtPosition(Physics.gravity / ship.shipParts.Count, part.centerOfMass);

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
        }
        if( totalBoyantForce != totalBoyantForce ){ //if null
            return;
        }
        Vector3 boyantForce = new Vector3(0f, totalBoyantForce / part.boxColliders.Count, 0f);
        //shipRb.AddForceAtPosition(boyantForce, part.centerOfMass);

        Debug.Log( "angular: "+shipRb.angularVelocity+" | velocity: "+shipRb.velocity );
    }
}
