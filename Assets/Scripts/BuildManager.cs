using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BuildManager : MonoBehaviour
{
    [SerializeField]
    DockedShip ship;
    DockActions dockActions;
    InputAction shift;
    InputAction pointerHeld;
    InputAction rotate;
    [SerializeField]
    ShipPart ghostBlock;
    Transform camRig;
    [SerializeField]
    float dragMultiplier = 75f;
    float timeOfLastPlace = 0;
    bool overUI = false;
    public DockMode dockMode = DockMode.Build;

    void Awake()
    {
        ship = FindObjectOfType<DockedShip>();
        dockActions = new DockActions();
        //Assets/Resources/Prefabs/ShipParts/Hull/hull.prefab
        SetGhostBlock(Resources.Load<ShipPart>("Prefabs/ShipParts/Hull/hull"));
        shift = dockActions.Build.Shift;
        dockActions.Build.Place.performed += PlaceBlock;
        dockActions.Build.DragPlacement.performed += DragAndPlaceBlock;
        dockActions.Build.Rotate.performed += RotateGhostBlock;
        camRig = FindObjectOfType<CameraManager>().transform;
    }

    public void SetGhostBlock(ShipPart part)
    {
        if (ghostBlock != null)
        {
            Destroy(ghostBlock.gameObject);
        }
        ghostBlock = part;
        ghostBlock = Instantiate(part, Vector3.up * 1000, part.transform.rotation);
        ghostBlock.transform.SetParent(transform);
        foreach (Renderer r in ghostBlock.GetComponentsInChildren<Renderer>())
        {
            List<Material> updatedMaterials = new List<Material>();
            foreach (Material mat in r.materials)
            {
                Material updatedMat = Instantiate(Resources.Load<Material>("Materials/GhostBlock"));
                updatedMat.color = new Color(mat.color.r, 1f, mat.color.b, 0.5f);
                updatedMaterials.Add(updatedMat);
            }
            r.materials = updatedMaterials.ToArray();
        }
        ghostBlock.name = "GhostBlock";
    }

    void RotateGhostBlock(InputAction.CallbackContext context)
    {
        if (dockMode != DockMode.Build)
        {
            return;
        }
        Vector2 dir = context.ReadValue<Vector2>();
        if (dir.x != 0)
        {
            ghostBlock.transform.Rotate(transform.up, Sign(dir.x) * 90);
        }
        if (dir.y != 0 && !ghostBlock.lockVerticalRotation)
        {
            // Get the y rotation of the camera
            float yRot = camRig.rotation.eulerAngles.y;
            // Round the rotation to the nearest 90 degrees
            yRot = Mathf.Round(yRot / 90) * 90;
            if (yRot == 0 || yRot == 360 || yRot == 180)
            {
                ghostBlock.transform.Rotate(transform.right, Sign(dir.y) * 90);
            }
            else if (yRot == 90 || yRot == 270)
            {
                ghostBlock.transform.Rotate(transform.forward, Sign(dir.y) * 90);
            }
        }
    }

    void OnEnable()
    {
        dockActions.Build.Enable();
    }

    void OnDisable()
    {
        dockActions.Build.Disable();
    }

    void Update()
    {
        CheckIfOverUI();
        if (overUI)
        {
            return;
        }
        if (dockMode == DockMode.Build)
        {
            BuildMode();
        }
        else if (dockMode == DockMode.Delete)
        {
            DeleteMode();
        }

    }

    void BuildMode()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, layerMask: (1 << 6)))
        {
            ghostBlock.gameObject.SetActive(true);
            ghostBlock.transform.position = hit.transform.position + hit.normal;
            if (!ship.AvoidCollisions(ghostBlock.gameObject))
            {
                ghostBlock.gameObject.SetActive(false);

            }
        }
        else
        {
            ghostBlock.transform.position = Vector3.up * 1000;
            ghostBlock.gameObject.SetActive(false);
        }
        HandleLeftClick();
    }

    void DeleteMode()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, layerMask: (1 << 6)))
        {
            ShipPart part = hit.transform.parent.parent.GetComponent<ShipPart>();
            // part.Highlight();
        }
    }

    void CheckIfOverUI()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            overUI = true;

        }
        else
        {
            overUI = false;

        }
    }

    void PlaceBlock(InputAction.CallbackContext context)
    {
        if (overUI)
        {
            return;
        }
        if (dockMode == DockMode.Build)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, layerMask: (1 << 6)))
            {
                ShipPart newBlock = PlaceGhostBlock(hit.transform.position + hit.normal);
                if (newBlock != null && shift.ReadValue<float>() == 1)
                {
                    ship.ShiftPosition(newBlock, hit.normal);
                }
            }
        }
        else if (dockMode == DockMode.Delete)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, layerMask: (1 << 6)))
            {
                ShipPart part = hit.transform.parent.parent.GetComponent<ShipPart>();

                bool removed = ship.RemovePart(part);
                if (removed && shift.ReadValue<float>() == 1)
                {
                    ship.ShiftPosition(part, hit.normal);
                }
            }
        }

    }

    ShipPart partOnClick = null;
    ShipPart lastPlacedBlock = null;
    Vector3 startDragDelta = Vector3.zero;
    List<string> blocksPlacedFromDrag = new List<string>();
    Vector3 lastHitNormal;
    Vector3 lastHitPoint;
    bool dragStart = false;
    void DragAndPlaceBlock(InputAction.CallbackContext context)
    {
        if (overUI)
        {
            return;
        }
        if (lastPlacedBlock != null && dragStart)
        {
            if (startDragDelta == Vector3.zero)
            {
                startDragDelta = context.ReadValue<Vector2>();
            }
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, layerMask: (1 << 6)))
            {
                if (!blocksPlacedFromDrag.Contains(hit.transform.parent.parent.GetComponent<ShipPart>().key))
                {
                    if (PlaceGhostBlock(hit.transform.position + hit.normal) != null)
                    {
                        lastHitNormal = hit.normal;
                    }


                }
                lastHitPoint = hit.point;
            }
            else
            {
                //Todo: when no hit, change mode to straight line
                Vector3 lastHitSide = lastPlacedBlock.transform.position + (lastHitNormal / 2);
                Vector3 dragDirection = (lastHitPoint - lastHitSide).normalized;
                float xMag = Mathf.Abs(dragDirection.x);
                float yMag = Mathf.Abs(dragDirection.y);
                float zMag = Mathf.Abs(dragDirection.z);
                Vector3 Normal = Vector3.zero;
                if (xMag > yMag && xMag > zMag)
                {
                    Normal = new Vector3(Sign(dragDirection.x), 0, 0);
                }
                else if (yMag > xMag && yMag > zMag)
                {
                    Normal = new Vector3(0, Sign(dragDirection.y), 0);
                }
                else
                {
                    Normal = new Vector3(0, 0, Sign(dragDirection.z));
                }

                Vector3 dimensions = lastPlacedBlock.transform.rotation * lastPlacedBlock.dimensions;
                dimensions = new Vector3(Mathf.Abs(dimensions.x), Mathf.Abs(dimensions.y), Mathf.Abs(dimensions.z));
                Normal = new Vector3(Normal.x * dimensions.x, Normal.y * dimensions.y, Normal.z * dimensions.z);


                PlaceGhostBlock(lastPlacedBlock.transform.position + Normal);


            }
        }
    }

    ShipPart PlaceGhostBlock(Vector3 position)
    {
        ShipPart newBlock = Instantiate(Resources.Load(ghostBlock.prefabPath), position, ghostBlock.transform.rotation).GetComponent<ShipPart>();
        newBlock.gameObject.SetActive(true);
        newBlock.name = newBlock.key;
        if (ship.AddPart(newBlock))
        {
            lastPlacedBlock = newBlock;
            if (dragging)
            {
                blocksPlacedFromDrag.Add(newBlock.key);
            }
            lastPlacedBlock = newBlock;
            timeOfLastPlace = Time.time;
            return newBlock;

        }

        return null;

    }

    float Sign(float x)
    {
        if (x > 0)
        {
            return 1;
        }
        else if (x < 0)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }



    bool dragging = false;

    void HandleLeftClick()
    {
        if (Mouse.current.leftButton.IsPressed() && dragging && !dragStart)
        {

            dragStart = true;
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, layerMask: (1 << 6)))
            {
                if (PlaceGhostBlock(hit.transform.position + hit.normal))
                {
                    lastHitNormal = hit.normal;
                    partOnClick = hit.transform.parent.parent.GetComponent<ShipPart>();
                }
            }
        }
        else if (!Mouse.current.leftButton.isPressed && dragging)
        {
            partOnClick = null;
            startDragDelta = Vector3.zero;
            lastPlacedBlock = null;
            dragging = false;
            blocksPlacedFromDrag.Clear();
            dragStart = false;
        }
    }
}

public static class MaterialExtensions
{
    public static void ToOpaqueMode(this Material material)
    {
        material.SetOverrideTag("RenderType", "");
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        material.SetInt("_ZWrite", 1);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = -1;
    }

    public static void ToFadeMode(this Material material)
    {
        material.SetOverrideTag("RenderType", "Transparent");
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }
}

public enum DockMode
{
    Build,
    Edit,
    Select,
    Delete
}