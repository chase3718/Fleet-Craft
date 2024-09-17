using System;
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
        Debug.DrawLine(turret.transform.position,turret.transform.position+turret.transform.up);
        Debug.DrawLine(turret.transform.position,turret.transform.position+turret.transform.forward);
        if( Input.GetMouseButtonDown(0) ){
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
        //Physics.Raycast(ray, out hit, math.INFINITY, layerMask: ( 1<<8 )
        //switch out the above, can't get it to detect water.
            if( Physics.Raycast(ray, out hit, math.INFINITY, layerMask: ( 1<<8 ) ) ){
                if( part != null){ //for whatever reason part.position is sometimes null
                    Train( hit );

                    //firings
                    //GameObject.CreatePrimitive( PrimitiveType.Sphere );

                }
            }
        }
    }
    private void Train(RaycastHit hit){
        
        Debug.DrawLine( part.position, hit.point );

        //ROTATION
        //Finding angle between the normal and the target
        Vector3 target = hit.point - barrels.transform.position;

        //We want to project target via the y axis onto a plane formed by turret.transform.forward, turret.transform.right
        Vector3 yaw = Vector3.ProjectOnPlane( target, turret.transform.up );
        yaw = Vector3.ProjectOnPlane( yaw, Vector3.up);
        turret.transform.localRotation = Quaternion.LookRotation(yaw);

        //Probably use a more naive approach for elevation to not fuck with 

        Debug.Log(Vector3.ProjectOnPlane(target, barrels.transform.forward));
        Vector3 elevation = new Vector3(
            0,
            Vector3.ProjectOnPlane(target, turret.transform.forward).y,
            Mathf.Sqrt(Mathf.Pow(target.x,2) + Mathf.Pow(target.z,2))
            );

        //elevation = Vector3.ProjectOnPlane(elevation, turret.transform.right); //change target to target.y or somethign
        barrels.localRotation = Quaternion.LookRotation(elevation);
    }
}
