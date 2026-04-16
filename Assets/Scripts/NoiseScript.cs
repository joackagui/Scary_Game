using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class NoiseScript : MonoBehaviour
{

    public RawImage noiseImage;
    public Transform slenderTransform;
    public VideoPlayer noiseVideoPlayer;
    private float currentNoiseAlpha = 0f;
    private float maxNoiseAlpha = 0.8f;
    private float noiseIncreaseRate = 0.6f;
    private float noiseDecreaseRate = 0.4f;
    private float viewThreshold = 0.85f;
    private Camera mainCamera;
    
    void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            enabled = false;
            return;
        }
    
        if (noiseImage == null || slenderTransform == null || noiseVideoPlayer == null)
        {
            enabled = false;
            return;
        }
        Color color = noiseImage.color;
        color.a = 0f;
        noiseImage.color = color;
    }

    void Update()
    {
        if (checkIfSlenderInView() && slenderTransform.GetComponentInChildren<SkinnedMeshRenderer>().enabled)
        {
            currentNoiseAlpha += noiseIncreaseRate * Time.deltaTime;
        }
        else
        {
            currentNoiseAlpha -= noiseDecreaseRate * Time.deltaTime;
        }
        currentNoiseAlpha = Mathf.Clamp01(currentNoiseAlpha);
        ApplyNoiseEffect();
    }

    bool checkIfSlenderInView()
    {
        Vector3 toSlender = (slenderTransform.position - mainCamera.transform.position).normalized;
        Vector3 cameraForward = mainCamera.transform.forward;
        float dot = Vector3.Dot(cameraForward, toSlender);
        return dot > viewThreshold;
    }

    void ApplyNoiseEffect()
    {
        float finalAlpha = currentNoiseAlpha * maxNoiseAlpha;
        Color color = noiseImage.color;
        color.a = finalAlpha;
        noiseImage.color = color;
    }
}
