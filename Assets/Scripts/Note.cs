using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Note : MonoBehaviour
{
    public Material highlightMaterial;
    public TMP_Text areYouSureText;
    public InputActionReference collectActionReference;

    private MeshRenderer[] meshRenderers;
    private Material[] originalMaterials;
    private float lookRange = 3f;

    private PlayerLook player;
    private Camera playerCamPosition;
    private bool isLookedAt = false;

    void Start()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        originalMaterials = new Material[meshRenderers.Length];
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            originalMaterials[i] = meshRenderers[i].material;
        }
        player = FindAnyObjectByType<PlayerLook>();
        playerCamPosition = player.GetComponentInChildren<Camera>();

    }

    void Update()
    {
        CheckIfLookingAtNote();
        CollectNote();
    }

    void CheckIfLookingAtNote()
    {
        Ray ray = new Ray(playerCamPosition.transform.position, playerCamPosition.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, lookRange) && hit.collider.gameObject == this.gameObject)
        {
            if (!isLookedAt)
            {
                isLookedAt = true;
                areYouSureText.gameObject.SetActive(true);
                IsLookedAt(true);
            }
        }
        else
        {
            if (isLookedAt)
            {
                isLookedAt = false;
                areYouSureText.gameObject.SetActive(false);
                IsLookedAt(false);
            }
        }
    }

    public void CollectNote()
    {
        if (isLookedAt && collectActionReference.action.WasPressedThisFrame())
        {
            if (areYouSureText) 
            {
                areYouSureText.gameObject.SetActive(false);
            }
            GameManager.Instance.AddNote();
            Destroy(gameObject);
        }
    }

    public void IsLookedAt(bool isLookAt)
    {
        isLookedAt = isLookAt;
        if (isLookedAt)
        {
            foreach (MeshRenderer mr in meshRenderers)
            {
                mr.material = highlightMaterial;
            }
        }
        else
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                meshRenderers[i].material = originalMaterials[i];
                areYouSureText.gameObject.SetActive(false);
            }
        }
    }
}
