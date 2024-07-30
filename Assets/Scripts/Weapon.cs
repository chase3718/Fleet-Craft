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
            if( Physics.Raycast(ray, out hit, math.INFINITY, layerMask: ( 1<<8 ) ) ){
                if( part != null){ //for whatever reason part.position is sometimes null
                    Debug.Log("boom, "+hit.transform.name+", "+hit.point);
                    Debug.DrawLine( part.position, hit.point );
                    
                    //Creating a direction vector of turret entities
                    Vector3 turretDirection = turret.transform.forward;
                    turretDirection.y = 0;
                    Vector3 barrelElevation = Vector3.zero;
                    barrelElevation.x = 1;
                    barrelElevation.y = barrels.transform.forward.y;

                    //Finding angle between the normal and the target
                    Vector3 target = hit.point - barrels.transform.position;
                    Vector3 yaw = target;
                        yaw.y = 0;//target heading in 2d)
                    //pitch
                    Vector3 pitch = Vector3.zero;
                    pitch.x = target.magnitude;
                    pitch.y = target.y;

                    float yawAngle = Vector3.SignedAngle( turretDirection, yaw, turret.transform.up );
                    //Debug.Log( "target heading : " + yaw + " , turret heading: "+ turretDirection + " , yaw amount: " + yawAngle ); 

                    float pitchAngle = Vector3.SignedAngle(barrelElevation, pitch, barrels.transform.right );
                    Debug.Log( "target elevation :" + pitch + " , barrel elevation: " + barrelElevation + " , elevation amount: " + pitchAngle );

                    //rotate
                    turret.transform.Rotate( 0,yawAngle,0 );
                    //barrels.transform.Rotate( pitchAngle,0,0 );

                    //firings
                    //GameObject.CreatePrimitive( PrimitiveType.Sphere );

                }
            }
        }
    }
}
