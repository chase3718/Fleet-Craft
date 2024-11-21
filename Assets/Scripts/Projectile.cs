using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public FloatingShip parent;
    public Rigidbody projectileRB;

    public Vector3 initialVelocity;
    public float damage;

    private Boolean splashed = false;
    private 

    void Start(){
        projectileRB.detectCollisions = false;
        projectileRB.AddForce(initialVelocity,ForceMode.VelocityChange);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        projectileRB.AddForce(Physics.gravity);
        this.gameObject.transform.localRotation = Quaternion.LookRotation(projectileRB.velocity.normalized);

        //Altitude Based Checks
        if(this.gameObject.transform.position.y < 0.0 && !splashed){ //creates a splash
            GameObject splash = Resources.Load<GameObject>("Particles/WaterSplash");
            Instantiate(splash,
                transform.position, 
                Quaternion.LookRotation(Vector3.up)
                );
            splashed = true;
        }
        if(this.gameObject.transform.position.y < -10.0){
            Destroy(this.gameObject);
        }

        //Collision against ship.
        //Using a ray system as it's more reliable.
        Ray hitRay = new Ray(transform.position,transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(hitRay, 1.0f);
        foreach(RaycastHit hit in hits){
            Floater f = hit.collider.GetComponentInParent<Floater>();
            if(f!= null){
                //spawn dust
                GameObject splash = Resources.Load<GameObject>("Particles/DamageParticle");
                Instantiate(splash,
                    transform.position, 
                    Quaternion.LookRotation(Vector3.up)
                    );
                RaycastHit[] damageApply = Physics.SphereCastAll(new Ray(f.transform.position,f.transform.forward),damage/100);
                foreach( RaycastHit part in damageApply){
                    f = part.collider.GetComponentInParent<Floater>();
                    if(f != null){
                        f.Damage(damage);
                    }
                }
                Destroy(this.gameObject);
                return;
            }
        }
    }
}
