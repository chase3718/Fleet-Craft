using UnityEngine;

public class Projectile : MonoBehaviour
{
    public FloatingShip parent;
    public Rigidbody projectileRB;

    public Vector3 initialVelocity;

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

        if(this.gameObject.transform.position.y < -10.0){
            Destroy(this);
            Debug.Log("Projectile destroyed");
        }
    }
}
