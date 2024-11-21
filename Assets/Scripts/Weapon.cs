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
    public FloatingShip parentShip{get; set;}
    public Rigidbody shipRb {get; set;}
    public ShipPart part {get; set;}
    public Transform turret;
    public Transform barrels;
    public float muzzleVelocity = 100;

    private int barrelCycle = 0;
    private float[] barrelLastFired;

    void Start()
    {
        turret = part.transform.GetChild(1).GetChild(1);
        barrels = turret.GetChild(1);

        barrelLastFired = new float[part.muzzles.Length];
        Array.Fill<float>(barrelLastFired,Time.time-part.reloadTime);
    }

    public void AimAt(Vector3 target){

        //Check what can fire.
        for(int i = 0; i < barrelLastFired.Length; i++){
            if(Time.time - barrelLastFired[i] >= part.reloadTime){
                barrelCycle = i;
                break;
            } else { barrelCycle = -1;}
        }
        if(barrelCycle==-1){return;}

        target -= barrels.transform.position;
        
        //ROTATION
        //Finding angle between the normal and the target
        Vector3 yaw = this.transform.InverseTransformDirection(target); yaw.y = 0;

        //ELEVATION
        //Probably use a more naive approach for elevation to not fuck with 
        Vector3 elevation = new Vector3(
            0.0f,
            Vector3.ProjectOnPlane(target, yaw).y, //target's direct elevation to barrel
            Mathf.Sqrt(Mathf.Pow(target.x,2) + Mathf.Pow(target.z,2)) //distance to target.
            ); //This is basically a 2d vector to the target from the barrel. Used to calculate range.
        
        //calculating the launch direction vector
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

        //Debug.Log("elevation : "+angle);

        if(!float.IsNaN(angle)){
            elevation.y = elevation.z*Mathf.Tan(-angle);

            //Check that aim isn't obstructed by self.
            //TODO: optimise this
            Boolean obstructed = false;
            Ray obstructionRay = new Ray(barrels.position, transform.rotation * new Vector3(yaw.x,elevation.y,yaw.z));
            RaycastHit[] hits = Physics.RaycastAll(obstructionRay, 50.0f);
            foreach( RaycastHit hit in hits ){
                // Debug.Log(hit.collider.transform.parent.parent.transform + " against " + this.transform + " : " + hit.collider.transform.parent.parent.transform.Equals(this.transform));

                Floater sp = hit.collider.GetComponentInParent<Floater>();
                if(sp != null){
                    if(! sp.Equals(GetComponent<Floater>()) && sp.GetComponentInParent<FloatingShip>().Equals(parentShip)){
                        obstructed = true;
                        
                        break;   
                    }
                }
            }

            Debug.DrawRay(barrels.position, transform.rotation * new Vector3(yaw.x,elevation.y,yaw.z), Color.red,5.0f);

            if(!obstructed){
                Train(yaw, elevation);
                Fire();
                DebugFire( target );
            }
        }
    }

    private Boolean ParentMatchRecursive(Transform t, Transform to){
        if(t.Equals(to)) return true;
        else if(t.parent != null) return ParentMatchRecursive(t.parent, to);
        else return false;
    }

    private void Train( Vector3 heading, Vector3 elevation ){
        turret.transform.localRotation = Quaternion.LookRotation(heading);
        barrels.localRotation = Quaternion.LookRotation(elevation);
    }

    private void Fire(){
        //Spawn muzzle
        GameObject muzzle = Resources.Load<GameObject>("Particles/MuzzleParticle");
        Instantiate(muzzle,
            barrels.transform.position + barrels.transform.rotation * part.muzzles[barrelCycle],
            barrels.transform.rotation);

        //Spawn projectile
        GameObject projectile = Resources.Load<GameObject>("Prefabs/Projectiles/shell");
        Projectile projscript = projectile.GetComponent<Projectile>();
        projscript.parent = parentShip;
        projscript.initialVelocity = barrels.transform.forward*muzzleVelocity;
        projscript.damage = part.firepower;
        projectile.transform.localScale = new Vector3(0.36f,0.36f,0.36f); //TODO:replace with caliber
        Instantiate(projectile,
            barrels.transform.position + barrels.transform.rotation * part.muzzles[barrelCycle],
            barrels.transform.rotation);
        
        //Apply reload
        barrelLastFired[barrelCycle] = Time.time;

        // barrelCycle = (barrelCycle+1)%part.muzzles.Length;
    }

    private void DebugFire( Vector3 target ){
        //Debug.DrawLine(barrels.transform.position,target,Color.white,10.0f);

        Vector3 velocity = barrels.transform.forward * muzzleVelocity;
        //Debug.DrawLine(barrels.transform.position,barrels.transform.forward*5,Color.white,10.0f);
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
