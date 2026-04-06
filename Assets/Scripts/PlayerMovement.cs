using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public InputActionReference sprintAction;

    private Vector2 movementInput;
    private Rigidbody rb;
    private bool isMoving = false;

    private bool isJumping = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void OnMovement(InputValue value)
    {
        movementInput = value.Get<Vector2>();

        bool moving = movementInput.magnitude > 0.1f;

        if (moving && !isMoving && !isJumping)
        {
            MusicManager.Instance?.StartFootsteps();
        }
        else if (!moving && isMoving)
        {
            MusicManager.Instance?.StopFootsteps();
        }

        isMoving = moving;
    }

    void MovePlayer()
    {
        Vector3 direction = transform.right * movementInput.x + transform.forward * movementInput.y;
        bool isSprinting = sprintAction.action.IsPressed();
        speed = isSprinting ? 10f : 5f;
        rb.linearVelocity = new Vector3(direction.x * speed, rb.linearVelocity.y, direction.z * speed);

        if (isMoving)
        {
            MusicManager.Instance?.SetFootstepsPitch(isSprinting ? 1.5f : 1f);
        }
    }

    vvoid OnJump(InputValue value)
    {
        if (value.isPressed && !isJumping)
        {
            rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            isJumping = true;
            MusicManager.Instance?.StopFootsteps();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
        }
    }
}