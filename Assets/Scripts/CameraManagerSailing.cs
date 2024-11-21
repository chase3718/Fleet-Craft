using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraManagerSailing : MonoBehaviour
{
    public Transform focus = default;
    public float distance = 25f;

    Vector3 focusPoint;
    public float focusRadius = 0.0f;

    [Range(0f, 1f)]
    public float focusCentering = 0.5f;

    Vector2 orbitAngles = new Vector2(45f,0f);

    Vector2 prevAngle;

    Vector2 prevMousePos;

    [Range(0f,360f)]
    public float rotationSpeed = 90f;

    [Range(-89f, 90f)]
    public float minVertical = -89f, maxVertical = 90f;

    void Start()
    {
        focusPoint = focus.position;
        transform.rotation = Quaternion.Euler(orbitAngles);
        prevAngle = orbitAngles;
    }

    // Fixed update because I want consistent camera movement
    void Update()
    {  
        UpdateFocusPos();
        HandleMouseClick();
    }
    void FixedUpdate(){
        Quaternion rot;
        if( ManualRotation() ){
            ConstraintAngles();
            //Debug.Log("Updated camera positon 1");
            rot = Quaternion.Euler(orbitAngles);
        } else{
            //TODO: some times 2 is ran when it is supposed to be 1, causing jitter.
            //Debug.Log("Updated camera positon 2");
            rot = transform.localRotation;
        }
        Vector3 lookPos = focusPoint - rot * Vector3.forward * distance;
        transform.SetPositionAndRotation(lookPos, rot);
    }

    void UpdateFocusPos(){
        float d = Vector3.Distance(focus.position,focusPoint);
        if( focusRadius > 0f && 
            d > focusRadius //focal object departed from the focus
        ){
            focusPoint = Vector3.Lerp(focus.position, focusPoint, focusRadius/d);
        }
        focusPoint = focus.position;
    }

    bool ManualRotation(){
        if(Input.GetMouseButton(1)){
            Vector2 input = new Vector2(
                prevMousePos.y - Input.mousePosition.y,
                - (prevMousePos.x - Input.mousePosition.x)
            );
            float e = 0.001f;
            if( Mathf.Abs(input.x) > e || Mathf.Abs(input.y) > e){
                orbitAngles = prevAngle + rotationSpeed * Time.unscaledDeltaTime * input;
            }
            return true;
        }
        return false;
    }

    void HandleMouseClick(){
        if(Input.GetMouseButtonDown(1)){
            prevMousePos = Input.mousePosition;
            Debug.Log("prev mouse pos assigned "+prevMousePos);
        }
        if(Input.GetMouseButtonUp(1)){
            prevAngle = orbitAngles;
            Debug.Log("prevAngle assigned"+prevAngle);
        }
    }

    void ConstraintAngles(){
        orbitAngles.x = Mathf.Clamp(orbitAngles.x, minVertical, maxVertical);
        if(orbitAngles.y < 0f){
            orbitAngles.y+=360f;
        } else if (orbitAngles.y >= 360f){
            orbitAngles.y-=360f;
        }
    }

}
