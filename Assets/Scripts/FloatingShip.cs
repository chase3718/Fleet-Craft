using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FloatingShip : MonoBehaviour
{
    Ship ship;
    Rigidbody shipRb;

    List<Weapon> weapons = new List<Weapon>();
    List<Rudder> rudders = new List<Rudder>();
    List<Propeller> propellers = new List<Propeller>(); 

    public float enginePower = 0;

    public float throttle;
    public float turn;

    public float totalHealth;
    public float currentHealth;

    void Start()
    {
        ship = gameObject.GetComponent<Ship>();
        shipRb = GetComponent<Rigidbody>();
        ship.Load("037a182d-00b8-4834-b63f-0549369c3666");
        ship.transform.SetParent(this.transform);
        //Testing purposes
        //ship.PremadeShip();
        InstantiateShip();
    }

    void InstantiateShip()
    {
        shipRb.constraints = RigidbodyConstraints.None;
        foreach (ShipPart part in ship.shipParts.Values)
        {
            //Debug.Log(part.partName);

            InitComponent<Floater>(part);

            if( part.horsepower > 0 ){
                InitComponent<Engine>(part);
            }
            if( part.propellerSpin > 0 ){
                propellers.Add(InitComponent<Propeller>(part));
            }
            if( part.isRudder ){
                rudders.Add(InitComponent<Rudder>(part));
            }
            if( part.firepower > 0 ){
                weapons.Add(InitComponent<Weapon>(part));
            }
        }
    }

    //initialise the components easier
    public T InitComponent<T >(ShipPart part) where T: Component, ShipMechanism
    {
        T component = part.AddComponent<T>();
        component.parentShip = this;
        component.shipRb = shipRb;
        component.part = part;
        return component;
    }

    void FixedUpdate()
    {
        //Drag
        shipRb.AddForce( shipRb.velocity * -5.0f );
        shipRb.AddTorque( shipRb.angularVelocity* -15f );
    }

    public void fireAt(Vector3 locale){
        foreach(Weapon weapon in weapons){
            weapon.AimAt(locale);
        }
    }
}