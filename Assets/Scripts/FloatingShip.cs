using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FloatingShip : MonoBehaviour
{
    Ship ship;
    Rigidbody shipRb;
    void Start()
    {
        ship = gameObject.GetComponent<Ship>();
        shipRb = ship.GetComponent<Rigidbody>();
        ship.Load("037a182d-00b8-4834-b63f-0549369c3666");
        //Testing purposes
        //ship.PremadeShip();
        InstantiateShip();
    }

    void InstantiateShip()
    {
        shipRb.constraints = RigidbodyConstraints.None;
        foreach (ShipPart part in ship.shipParts.Values)
        {
            Debug.Log(part.partName);

            InitComponent<Floater>(part);

            if( part.horsepower > 0 ){
                InitComponent<Engine>(part);
            }
            if( part.propellerSpin > 0 ){
                InitComponent<Propeller>(part);
            }
            if( part.isRudder ){
                InitComponent<Rudder>(part);
            }
            if( part.firepower > 0 ){
                InitComponent<Weapon>(part);
            }
        }
    }

    //initialise the components easier
    public T InitComponent<T >(ShipPart part) where T: Component, ShipMechanism
    {
        T component = part.AddComponent<T>();
        component.ship = ship;
        component.shipRb = shipRb;
        component.part = part;
        return component;
    }

    void FixedUpdate()
    {
        //Drag
        shipRb.AddForce( Vector3.zero - shipRb.velocity.normalized * (shipRb.velocity.magnitude * 5.0f) );
        shipRb.AddTorque( Vector3.zero - shipRb.angularVelocity.normalized * (shipRb.angularVelocity.magnitude * 10.5f) );
    }
}