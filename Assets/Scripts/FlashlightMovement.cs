using UnityEngine;
using UnityEngine.InputSystem;


public class FlashlightMovement : MonoBehaviour
{
    public Animator flashlight;

    public InputActionReference flashlightWalkAction;
    public InputActionReference flashlightSprintAction;
    void Start()
    {
        flashlight = GetComponent<Animator>();
    }

    void Update()
    {
        Vector2 movementInput = flashlightWalkAction.action.ReadValue<Vector2>();
        bool isMoving = movementInput.magnitude > 0.1f;
        bool isSprinting = flashlightSprintAction.action.IsPressed();
        if (isMoving){
            if (isSprinting){
                flashlight.ResetTrigger("Walk");
                flashlight.SetTrigger("Sprint");
            } else {
                flashlight.ResetTrigger("Sprint");
                flashlight.SetTrigger("Walk");
            }
        } else {
            flashlight.ResetTrigger("Walk");
            flashlight.ResetTrigger("Sprint");
        }
        if(isSprinting && isMoving){
            flashlight.ResetTrigger("Sprint");
            flashlight.SetTrigger("Walk");
        }
    }
    public void OnEnable()
    {
        flashlightWalkAction.action.Enable();
        flashlightSprintAction.action.Enable();
    }

    public void OnDisable()
    {
        flashlightWalkAction.action.Disable();
        flashlightSprintAction.action.Disable();
    }
}
