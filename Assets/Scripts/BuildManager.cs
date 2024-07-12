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
    PartPreviewManager partPreview;
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
    ShipPart selectedPart;
    PartPreviewManager previewObject;

    void Awake()
    {
        ship = FindObjectOfType<DockedShip>();
        partPreview = FindObjectOfType<PartPreviewManager>();
        dockActions = new DockActions();
        SetGhostBlock(Resources.Load<ShipPart>("Prefabs/ShipParts/Hull/hull"));
        shift = dockActions.Build.Shift;
        dockActions.Build.Place.performed += HandleClick;
        dockActions.Build.Rotate.performed += RotateGhostBlock;
        camRig = FindObjectOfType<CameraManager>().transform;
        previewObject = FindObjectOfType<PartPreviewManager>();
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
            if (highlightedPart != null)
            {
                highlightedPart.Unhighlight();
                highlightedPart = null;
            }
            BuildMode();
        }
        else if (dockMode == DockMode.Delete)
        {
            DeleteMode();
        }
        else if (dockMode == DockMode.Select)
        {
            SelectMode();
        }

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
        partPreview.SetPreviewObject(ghostBlock.gameObject);

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
            ghostBlock.transform.Rotate(transform.up, Sign(dir.x) * 90, Space.World);
        }
        if (dir.y != 0 && !ghostBlock.lockVerticalRotation)
        {
            // Get the y rotation of the camera
            float yRot = camRig.rotation.eulerAngles.y;
            // Round the rotation to the nearest 90 degrees
            yRot = Mathf.Round(yRot / 90) * 90;
            if (yRot == 0 || yRot == 360 || yRot == 180)
            {
                ghostBlock.transform.Rotate(transform.right, Sign(dir.y) * 90, Space.World);
            }
            else if (yRot == 90 || yRot == 270)
            {
                ghostBlock.transform.Rotate(transform.forward, Sign(dir.y) * 90, Space.World);
            }
        }
        previewObject.targetRotation = ghostBlock.transform.rotation;
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
    }

    ShipPart highlightedPart = null;
    void DeleteMode()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, layerMask: (1 << 6)))
        {
            ShipPart part = hit.transform.parent.parent.GetComponent<ShipPart>();
            part.Highlight(Color.red);
            if (highlightedPart != null && highlightedPart != part)
            {
                highlightedPart.Unhighlight();
            }
            highlightedPart = part;

        }
        else
        {
            if (highlightedPart != null)
            {
                highlightedPart.Unhighlight();
                highlightedPart = null;
            }

        }
    }

    void SelectMode()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, layerMask: (1 << 6)))
        {
            ShipPart part = hit.transform.parent.parent.GetComponent<ShipPart>();
            part.Highlight(Color.yellow);
            if (highlightedPart != null && highlightedPart != part)
            {
                highlightedPart.Unhighlight();
            }
            highlightedPart = part;
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                selectedPart = part;
                dockMode = DockMode.Edit;
            }
        }
        else
        {
            if (highlightedPart != null)
            {
                highlightedPart.Unhighlight();
                highlightedPart = null;
            }
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

    void HandleClick(InputAction.CallbackContext context)
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
                ShipPart newBlock = PlaceGhostBlock();
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

    ShipPart PlaceGhostBlock()
    {
        ShipPart newBlock = Instantiate(Resources.Load(ghostBlock.prefabPath), ghostBlock.position, ghostBlock.transform.rotation).GetComponent<ShipPart>();
        newBlock.gameObject.SetActive(true);
        newBlock.name = newBlock.key;
        if (ship.AddPart(newBlock))
        {
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