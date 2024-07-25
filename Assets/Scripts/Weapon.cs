using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour, ShipMechanism
{
    public Ship ship {get; set;}
    public Rigidbody shipRb {get; set;}
    public ShipPart part {get; set;}
    public Transform turret;
    public Transform barrels;

    void Start()
    {
        turret = part.transform.GetChild(1).GetChild(1);
        barrels = turret.GetChild(1);
    }

    void Update()
    {

        if( Input.GetMouseButtonDown(0) ){
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
        //Physics.Raycast(ray, out hit, math.INFINITY, layerMask: ( 1<<8 )
        //switch out the above, can't get it to detect water.
            if( Physics.Raycast(ray, out hit ) ){
                if( part != null){ //for whatever reason part.position is sometimes null
                    Debug.Log("boom, "+hit.transform.name+", "+hit.point);
                    Debug.DrawLine( part.position, hit.point );
                    
                    //targetting. Really scuffed code but it workss
                    Vector3 target = hit.point - part.position;
                    Vector3 yaw = target;
                        yaw.y = 0;
                    Vector3 elevation = target;
                        elevation.x = yaw.x;
                    
                    turret.rotation = Quaternion.LookRotation(yaw);
                    barrels.rotation = Quaternion.LookRotation(elevation);


                    //firings
                }
            }
        }
    }
}
