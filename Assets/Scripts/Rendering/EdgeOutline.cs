using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeOutline : MonoBehaviour
{
    ShipPart shipPart;
    List<Mesh> meshes;

    void Start()
    {
        shipPart = gameObject.GetComponent<ShipPart>();
        meshes = shipPart.allMeshes;

        foreach (Mesh mesh in meshes)
        {

            foreach (int vertex in mesh.triangles)
            {
            }
        }
    }
}
