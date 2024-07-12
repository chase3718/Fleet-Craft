using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class CameraManager : MonoBehaviour
{
    Ship ship;
    DockActions dockActions;
    InputAction pan;
    InputAction shift;
    InputAction rise;
    InputAction touch1;
    InputAction touch2;
    InputAction touch1Contact;
    InputAction touch2Contact;
    Transform camTransform;

    // Panning
    [SerializeField]
    float panDefaultSpeed = 18f;
    [SerializeField]
    float panMaxSpeed = 30f;
    // Zooming
    [SerializeField]
    float zoomMin = 5f;
    [SerializeField]
    float zoomMax = 50f;
    [SerializeField]
    float zoomSpeed = 5f;

    // Rotation]
    [SerializeField]
    float rotationSpeed = 15f;

    public float zoomAmount;
    Vector3 movement;

    public Vector3 lastPosition;
    void Awake()
    {
        ship = FindObjectOfType<Ship>();
        dockActions = new DockActions();
        camTransform = GetComponentInChildren<Camera>().transform;
        zoomAmount = -ship.maxZ - 10;
    }

    void OnEnable()
    {
        zoomAmount = camTransform.localPosition.z;
        camTransform.LookAt(transform);

        lastPosition = transform.position;
        pan = dockActions.Movement.Pan;
        rise = dockActions.Movement.Rise;
        shift = dockActions.Movement.Shift;
        touch1 = dockActions.Movement.TouchPoint;
        touch2 = dockActions.Movement.TouchPoint2;
        touch1Contact = dockActions.Movement.Touch1Contact;
        touch2Contact = dockActions.Movement.Touch2Contact;
        dockActions.Movement.Zoom.performed += ZoomCamera;
        dockActions.Movement.Rotate.performed += RotateCamera;
        dockActions.Movement.Enable();
    }

    void OnDisable()
    {
        dockActions.Movement.Zoom.performed += ZoomCamera;
        dockActions.Movement.Rotate.performed -= RotateCamera;

        dockActions.Movement.Disable();
    }

    void Update()
    {
        GetHorizontalMovement();
        GetVerticalMovement();
        ApplyMovement();
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, ship.minX - 2, ship.maxX + 2), Mathf.Clamp(transform.position.y, ship.minY - 2, ship.maxY + 2), Mathf.Clamp(transform.position.z, ship.minZ - 2, ship.maxZ + 2));
    }

    void ApplyMovement()
    {
        float mod = shift.ReadValue<float>() == 1 ? panMaxSpeed : panDefaultSpeed;
        transform.position += movement * Time.deltaTime * mod;
        movement = Vector3.zero;
    }

    void GetHorizontalMovement()
    {
        Vector3 inputValue;
        if (touch2Contact.ReadValue<float>() == 1)
        {
            Vector3 touch1delta = touch1.ReadValue<TouchState>().delta;
            Vector3 touch2delta = touch2.ReadValue<TouchState>().delta;

            bool sameXDir = Mathf.Sign(touch1delta.x) == Mathf.Sign(touch2delta.x);
            if (sameXDir)
            {
                inputValue = new Vector3(-touch1delta.x, 0, 0);
                inputValue *= 19.2f / Screen.width;
                inputValue = Quaternion.Euler(0, transform.eulerAngles.y, 0) * inputValue;

                movement += inputValue;
            }
        }
        else
        {

            inputValue = pan.ReadValue<Vector2>().x * GetCameraRight()
                               + pan.ReadValue<Vector2>().y * GetCameraForward();


            movement += inputValue.normalized;
        }

    }

    void GetVerticalMovement()
    {
        Vector3 inputValue;
        if (touch2Contact.ReadValue<float>() == 1)
        {
            Vector3 touch1delta = touch1.ReadValue<TouchState>().delta;
            Vector3 touch2delta = touch2.ReadValue<TouchState>().delta;

            bool sameYDir = Mathf.Sign(touch1delta.y) == Mathf.Sign(touch2delta.y);
            if (sameYDir)
            {
                inputValue = new Vector3(0, -touch1delta.y, 0);
                inputValue *= 10.8f / Screen.height;
                inputValue = Quaternion.Euler(transform.eulerAngles.x, 0, 0) * inputValue;
                movement += inputValue;
            }
        }
        else
        {

            inputValue = Vector3.up * rise.ReadValue<float>();
            movement += inputValue.normalized;
        }
    }


    Vector3 GetCameraRight()
    {
        Vector3 right = camTransform.right;
        right.y = 0;
        return right;
    }

    Vector3 GetCameraForward()
    {
        Vector3 forward = camTransform.forward;
        forward.y = 0;
        return forward;
    }


    void RotateCamera(InputAction.CallbackContext context)
    {
        if (touch2Contact.ReadValue<float>() == 1)
        {
            return;
        }

        Vector2 delta = context.ReadValue<Vector2>();
        print(delta);
        if (delta.magnitude < 0.1f)
        {
            return;
        }

        delta *= new Vector3(1.92f / Screen.width, 1.08f / Screen.height, 0);


        float xRot = delta.y * rotationSpeed * 1000 * Time.deltaTime;
        float yRot = delta.x * rotationSpeed * 1000 * Time.deltaTime;

        transform.Rotate(Vector3.up, yRot, Space.World);
        transform.Rotate(Vector3.right, -xRot, Space.Self);

        float x = transform.eulerAngles.x;
        if (x > 180)
        {
            x -= 360;
        }

        x = Mathf.Clamp(x, -80, 80);

        transform.eulerAngles = new Vector3(x, transform.eulerAngles.y, 0);
    }

    void ZoomCamera(InputAction.CallbackContext context)
    {
        if (touch2Contact.ReadValue<float>() == 1)
        {
            Vector3 touch1Delta = touch1.ReadValue<TouchState>().delta;
            Vector3 touch2Delta = touch2.ReadValue<TouchState>().delta;

            bool sameYDir = Mathf.Sign(touch1Delta.y) == Mathf.Sign(touch2Delta.y);
            bool sameXDir = Mathf.Sign(touch1Delta.x) == Mathf.Sign(touch2Delta.x);
            if (!sameYDir || !sameXDir)
            {
                if (Mathf.Abs(touch1Delta.x) > Mathf.Abs(touch1Delta.y))
                {
                    float input = touch1Delta.x;
                    float pan = input * panDefaultSpeed * Time.deltaTime;
                    transform.position += transform.right * pan * 19.2f / Screen.width;
                }
                else
                {
                    float input = touch1Delta.y;
                    float pan = input * panDefaultSpeed * Time.deltaTime;
                    transform.position += transform.forward * pan * 10.8f / Screen.height;
                }


            }
        }
        else
        {

            float input = -context.ReadValue<Vector2>().y;



            float zoom = input * zoomSpeed * Time.deltaTime;
            zoomAmount = Mathf.Clamp(camTransform.localPosition.z - zoom, -zoomMax, -zoomMin);

            camTransform.localPosition = new Vector3(0, 0, zoomAmount);
        }
    }

    float Sign(float input)
    {
        return input > 0 ? 1 : -1;
    }
}
