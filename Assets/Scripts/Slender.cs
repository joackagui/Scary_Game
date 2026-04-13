using UnityEngine;

public class Slender : MonoBehaviour
{
    
   public UnityEngine.AI.NavMeshAgent navMeshAgent;
   public PlayerMovement player;
   public SkinnedMeshRenderer slenderMesh;
   public Animator slenderAnimator;

   private float baseSpeed = 0.5f;

    void Start()
    {
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        player = FindObjectOfType<PlayerMovement>();
        slenderMesh = GetComponentInChildren<SkinnedMeshRenderer>();
        slenderAnimator = GetComponent<Animator>();

        navMeshAgent.speed = baseSpeed;
    }
    void Update()
    {
        navMeshAgent.SetDestination(player.transform.position);
        float currentVelocity = navMeshAgent.velocity.magnitude;
        slenderAnimator.SetFloat("speed", currentVelocity);
        ChangeDifficulty();
    }

    public void ChangeDifficulty()
    {
        int notesCount = GameManager.Instance.GetNotesCount();
        navMeshAgent.speed = baseSpeed + (notesCount * 0.5f);
    }
}