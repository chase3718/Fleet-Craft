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
        Debug.Log(target);

        Vector3 yaw = target;
            yaw.y = 0;//target heading in 2d)
        turret.transform.localRotation = Quaternion.LookRotation(yaw);
        
        //ELEVATION
        target = hit.point - barrels.transform.position;
        Vector3 elevation = Vector3.zero;
        elevation.y = 1.0f;
        barrels.transform.localRotation = Quaternion.LookRotation( elevation );
        //Debug.Log( "Current elevation : "+barrels.transform.rotation.eulerAngles + ", target : "+elevation );
    }
    private void TrainDeprecated( RaycastHit hit ){
        Debug.Log("boom, "+hit.transform.name+", "+hit.point);
        Debug.DrawLine( part.position, hit.point );
        
        //Creating a direction vector of turret entities
        Vector3 turretDirection = turret.transform.forward;
        turretDirection.y = 0;
        
        //ROTATION
        //Finding angle between the normal and the target
        Vector3 target = hit.point - barrels.transform.position;
        Vector3 yaw = target;
            yaw.y = 0;//target heading in 2d)

        float yawAngle = Vector3.SignedAngle( turretDirection, yaw, turret.transform.up );
        //Debug.Log( "target heading : " + yaw + " , turret heading: "+ turretDirection + " , yaw amount: " + yawAngle ); 

        //rotate
        turret.transform.Rotate( 0,yawAngle,0 );
        


        //ELEVATION
        Vector3 barrelElevation = Vector3.zero;
        barrelElevation.x = 1;
        barrelElevation.y = barrels.transform.forward.y;

        //pitch                    
        Vector3 pitch = Vector3.zero;
        pitch.x = target.magnitude;
        pitch.y = target.y;

        float pitchAngle = Vector3.SignedAngle(barrelElevation, pitch, Vector3.back );
        Debug.Log( "target elevation :" + pitch + " , barrel elevation: " + barrelElevation + " , elevation amount: " + pitchAngle );

        //elevate
        barrels.transform.Rotate( pitchAngle,0,0 );
        
    }
}
