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

    private Boolean panning = false; //thread safety. Avoids race conditions of below
    Vector2 prevAngle;
        
    Vector2 downMousePos;
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
        HandleMouseClick();
    }
    void FixedUpdate(){
        UpdateFocusPos();

        if( ManualRotation() ){
            ConstraintAngles();
            transform.rotation = Quaternion.Euler(orbitAngles);
        }
        Vector3 lookPos = focusPoint - transform.rotation * Vector3.forward * distance;
        transform.position = lookPos;
    }
    void HandleMouseClick(){
        if(Input.GetMouseButtonDown(1)){
            downMousePos = Input.mousePosition;
            panning = true;
        }
        if(Input.GetMouseButtonUp(1)){
            prevAngle = orbitAngles;
            panning = false;
        }
    }

    bool ManualRotation(){
        if(panning){
            Vector2 input = new Vector2(
                downMousePos.y - Input.mousePosition.y,
                - (downMousePos.x - Input.mousePosition.x)
            );
            float e = 0.001f;
            if( input.magnitude > e){
                orbitAngles = prevAngle + rotationSpeed * Time.unscaledDeltaTime * input;
                return true;
            }
        }
        return false;
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

    void ConstraintAngles(){
        orbitAngles.x = Mathf.Clamp(orbitAngles.x, minVertical, maxVertical);
        if(orbitAngles.y < 0f){
            orbitAngles.y+=360f;
        } else if (orbitAngles.y >= 360f){
            orbitAngles.y-=360f;
        }
    }

}
