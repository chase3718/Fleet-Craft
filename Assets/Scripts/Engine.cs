using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour, ShipMechanism
{
    public Ship ship {get; set;}
    public Rigidbody shipRb {get; set;}
    public ShipPart part {get; set;}

    public void Start(  ){
        ship.enginePower += part.horsepower;
    }

    public void FixedUpdate(){
        //for damage
    }

    
}
