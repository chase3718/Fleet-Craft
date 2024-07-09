using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PartPreviewManager : MonoBehaviour
{
    public PartPreviewManager instance;
    Transform camRig;
    GameObject previewObject;
    Camera previewCamera;
    Transform previewCameraRig;
    VisualElement previewUI;
    public Quaternion targetRotation;

    void Start()
    {
        instance = this;
        camRig = FindObjectOfType<CameraManager>().transform;
        previewCamera = GetComponentInChildren<Camera>();
        previewUI = FindObjectOfType<UIDocument>().rootVisualElement.Q<VisualElement>("PartPreview");
        print(previewUI.name);
        // Add camera view to the UI
        RenderTexture cameraTexture = new RenderTexture(256, 256, 24);
        cameraTexture.autoGenerateMips = false;
        previewCamera.targetTexture = cameraTexture;
        Background previewBackground = new Background();
        previewBackground.renderTexture = cameraTexture;
        previewUI.style.backgroundImage = previewBackground;
        previewCameraRig = previewCamera.transform.parent;
    }

    void Update()
    {
        previewCameraRig.rotation = camRig.rotation;

        previewObject.transform.rotation = Quaternion.Slerp(previewObject.transform.rotation, targetRotation, Time.deltaTime * 10);
    }

    public GameObject SetPreviewObject(GameObject part)
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
        }
        if (previewCamera == null)
        {
            previewCamera = GetComponentInChildren<Camera>();
        }
        previewObject = Instantiate(Resources.Load<GameObject>(part.GetComponent<ShipPart>().prefabPath));
        previewObject.GetComponent<ShipPart>().SetBoxColliders();
        SetLayerRecursively(previewObject);
        previewObject.transform.SetParent(transform);
        previewObject.transform.localPosition = Vector3.zero;
        previewObject.transform.localRotation = part.transform.rotation;
        ShipPart shipPart = previewObject.GetComponent<ShipPart>();
        Vector3 zeroCollider = shipPart.boxColliders.Find(collider => collider.transform.localPosition == Vector3.zero).transform.localPosition;
        Vector3 centerOfMass = shipPart.centerOfMass;
        print(centerOfMass + " " + zeroCollider);
        previewObject.transform.localPosition = -centerOfMass + zeroCollider;
        previewCamera.orthographicSize = (float)shipPart.dimensions.z / 2 + 0.5f;
        return previewObject;
    }

    void SetLayerRecursively(GameObject obj)
    {
        obj.layer = LayerMask.NameToLayer("Preview");
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject);
        }
    }
}
