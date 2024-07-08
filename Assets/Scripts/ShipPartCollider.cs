using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ShipPartCollider : MonoBehaviour
{
    public bool isStructural;
    public List<Vector3Int> checkDirections = new List<Vector3Int>();
    public bool sturdyComponent;

    void Awake()
    {

        if (checkDirections.Count == 0)
        {
            checkDirections.Add(Vector3Int.up);
            checkDirections.Add(Vector3Int.down);
            checkDirections.Add(Vector3Int.left);
            checkDirections.Add(Vector3Int.right);
            checkDirections.Add(Vector3Int.forward);
            checkDirections.Add(Vector3Int.back);
        }
        if (checkDirections.Count == 1 && checkDirections[0] == Vector3Int.zero)
        {
            checkDirections.Clear();
        }
    }

    public bool CheckConnections()
    {
        foreach (Vector3Int dir in checkDirections)
        {
            Vector3Int checkPos = ShipPart.ToVector3Int(transform.position) + dir;
            Collider[] colliders = Physics.OverlapSphere(checkPos, 0.1f, layerMask: 1 << 6);
            if (colliders.Length > 0)
            {
                foreach (Collider col in colliders)
                {
                    ShipPartCollider partCollider = col.GetComponent<ShipPartCollider>();
                    if (partCollider.isStructural &&
                        partCollider.checkDirections.Contains(-dir) &&
                        partCollider.transform.parent != transform.parent)
                    {
                        print("Connected");
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
