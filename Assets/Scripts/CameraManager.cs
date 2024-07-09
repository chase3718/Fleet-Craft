using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class CameraManager : MonoBehaviour
{
    Ship ship;
    DockActions dockActions;
    InputAction movement;
    InputAction shift;
    InputAction rise;
    InputAction touch1;
    InputAction touch2;
    InputAction touch2Contact;
    Transform camTransform;

    // Panning
    [SerializeField]
    float panDefaultSpeed = 18f;
    [SerializeField]
    float panMaxSpeed = 30f;
    float xzPanSpeed;
    float yPanSpeed;
    [SerializeField]
    float panAcceleration = 20f;
    [SerializeField]
    float panDamping = 10f;

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

    Vector3 velocity;

    public Vector3 lastPosition;
    Vector3 targetPosition;

    Vector3 startDragPos;

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
        movement = dockActions.Movement.Pan;
        rise = dockActions.Movement.Rise;
        shift = dockActions.Movement.Shift;
        touch1 = dockActions.Movement.TouchPoint;
        touch2 = dockActions.Movement.TouchPoint2;
        touch2Contact = dockActions.Movement.Touch2Contact;
        dockActions.Movement.Zoom.performed += ZoomCamera;
        dockActions.Movement.Rotate.performed += RotateCamera;
        dockActions.Movement.TouchPoint2.performed += GetTwoTouchMovement;
        dockActions.Movement.Enable();
    }

    void OnDisable()
    {
        dockActions.Movement.Zoom.performed += ZoomCamera;
        dockActions.Movement.Rotate.performed -= RotateCamera;
        dockActions.Movement.TouchPoint2.performed -= GetTwoTouchMovement;

        dockActions.Movement.Disable();
    }

    void Update()
    {
        GetHorizontalMovement();
        GetVerticalMovement();
        UpdateVelocity();
        UpdateBasePosition();
    }

    void UpdateVelocity()
    {
        velocity = (transform.position - lastPosition) / Time.deltaTime;

        lastPosition = transform.position;
    }

    void GetHorizontalMovement()
    {
        Vector3 inputValue = movement.ReadValue<Vector2>().x * GetCameraRight()
                           + movement.ReadValue<Vector2>().y * GetCameraForward();

        inputValue = inputValue.normalized;

        if (inputValue.sqrMagnitude > 0.1f)
        {
            targetPosition += inputValue;
        }
    }

    void GetVerticalMovement()
    {
        Vector3 inputValue = Vector3.up * rise.ReadValue<float>();

        if (inputValue.sqrMagnitude > 0.1f)
        {
            targetPosition += inputValue;
        }
    }

    float lastDistance;
    void GetTwoTouchMovement(InputAction.CallbackContext context)
    {
        TouchState input1 = touch1.ReadValue<TouchState>();
        TouchState input2 = touch2.ReadValue<TouchState>();


        float angle = Vector2.Angle(input1.delta, input2.delta);


        Vector2 delta = input1.delta + input2.delta;
        float xDelta = delta.x;
        float yDelta = delta.y;

        Vector3 horizValue = xDelta * GetCameraRight();
        Vector3 vertiValue = Vector3.up * yDelta;

        Vector3 inputValue = -Vector3.Normalize(horizValue + vertiValue) / 3;

        if (inputValue.sqrMagnitude > 0.1f)
        {
            transform.position += inputValue;
            lastPosition = transform.position;
        }



        if (angle > 60)
        {
            float input = Vector2.Distance(input1.position, input2.position) - lastDistance;

            float zoom = -input * zoomSpeed * 10 * Time.deltaTime;
            zoomAmount = Mathf.Clamp(camTransform.localPosition.z - zoom, -zoomMax, -zoomMin);
            camTransform.localPosition = new Vector3(0, 0, zoomAmount);

        }
        lastDistance = Vector2.Distance(input1.position, input2.position);
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

    void UpdateBasePosition()
    {
        if (targetPosition.sqrMagnitude > 0.1f)
        {
            float speed = shift.ReadValue<float>() == 1 ? panMaxSpeed : panDefaultSpeed;
            xzPanSpeed = Mathf.Lerp(xzPanSpeed, speed, panAcceleration * Time.deltaTime);
            transform.position += targetPosition * xzPanSpeed * Time.deltaTime;
        }
        else
        {
            velocity = Vector3.Lerp(velocity, Vector3.zero, panDamping * Time.deltaTime);
            transform.position += velocity * Time.deltaTime;
        }
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, ship.minX - 10, ship.maxX + 10), Mathf.Clamp(transform.position.y, ship.minY - 10, ship.maxY + 10), Mathf.Clamp(transform.position.z, ship.minZ - 10, ship.maxZ + 10));
        targetPosition = Vector3.zero;
    }

    void RotateCamera(InputAction.CallbackContext context)
    {
        if (touch2Contact.ReadValue<float>() == 1)
        {
            return;
        }
        Vector2 delta = context.ReadValue<Vector2>();

        float xRot = delta.y * rotationSpeed * 10 * Time.deltaTime;
        float yRot = delta.x * rotationSpeed * 10 * Time.deltaTime;

        transform.Rotate(Vector3.up, yRot, Space.World);
        transform.Rotate(Vector3.right, -xRot, Space.Self);

        float x = transform.eulerAngles.x;
        if (x > 180)
        {
            x -= 360;
        }

        x = Mathf.Clamp(x, -80, 80);

        transform.eulerAngles = new Vector3(x, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    void ZoomCamera(InputAction.CallbackContext context)
    {
        float input = -context.ReadValue<Vector2>().y;

        float zoom = input * zoomSpeed * Time.deltaTime;
        zoomAmount = Mathf.Clamp(camTransform.localPosition.z - zoom, -zoomMax, -zoomMin);

        camTransform.localPosition = new Vector3(0, 0, zoomAmount);
    }

}
