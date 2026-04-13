using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioClip level1Music;
    public AudioClip footstepsClip; // ← Asigna tu clip de pasos en el Inspector

    private AudioSource audioSource;
    private AudioSource footstepsSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        footstepsSource = gameObject.AddComponent<AudioSource>();
        footstepsSource.loop = true;
        footstepsSource.playOnAwake = false;
        footstepsSource.clip = footstepsClip;
        footstepsSource.volume = 0.5f;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "Level1":
                audioSource.volume = 1f;
                PlayMusic(level1Music);
                break;
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (audioSource.clip == clip) return;
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void StartFootsteps()
    {
        if (footstepsClip == null || footstepsSource.isPlaying) return;
        footstepsSource.Play();
    }

    public void StopFootsteps()
    {
        if (footstepsSource.isPlaying)
            footstepsSource.Stop();
    }
    public void SetFootstepsPitch(float pitch)
    {
        footstepsSource.pitch = pitch;
    }
}