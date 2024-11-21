using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public FloatingShip parent;
    public Rigidbody projectileRB;

    public Vector3 initialVelocity;

    private Boolean splashed = false;

    void Start(){
        Debug.Log("Projectile spawned");
        projectileRB.detectCollisions = false;
        projectileRB.AddForce(initialVelocity,ForceMode.VelocityChange);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        projectileRB.AddForce(Physics.gravity);
        this.gameObject.transform.localRotation = Quaternion.LookRotation(projectileRB.velocity.normalized);

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
    }
}
