using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class DockedShip : MonoBehaviour
{
    public Ship ship;
    public Transform camRig;

    void Start()
    {
        ship = gameObject.GetComponent<Ship>();
        ship.Load("037a182d-00b8-4834-b63f-0549369c3666");
        InstantiateShip();
        camRig = FindObjectOfType<CameraManager>().transform;
    }

    void InstantiateShip()
    {
        foreach (KeyValuePair<string, ShipPart> part in ship.shipParts)
        {
            GameObject partPrefab = Resources.Load<GameObject>(part.Value.prefabPath);
            GameObject partInstance = Instantiate(partPrefab, part.Value.position, Quaternion.identity);
            partInstance.name = part.Key;
            partInstance.transform.SetParent(transform);
            Ship.SetLayerRecursively(partInstance);
        }
    }

    public bool AddPart(ShipPart part)
    {
        if (AvoidCollisions(part.gameObject) && part.IsStructuallySound())
        {
            if (ship.AddPart(part))
            {
                return true;
            }
            else
            {
                Destroy(part.gameObject);
                return false;
            }
        }
        else
        {
            Destroy(part.gameObject);
            return false;
        }
    }

    public bool RemovePart(ShipPart part)
    {
        if (ship.RemoveBlock(part))
        {
            Destroy(part.gameObject);
            return true;
        }
        return false;
    }

    public void ShiftPosition(ShipPart part, Vector3 normal)
    {
        Vector3 partDimensions = part.transform.rotation * part.dimensions;
        partDimensions = new Vector3(Mathf.Abs(partDimensions.x), Mathf.Abs(partDimensions.y), Mathf.Abs(partDimensions.z));
        Vector3 movement = new Vector3(normal.x * partDimensions.x, normal.y * partDimensions.y, normal.z * partDimensions.z);
        camRig.position += movement;
        camRig.GetComponent<CameraManager>().lastPosition += movement;
    }

    public bool AvoidCollisions(GameObject _obj)
    {
        int rep = 0;
        BoxCollider[] colliders = _obj.GetComponent<ShipPart>().boxColliders.ToArray();
        if (DumbCollision(_obj))
        {
            for (int i = 0; i < colliders.Count() - 1; i++)
            {

                Vector3 childA = colliders[i].transform.position;
                Vector3 childB = colliders[i + 1].transform.position;
                _obj.transform.position += childA - childB;

                if (!DumbCollision(_obj))
                {
                    return true;
                }
            }
        }
        else
        {
            return true;
        }
        return false;
    }


    public bool DumbCollision(GameObject _obj)
    {
        BoxCollider[] colliders = _obj.GetComponent<ShipPart>().boxColliders.ToArray();
        for (int i = 0; i < colliders.Count(); i++)
        {
            string child = colliders[i].transform.position.ToString();
            if (ship.colliderData.ContainsKey(child))
            {
                return true;
            }
        }
        return false;
    }
}
