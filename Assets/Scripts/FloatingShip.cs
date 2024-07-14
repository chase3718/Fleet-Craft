using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FloatingShip : MonoBehaviour
{
    Ship ship;
    void Start()
    {
        ship = gameObject.GetComponent<Ship>();
        ship.Load("037a182d-00b8-4834-b63f-0549369c3666");
        //Testing purposes
        //ship.PremadeShip();
        InstantiateShip();
    }

    void InstantiateShip()
    {
        Rigidbody shipRb = ship.GetComponent<Rigidbody>();
        shipRb.constraints = RigidbodyConstraints.None;
        foreach (ShipPart part in ship.shipParts.Values)
        {
            Debug.Log(part.partName);

            Floater floater = part.AddComponent<Floater>();
            floater.ship = ship;
            floater.shipRb = shipRb;
            floater.part = part;
        }
    }
}
