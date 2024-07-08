using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBox : MonoBehaviour
{
    ShipPart shipPart;

    void Start()
    {
        shipPart = GetComponent<ShipPart>();
    }

    void Update()
    {
        foreach (BoxCollider collider in shipPart.boxColliders)
        {
            Vector3 center = collider.transform.position;
            Vector3 size = collider.size;
            Vector3 min = center - size / 2;
            Vector3 max = center + size / 2;
            Debug.DrawLine(min, new Vector3(max.x, min.y, min.z), Color.red);
            Debug.DrawLine(min, new Vector3(min.x, max.y, min.z), Color.red);
            Debug.DrawLine(min, new Vector3(min.x, min.y, max.z), Color.red);
            Debug.DrawLine(max, new Vector3(min.x, max.y, max.z), Color.red);
            Debug.DrawLine(max, new Vector3(max.x, min.y, max.z), Color.red);
            Debug.DrawLine(max, new Vector3(max.x, max.y, min.z), Color.red);
            Debug.DrawLine(new Vector3(min.x, max.y, min.z), new Vector3(max.x, max.y, min.z), Color.red);
            Debug.DrawLine(new Vector3(min.x, max.y, min.z), new Vector3(min.x, max.y, max.z), Color.red);
            Debug.DrawLine(new Vector3(max.x, max.y, min.z), new Vector3(max.x, max.y, max.z), Color.red);
            Debug.DrawLine(new Vector3(min.x, min.y, max.z), new Vector3(max.x, min.y, max.z), Color.red);
            Debug.DrawLine(new Vector3(min.x, min.y, max.z), new Vector3(min.x, max.y, max.z), Color.red);
            Debug.DrawLine(new Vector3(max.x, min.y, max.z), new Vector3(max.x, max.y, max.z), Color.red);
        }
    }
}
