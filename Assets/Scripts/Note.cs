using UnityEngine;
using UnityEngine.InputSystem;

public class Note : MonoBehaviour
{
    public MeshRenderer[] meshRenderers;
    private Material[] originalMaterials;
    public Material highlightMaterial;
    private Camera playerCameraPosition;
    public float highlightDistance = 5f;
    private PlayerLook player;
    private bool isLookedAt = false;

    public InputActionReference interactAction;

    void Start()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        originalMaterials = new Material[meshRenderers.Length];
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            originalMaterials[i] = meshRenderers[i].material;
        }

        player = FindAnyObjectByType<PlayerLook>();
        playerCameraPosition = player.GetComponentInChildren<Camera>();

        interactAction.action.performed += OnInteract;
        interactAction.action.Enable();
    }

    void OnDestroy()
    {
        if (interactAction != null)
        {
            interactAction.action.performed -= OnInteract;
        }
    }

    void Update()
    {
        Ray ray = new Ray(playerCameraPosition.transform.position, playerCameraPosition.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, highlightDistance))
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                isLookedAt = true;
                SetLookedAt(true);
            }
            else
            {
                isLookedAt = false;
                SetLookedAt(false);
            }
        }
        else
        {
            isLookedAt = false;
            SetLookedAt(false);
        }
    }

    void SetLookedAt(bool value)
    {
        if (value)
        {
            foreach (MeshRenderer mr in meshRenderers)
                mr.material = highlightMaterial;
        }
        else
        {
            for (int i = 0; i < meshRenderers.Length; i++)
                meshRenderers[i].material = originalMaterials[i];
        }
    }

    void OnInteract(InputAction.CallbackContext context)
    {
            Destroy(this.gameObject);
    }
}