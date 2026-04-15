using System.Collections;
using UnityEngine;

public class Slender : MonoBehaviour
{
    public UnityEngine.AI.NavMeshAgent navMeshAgent;
    public PlayerMovement player;
    public PlayerLook playerLook;
    public SkinnedMeshRenderer slenderMesh;
    public Animator slenderAnimator;

    [Header("Audio")]
    public AudioClip ambientLoop;
    public AudioClip jumpscareClip;
    public float maxHearDistance = 10f;
    public float minPitch = 0.9f; 
    public float maxPitch = 1.05f;   
    public float minVolume = 0.2f;   
    public float maxVolume = 1.0f;    

    private AudioSource spatialSource;
    private float baseSpeed = 0.5f;
    private bool isGameOver = false;
    private float catchDistance = 1.5f;

    [Header("Teleportation")]
    public Transform[] teleportDestinations;
    public float teleportTriggerDistance = 25f;
    public float minTeleportDestinationDistanceFromPlayer = 7f;

    private bool isTeleporting = false;
    private bool hasTeleportedBecauseTooFar = false;

    void Start()
    {
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        player = FindObjectOfType<PlayerMovement>();
        playerLook = player.GetComponentInChildren<PlayerLook>();
        slenderMesh = GetComponentInChildren<SkinnedMeshRenderer>();
        slenderAnimator = GetComponent<Animator>();
        navMeshAgent.speed = baseSpeed;

        spatialSource = gameObject.AddComponent<AudioSource>();
        spatialSource.clip         = ambientLoop;
        spatialSource.loop         = true;
        spatialSource.spatialBlend = 1f;
        spatialSource.rolloffMode  = AudioRolloffMode.Linear;
        spatialSource.minDistance  = 2f;
        spatialSource.maxDistance  = maxHearDistance;
        spatialSource.volume       = minVolume;
        spatialSource.dopplerLevel = 0f;
        spatialSource.playOnAwake  = false;
    }

    void Update()
    {
        if (isGameOver) return;

        if (slenderMesh.enabled && navMeshAgent.enabled)
        {
            navMeshAgent.SetDestination(player.transform.position);
            float currentVelocity = navMeshAgent.velocity.magnitude;
            slenderAnimator.SetFloat("speed", currentVelocity);
            checkPlayerDistance();
            UpdateAmbientAudio();
        }
        else
        {
            if (spatialSource.isPlaying)
                spatialSource.Stop();
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer > teleportTriggerDistance && !isTeleporting)
        {
            if (!isTeleporting && !hasTeleportedBecauseTooFar && slenderMesh.enabled)
                StartCoroutine(TeleportToDestination());
        }
        else
        {
            hasTeleportedBecauseTooFar = false;
        }

        ChangeDifficulty();
    }

    private void UpdateAmbientAudio()
    {
        if (!spatialSource.isPlaying)
            spatialSource.Play();

        float distance = Vector3.Distance(transform.position, player.transform.position);
        float t = 1f - Mathf.Clamp01(distance / maxHearDistance);

        // Both pitch AND volume scale with proximity for a much more natural feel
        spatialSource.pitch  = Mathf.Lerp(minPitch,  maxPitch,  t);
        spatialSource.volume = Mathf.Lerp(minVolume, maxVolume, t);
    }

    public void ChangeDifficulty()
    {
        int notesCount = GameManager.Instance.GetNotesCount();

        if (notesCount < 1)
        {
            slenderMesh.enabled  = false;
            navMeshAgent.enabled = false;
        }
        else
        {
            slenderMesh.enabled  = true;
            navMeshAgent.enabled = true;
        }

        if (notesCount >= 5)
        {
            slenderAnimator.SetFloat("speed", navMeshAgent.speed);
            navMeshAgent.speed = baseSpeed + (notesCount * 0.5f) + 1.0f;
        }
    }

    private void checkPlayerDistance()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= catchDistance)
            catchPlayer();
    }

    private void catchPlayer()
    {
        isGameOver = true;
        navMeshAgent.isStopped = true;
        navMeshAgent.velocity  = Vector3.zero;

        Vector3 lookAtPlayer = new Vector3(
            player.transform.position.x,
            transform.position.y,
            player.transform.position.z
        );
        transform.LookAt(lookAtPlayer);
        slenderAnimator.SetTrigger("jumpscare");
        playerLook.enabled = false;
        player.enabled     = false;
        playerLook.playerCamera.transform.LookAt(transform.position + Vector3.up * 2f);

        spatialSource.Stop();
        spatialSource.enabled = false;

        MusicManager.Instance.PlayJumpscare(jumpscareClip);

        GameManager.Instance.TriggerGameOver();
    }

    private IEnumerator TeleportToDestination()
    {
        isTeleporting = true;

        if (navMeshAgent.enabled)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
        }

        Transform selectedDestination = null;
        float bestDistanceToPlayer = float.MaxValue;

        for (int i = 0; i < teleportDestinations.Length; i++)
        {
            Transform destination = teleportDestinations[i];
            if (destination == null)
                continue;

            float distanceToPlayer = Vector3.Distance(player.transform.position, destination.position);
            if (distanceToPlayer < minTeleportDestinationDistanceFromPlayer)
                continue;

            if (distanceToPlayer < bestDistanceToPlayer)
            {
                bestDistanceToPlayer = distanceToPlayer;
                selectedDestination = destination;
            }
        }

        if (selectedDestination != null)
        {
            if (navMeshAgent.enabled)
            {
                navMeshAgent.Warp(selectedDestination.position);
            }
            else
            {
                transform.position = selectedDestination.position;
            }
        }

        hasTeleportedBecauseTooFar = true;
        yield return new WaitForSeconds(5f);

        if (navMeshAgent.enabled)
            navMeshAgent.isStopped = false;

        isTeleporting = false;
    }
}