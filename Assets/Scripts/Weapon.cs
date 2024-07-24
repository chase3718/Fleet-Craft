using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour, ShipMechanism
{
    public Ship ship {get; set;}
    public Rigidbody shipRb {get; set;}
    public ShipPart part {get; set;}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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
                    
                    //targetting
                    Vector3 target = hit.point - part.position;
                    target.y = 0;
                    part.transform.rotation = Quaternion.LookRotation(target);

                    //firings
                }
            }
        }
    }
}
