using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour, ShipMechanism
{
    public FloatingShip parentShip{get; set;}
    public Rigidbody shipRb {get; set;}
    public ShipPart part {get; set;}

    public void Start(  ){
        parentShip.enginePower += part.horsepower;
    }

    public void FixedUpdate(){
        //for damage
    }

    
}
