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
    public Vector3 floatPoint =>  part.transform.position+part.centerOfMass;
    public Vector3 massPoint =>  part.transform.position+part.centerOfMass;

    void FixedUpdate()
    {
        //Gravity
        shipRb.AddForceAtPosition(Physics.gravity / ship.shipParts.Count, massPoint);

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
        shipRb.AddForceAtPosition(boyantForce / ship.shipParts.Count, floatPoint);
    

        //Debug
        Debug.Log( "angular: "+shipRb.angularVelocity+" | velocity: "+shipRb.velocity );
        Debug.DrawLine( shipRb.transform.TransformPoint(shipRb.centerOfMass), part.position, new Color( 1.0f, 1.0f, 1.0f ) );

    }
}
