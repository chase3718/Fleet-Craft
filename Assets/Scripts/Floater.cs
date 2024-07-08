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
        shipRb.AddForceAtPosition(Physics.gravity / ship.shipParts.Count, part.centerOfMass);
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

        Vector3 boyantForce = new Vector3(0f, totalBoyantForce / part.boxColliders.Count, 0f);
        shipRb.AddForceAtPosition(boyantForce, part.centerOfMass);
    }
}
