using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public string id;
    public string shipName;
    public string shipText;
    [HideInInspector] public Dictionary<string, ShipPart> shipParts = new Dictionary<string, ShipPart>();
    public Dictionary<string, GameObject> colliderData = new Dictionary<string, GameObject>();
    private List<string> staticParts = new List<string>();
    private List<string> weaponParts = new List<string>();
    private List<string> engineParts = new List<string>();
    private List<string> rudderParts = new List<string>();
    private List<string> propellerParts = new List<string>();
    private List<string> aircraft = new List<string>();
    public int width;
    public int height;
    public int length;
    public int minX;
    public int minY;
    public int minZ;
    public int maxX;
    public int maxY;
    public int maxZ;
    public float enginePower;

    public string savePath => $"{Application.persistentDataPath}/savedata/ships/{id}/ship.json";

    public bool AddPart(ShipPart part)
    {
        if (shipParts.ContainsKey(part.key))
        {
            print("Part already exists");
            Destroy(part.gameObject);
            return false;
        }
        foreach (BoxCollider collider in part.boxColliders)
        {
            if (colliderData.ContainsKey(collider.transform.position.ToString()))
            {
                print("Collision detected");
                Destroy(part.gameObject);
                shipParts.Remove(part.key);
                SetColliderData();
                return false;
            }
        }
        foreach (BoxCollider collider in part.boxColliders)
        {
            colliderData.Add(collider.transform.position.ToString(), collider.gameObject);
        }
        shipParts.Add(part.key, part);
        SetLayerRecursively(part.gameObject);
        part.transform.SetParent(transform);

        Vector3 partPos = part.position;
        part.transform.position = transform.position + transform.rotation * partPos;
        part.transform.rotation = part.transform.rotation * transform.rotation;

        return true;
    }


    public bool RemoveBlock(ShipPart _obj)
    {
        if (shipParts.Count == 1)
        {
            return false;
        }
        SetDimensions();
        if (shipParts.ContainsKey(_obj.key))
        {
            Dictionary<string, ShipPart> tempParts = shipParts.ToDictionary(entry => entry.Key, entry => entry.Value);
            tempParts.Remove(_obj.key);
            if (CheckCongruency(tempParts))
            {
                shipParts.Remove(_obj.key);
                SetColliderData();
                return true;
            }
            return false;
        }
        return false;
    }

    ShipPartCollider[,,] checkArray;
    int totalStructuralParts;
    bool CheckCongruency(Dictionary<string, ShipPart> parts)
    {
        totalStructuralParts = 0;
        checkArray = new ShipPartCollider[width, height, length];
        Vector3Int checkStart = new Vector3Int(0, 0, 0);
        foreach (KeyValuePair<string, ShipPart> part in parts)
        {
            foreach (BoxCollider collider in part.Value.boxColliders)
            {
                if (collider.GetComponent<ShipPartCollider>().isStructural)
                {
                    Vector3Int pos = new Vector3Int((int)collider.transform.position.x - minX, (int)collider.transform.position.y - minY, (int)collider.transform.position.z - minZ);
                    print(pos.x + " " + pos.y + " " + pos.z);
                    checkArray[pos.x, pos.y, pos.z] = collider.GetComponent<ShipPartCollider>();
                    checkStart = pos;
                    totalStructuralParts++;
                }
            }
        }

        return CheckCongruency(checkStart);
    }


    bool CheckCongruency(Vector3Int start)
    {
        Queue<KeyValuePair<Vector3Int, ShipPartCollider>> queue = new Queue<KeyValuePair<Vector3Int, ShipPartCollider>>();
        queue.Enqueue(new KeyValuePair<Vector3Int, ShipPartCollider>(start, checkArray[start.x, start.y, start.z]));
        checkArray[start.x, start.y, start.z] = null;
        int connectedParts = 0;
        while (queue.Count > 0)
        {
            KeyValuePair<Vector3Int, ShipPartCollider> current = queue.Dequeue();
            connectedParts++;
            foreach (Vector3Int dir in current.Value.checkDirections)
            {
                Vector3Int checkPos = current.Key + dir;
                if (checkPos.x >= 0 && checkPos.x < width && checkPos.y >= 0 && checkPos.y < height && checkPos.z >= 0 && checkPos.z < length)
                {
                    if (checkArray[checkPos.x, checkPos.y, checkPos.z] != null)
                    {
                        queue.Enqueue(new KeyValuePair<Vector3Int, ShipPartCollider>(checkPos, checkArray[checkPos.x, checkPos.y, checkPos.z]));
                        checkArray[checkPos.x, checkPos.y, checkPos.z] = null;
                    }
                }
            }
        }
        return connectedParts == totalStructuralParts;
    }

    public void SetDimensions()
    {
        minX = Int32.MaxValue;
        minY = Int32.MaxValue;
        minZ = Int32.MaxValue;
        maxX = Int32.MinValue;
        maxY = Int32.MinValue;
        maxZ = Int32.MinValue;

        foreach (Vector3Int position in colliderData.ToArray().Select(x => ToVector3Int(x.Key)))
        {
            if ((int)position.x < (int)minX)
            {
                minX = (int)position.x;
            }
            if ((int)position.x > (int)maxX)
            {
                maxX = (int)position.x;
            }
            if ((int)position.y < (int)minY)
            {
                minY = (int)position.y;
            }
            if ((int)position.y > (int)maxY)
            {
                maxY = (int)position.y;
            }
            if ((int)position.z < (int)minZ)
            {
                minZ = (int)position.z;
            }
            if ((int)position.z > (int)maxZ)
            {
                maxZ = (int)position.z;
            }
        }


        if (minX == Int32.MaxValue)
        {
            minX = 0;
        }
        if (minY == Int32.MaxValue)
        {
            minY = 0;
        }
        if (minZ == Int32.MaxValue)
        {
            minZ = 0;
        }
        if (maxX == Int32.MinValue)
        {
            maxX = 0;
        }
        if (maxY == Int32.MinValue)
        {
            maxY = 0;
        }
        if (maxZ == Int32.MinValue)
        {
            maxZ = 0;
        }


        width = (int)Mathf.Abs(maxX - minX) + 1;
        height = (int)Mathf.Abs(maxY - minY) + 1;
        length = (int)Mathf.Abs(maxZ - minZ) + 1;
    }
    public static void SetLayerRecursively(GameObject part)
    {
        part.layer = 6;
        foreach (GameObject child in part.transform.Cast<Transform>().Select(t => t.gameObject))
        {
            SetLayerRecursively(child);
        }
    }

    void SetStaticParts()
    {
        staticParts.Clear();
        foreach (KeyValuePair<string, ShipPart> part in shipParts)
        {
            if (part.Value.isStatic)
            {
                staticParts.Add(part.Key);
            }
        }
    }

    void SetWeaponParts()
    {
        weaponParts.Clear();
        foreach (KeyValuePair<string, ShipPart> part in shipParts)
        {
            if (part.Value.GetComponent<Weapon>() != null)
            {
                weaponParts.Add(part.Key);
            }
        }
    }

    void SetColliderData()
    {
        colliderData.Clear();
        foreach (KeyValuePair<string, ShipPart> part in shipParts)
        {
            foreach (BoxCollider collider in part.Value.boxColliders)
            {
                colliderData.Add(collider.transform.position.ToString(), collider.gameObject);
            }
        }
    }
    private Vector3Int ToVector3Int(string pos)
    {
        if (pos.StartsWith("(") && pos.EndsWith(")"))
        {
            pos = pos.Substring(1, pos.Length - 2);
        }

        string[] sArray = pos.Split(',');

        Vector3Int result = new Vector3Int((int)float.Parse(sArray[0]), (int)float.Parse(sArray[1]), (int)float.Parse(sArray[2]));

        return result;
    }
    public string Serialize()
    {
        if (id == null)
        {
            id = System.Guid.NewGuid().ToString();
        }
        SerializableShip ss = new SerializableShip();
        ss.id = id;
        ss.shipName = shipName;
        ss.shipText = shipText;
        ss.shipParts = shipParts.Values.Select(part => part.Serialize()).ToList();
        return JsonConvert.SerializeObject(ss);
    }

    public void Deserialize(string json)
    {
        var jsonResult = JsonConvert.DeserializeObject(json).ToString();
        SerializableShip ss = JsonConvert.DeserializeObject<SerializableShip>(jsonResult);
        id = ss.id;
        shipName = ss.shipName.Replace("_", " ");
        shipText = ss.shipText;
        shipParts.Clear();

        foreach (ShipPart.SerializeableShipPart ssp in ss.shipParts)
        {
            GameObject part = Instantiate(Resources.Load(ssp.prefabPath) as GameObject);
            ShipPart shipPart = part.GetComponent<ShipPart>().Deserialize(ssp);
            part.name = shipPart.key;
            shipPart.transform.SetParent(transform);
            AddPart(shipPart);
        }

        SetStaticParts();
        SetWeaponParts();
        SetColliderData();
        SetDimensions();
    }
    public void Save()
    {
        string json = Serialize();
        string destination = savePath;
        FileStream file;

        if (File.Exists(destination))
        {
            file = File.OpenWrite(destination);
        }
        else
        {
            if (!Directory.Exists($"{Application.persistentDataPath}/savedata/ships/{id}"))
            {
                Directory.CreateDirectory($"{Application.persistentDataPath}/savedata/ships/{id}/");
            }
            file = File.Create(destination);
        }

        byte[] data = Encoding.ASCII.GetBytes(json);
        file.Write(data, 0, data.Length);
        file.Close();



    }

    public void Load(string _id)
    {

        string destination = $"{Application.persistentDataPath}/savedata/ships/{_id}/ship.json";
        FileStream file;

        if (File.Exists(destination))
        {
            file = File.OpenRead(destination);
        }
        else
        {
            Debug.LogError("File not found");
            PremadeShip();
            return;
        }

        byte[] data = new byte[file.Length];
        file.Read(data, 0, data.Length);
        file.Close();


        string json = JsonConvert.SerializeObject(Encoding.ASCII.GetString(data));
        Deserialize(json);
    }
    //Just for testing
    public void PremadeShip()
    {
        string data = Resources.Load<TextAsset>("Premade Ships/ship").ToString();


        string json = JsonConvert.SerializeObject(data);
        Deserialize(json);

    }

    public string Hash()
    {
        string value = Serialize();
        StringBuilder Sb = new StringBuilder();

        using (SHA256 hash = SHA256Managed.Create())
        {
            Encoding enc = Encoding.UTF8;
            Byte[] result = hash.ComputeHash(enc.GetBytes(value));

            foreach (Byte b in result)
                Sb.Append(b.ToString("x2"));
        }

        return Sb.ToString();
    }
    public class SerializableShip
    {
        public string id;
        public string shipName;
        public string shipText;
        public List<ShipPart.SerializeableShipPart> shipParts = new List<ShipPart.SerializeableShipPart>();

    }
}
