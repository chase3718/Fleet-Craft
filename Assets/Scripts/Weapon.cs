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
    public float muzzleVelocity = 10;

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
        //Debug.Log(Vector3.ProjectOnPlane(target, barrels.transform.forward));
        Vector3 elevation = new Vector3(
            0.0f,
            Vector3.ProjectOnPlane(target, turret.transform.forward).y,
            Mathf.Sqrt(Mathf.Pow(target.x,2) + Mathf.Pow(target.z,2))
            ); //This is basically a 2d vector to the target from the barrel. Used to calculate range.
        
        //calculating the launch direction vector
        // float angle = Mathf.Asin( ((Mathf.Pow(muzzleVelocity,2)/elevation.z)+2*elevation.y)/(elevation.z+2*elevation.y) );
        // elevation.y = 0 + elevation.z*Mathf.Tan(Mathf.Deg2Rad*angle);

        float grav = Physics.gravity.y;
        float angle = Mathf.Atan(
            ( 
                Mathf.Pow(muzzleVelocity,2)-
                Mathf.Abs(
                    Mathf.Sqrt(Mathf.Pow(muzzleVelocity,4)-
                    grav*(grav*Mathf.Pow(elevation.z,2) + 2*(-elevation.y)*Mathf.Pow(muzzleVelocity,2))
                ))
            )/(grav*elevation.z)
            );

        Debug.Log("elevation : "+angle);

        if(!float.IsNaN(angle)){
            elevation.y = elevation.z*Mathf.Tan(-angle);
            barrels.localRotation = Quaternion.LookRotation(elevation);
            
            Fire();
            DebugFire( target );
            
        }
    }

    private void Fire(){
        GameObject projectile = Resources.Load<GameObject>("Prefabs/Projectiles/shell");
        Projectile projscript = projectile.GetComponent<Projectile>();
        projscript.parent = ship;
        projscript.initialVelocity = barrels.transform.forward*muzzleVelocity;
        projectile.transform.localScale = new Vector3(0.36f,0.36f,0.36f); //TODO:replace with caliber
        Instantiate(projectile,barrels.transform.position,barrels.transform.rotation);
    }

    private void DebugFire( Vector3 target ){
        Debug.DrawLine(barrels.transform.position,target,Color.white,10.0f);
        Vector3 velocity = barrels.transform.forward * muzzleVelocity;
        Debug.DrawLine(barrels.transform.position,barrels.transform.position+velocity*5,Color.white,10.0f);

        Vector3 previous = barrels.transform.position;
        Vector3 newPos = previous;

        float timescale = 0.1f;

        for( int i = 0; i < 20; i++ ){
            newPos += velocity * timescale;
            newPos.y += Physics.gravity.y/2 * timescale * timescale;
            velocity.y += Physics.gravity.y * timescale;

            Debug.DrawLine(newPos,previous,Color.white,10.0f);

            previous = newPos;
            Debug.Log("fired");
        }
    }
}
