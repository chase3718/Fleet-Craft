using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class WaterManager : MonoBehaviour
{
    MeshFilter meshFilter;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    void Update()
    {
        Vector3[] verts = meshFilter.mesh.vertices;
        for (int i = 0; i < verts.Length; i++)
        {
            verts[i].y = WaveManager.instance.GetWaveHeight(verts[i].x);
        }

        meshFilter.mesh.vertices = verts;
        meshFilter.mesh.RecalculateNormals();
    }
}
