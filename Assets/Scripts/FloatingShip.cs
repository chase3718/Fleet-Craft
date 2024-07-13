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
        foreach (ShipPart part in ship.shipParts.Values)
        {
            Floater floater = part.AddComponent<Floater>();
            floater.ship = ship;
            floater.shipRb = ship.GetComponent<Rigidbody>();
            floater.part = part;
        }
    }
}
