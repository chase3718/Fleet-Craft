using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShip : MonoBehaviour
{
    FloatingShip ship;

    void Awake()
    {
        ship = GetComponent<FloatingShip>();
    }

    // Update is called once per frame
    void Update()
    {
        //Firing
        if( Input.GetMouseButtonDown(0) ){
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            Plane sea = new Plane(Vector3.up, Vector3.zero);
            float dist = 0.0f;
            if(sea.Raycast(ray, out dist)){
                Vector3 target = ray.GetPoint(dist);
                ship.fireAt( target );
            }
        }
        //Sailing
        if( Input.GetKey( "w" )){ 
            ship.throttle = 1.0f;
        } else if( Input.GetKey( "s" )){
            ship.throttle = -1.0f;
        } else if( ship.throttle != 0.0f ){  //TODO:remove this later.
            ship.throttle = 0.0f;
        }

        //Turning
        if( Input.GetKey( "a" ) ){
            ship.turn = -1;
        } else if( Input.GetKey( "d" ) ){
            ship.turn = 1;
        } else if (ship.turn != 0.0f){
            ship.turn = 0.0f;
        }
    }
}
