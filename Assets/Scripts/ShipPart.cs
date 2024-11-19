using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum ShipPartCategory
{
    Hull,
    Engine,
    Weapon,
    Aircraft,
    Decoration
}

public class ShipPart : MonoBehaviour
{
    public string key => $"{partName}_{ToVector3Int(position)}";
    public string partName;
    public string alias;
    public string description;
    public float mass;
    public float volume;
    public float armor;
    public float toughness;
    public float health;
    public float horsepower;
    public float propellerSpin;
    public bool isRudder;
    public float firepower;

    public Vector3[] muzzles;
    public float reloadTime;
    
    public Color paintColor;
    public ShipPartCategory category;
    public bool isStatic;
    public bool enableOffset;
    public bool lockHorizontalRotation, lockVerticalRotation;
    public bool requireSturdyConnection;
    public Vector3Int position => ToVector3Int(transform.position);
    public string prefabPath => $"Prefabs/ShipParts/{category}/{partName}";
    public List<BoxCollider> boxColliders;
    public List<Mesh> staticMeshes => GetStaticMeshes();
    public List<Mesh> allMeshes => GetAllMeshes();
    public Vector3Int dimensions => GetDimensions();
    public Vector3 centerOfMass => GetCenterOfMass();

    void Awake()
    {
        SetBoxColliders();
    }

    public void Highlight(Color color)
    {
        foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
        {
            // add emission to the material
            Material material = meshRenderer.material;
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", color);

        }
    }

    public void Unhighlight()
    {
        foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
        {
            // remove emission from the material
            Material material = meshRenderer.material;
            material.DisableKeyword("_EMISSION");
        }
    }

    public List<BoxCollider> SetBoxColliders()
    {
        GameObject colliderContainer = gameObject.transform.Find("Colliders").gameObject;
        List<BoxCollider> colliders = new List<BoxCollider>();
        foreach (BoxCollider child in colliderContainer.GetComponentsInChildren<BoxCollider>())
        {
            if (!child.gameObject.activeSelf)
            {
                continue;
            }
            colliders.Add(child.GetComponent<BoxCollider>());
        }
        boxColliders = colliders;
        return colliders;
    }

    Vector3 GetCenterOfMass()
    {
        Vector3 center = Vector3.zero;
        foreach (BoxCollider collider in boxColliders)
        {
            center += collider.transform.localPosition;
        }
        return center / boxColliders.Count;
    }

    List<Mesh> GetStaticMeshes()
    {
        Transform meshContainer = transform.Find("Meshes");
        Transform staticMeshContainer = meshContainer.Find("StaticMeshes");
        List<Mesh> meshes = new List<Mesh>();
        foreach (Transform child in staticMeshContainer)
        {
            meshes.Add(child.GetComponent<MeshFilter>().sharedMesh);
        }
        return meshes;
    }

    List<Mesh> GetAllMeshes()
    {
        Transform meshContainer = transform.Find("Meshes");
        List<Mesh> meshes = new List<Mesh>();
        foreach (MeshFilter child in meshContainer.GetComponentsInChildren<MeshFilter>())
        {
            meshes.Add(child.sharedMesh);
        }
        return meshes;
    }

    Vector3Int GetDimensions()
    {

        Vector3 min = Vector3.positiveInfinity;
        Vector3 max = Vector3.negativeInfinity;
        foreach (BoxCollider collider in boxColliders)
        {
            min = Vector3.Min(min, collider.transform.position);
            max = Vector3.Max(max, collider.transform.position);
        }
        return ToVector3Int(new Vector3(Mathf.Abs(max.x - min.x) + 1, Mathf.Abs(max.y - min.y) + 1, Mathf.Abs(max.z - min.z) + 1));
    }

    public SerializeableShipPart Serialize()
    {
        SerializeableShipPart ssp = new SerializeableShipPart();
        ssp.partName = partName;
        ssp.mass = mass;
        ssp.volume = volume;
        ssp.armor = armor;
        ssp.toughness = toughness;
        ssp.health = health;
        ssp.category = category;
        ssp.isStatic = isStatic;
        ssp.enableOffset = enableOffset;

        ssp.paintColor = new SerializeableShipPart.PaintColor(paintColor);
        ssp.position = new SerializeableShipPart.Position(position);
        ssp.rotation = new SerializeableShipPart.Rotation(transform.rotation);

        // Serialize to xml
        return ssp;
    }

    public ShipPart Deserialize(SerializeableShipPart ssp)
    {
        partName = ssp.partName;
        mass = ssp.mass;
        volume = ssp.volume;
        armor = ssp.armor;
        toughness = ssp.toughness;
        health = ssp.health;
        category = ssp.category;
        isStatic = ssp.isStatic;
        enableOffset = ssp.enableOffset;
        paintColor = ssp.paintColor.ToColor();
        gameObject.name = partName;
        transform.position = ssp.position.ToVector3();
        transform.rotation = ssp.rotation.ToQuaternion();
        SetBoxColliders();
        return this;
    }

    public static Vector3Int ToVector3Int(Vector3 vector)
    {
        return new Vector3Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), Mathf.RoundToInt(vector.z));
    }

    public bool IsStructuallySound()
    {
        SetBoxColliders();
        if (requireSturdyConnection)
        {
            foreach (BoxCollider col in boxColliders)
            {
                ShipPartCollider partCollider = col.GetComponent<ShipPartCollider>();
                print("checking is structurally sound" + partCollider.CheckConnections());
                if (partCollider.sturdyComponent && !partCollider.CheckConnections())
                {
                    return false;
                }
            }
            return true;
        }
        foreach (BoxCollider col in boxColliders)
        {
            ShipPartCollider partCollider = col.GetComponent<ShipPartCollider>();
            if (partCollider.CheckConnections())
            {
                return true;
            }
        }
        return false;
    }

    public class SerializeableShipPart
    {
        public string partName;
        public float mass;
        public float volume;
        public float armor;
        public float toughness;
        public float health;
        public PaintColor paintColor;
        public ShipPartCategory category;
        public Position position;
        public Rotation rotation;
        public bool isStatic;
        public bool enableOffset;

        public string prefabPath => $"Prefabs/ShipParts/{category}/{partName}";

        public class Position
        {
            public float x;
            public float y;
            public float z;

            public Position(Vector3 position)
            {
                x = position.x;
                y = position.y;
                z = position.z;
            }

            public Vector3 ToVector3()
            {
                return new Vector3(x, y, z);
            }
        }

        public class Rotation
        {
            public float x;
            public float y;
            public float z;
            public float w;

            public Rotation(Quaternion rotation)
            {
                x = rotation.x;
                y = rotation.y;
                z = rotation.z;
                w = rotation.w;
            }

            public Quaternion ToQuaternion()
            {
                return new Quaternion(x, y, z, w);
            }
        }

        public class PaintColor
        {
            public float r;
            public float g;
            public float b;
            public float a;

            public PaintColor(Color color)
            {
                r = color.r;
                g = color.g;
                b = color.b;
                a = color.a;
            }

            public Color ToColor()
            {
                return new Color(r, g, b, a);
            }
        }
    }
}
