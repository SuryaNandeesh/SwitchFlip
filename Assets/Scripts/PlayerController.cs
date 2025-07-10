using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference movement;
    [SerializeField] private Canvas canvas;
    [SerializeField] public float jumpButtonCooldown;

    Rigidbody rb;

    private PlayerInput playerInput;
    private CharacterController controller;
    private Vector3 playerVelocity;
    public bool groundedPlayer;
    public float speed = 6;
    private float jumpHeight = 4.0f;
    private float gravityValue = -9.81f;
    public float? jumpButtonPressedTime;
    public float? lastGroundedTime;
    private InputAction moveAction;
    private InputAction threeMoveAction;
    private InputAction jumpAction;

    // Animation state tracking
    [Header("Animation States")]
    public bool isJumping = false;
    public bool isWalking = false;
    public bool isFalling = false;
    
    // Animation thresholds
    [SerializeField] private float walkThreshold = 0.1f;
    [SerializeField] private float fallThreshold = -1f; // Y velocity threshold for falling
    
    // References
    private AnimStateController animController;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        controller = gameObject.AddComponent<CharacterController>();
        controller.center = new Vector3(0, 0.78f, 0); // Adjust Y value based on model size
        controller.height = 1.48f;

        // Get animation controller reference
        animController = GetComponent<AnimStateController>();

        moveAction = playerInput.actions["Movement"];
        threeMoveAction = playerInput.actions["ThreeDeeMovement"];
        jumpAction = playerInput.actions["Jump"];

        if (canvas != null)
        {
            canvas.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool wasGrounded = groundedPlayer;
        groundedPlayer = controller.isGrounded;

        // Track when we land
        if (!wasGrounded && groundedPlayer)
        {
            OnLanded();
        }

        if (groundedPlayer)
        {
            lastGroundedTime = Time.time;
        }

        Vector2 input = moveAction.enabled ? moveAction.ReadValue<Vector2>() : threeMoveAction.ReadValue<Vector2>(); // Use cached action

        if (jumpAction.triggered) { jumpButtonPressedTime = Time.time; }

        // Handle jumping
        if (jumpAction.IsPressed() && !groundedPlayer && Time.time - lastGroundedTime <= jumpButtonCooldown)
        {
            // Prevent jumping while in the air
            playerVelocity.y = 0f;
        }
        else if (jumpAction.IsPressed() && groundedPlayer && Time.time - jumpButtonPressedTime <= jumpButtonCooldown)
        {
            PerformJump();
        }

        Vector3 move = new Vector3(input.x, 0, input.y);
        move.y = 0f;
        controller.Move(move * Time.deltaTime * speed);

        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; // Small negative value prevents floating on slopes
        }

        float fallMultiplier = 2.5f; // Increases fall speed
        if (!groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y += gravityValue * fallMultiplier * Time.deltaTime;
        }
        else
        {
            playerVelocity.y += gravityValue * Time.deltaTime;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        // Handle rotation
        if (move.magnitude > walkThreshold) // Avoid jitter when standing still
        {
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // Smoother rotation
        }

        // Update animation states
        UpdateAnimationStates(move);
    }

    private void PerformJump()
    {
        playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        jumpButtonPressedTime = null; // Reset jump button pressed time
        lastGroundedTime = null; // Reset last grounded time
        
        // Set jump animation state
        isJumping = true;
        isFalling = false;
        
        Debug.Log("Jump performed!");
    }

    private void OnLanded()
    {
        isJumping = false;
        isFalling = false;
        Debug.Log("Player landed!");
    }

    private void UpdateAnimationStates(Vector3 moveInput)
    {
        // Update walking state
        bool wasWalking = isWalking;
        isWalking = moveInput.magnitude > walkThreshold && groundedPlayer;
        
        // Update falling state (only when not grounded and moving downward)
        if (!groundedPlayer)
        {
            if (playerVelocity.y < fallThreshold && !isJumping)
            {
                isFalling = true;
                isJumping = false; // Transition from jump to fall
            }
        }
        else
        {
            isFalling = false;
        }

        // Notify animation controller if we have one
        if (animController != null)
        {
            animController.UpdateAnimationStates(isWalking, isJumping, isFalling, groundedPlayer);
        }
    }

    public void SetPerspective(bool is3D)
    {
        if (is3D)
        {
            moveAction.Disable();
            threeMoveAction.Enable();
            Debug.Log("Switched to 3D movement input.");
        }
        else
        {
            threeMoveAction.Disable();
            moveAction.Enable();
            Debug.Log("Switched to 2D movement input.");
        }
    }

    // Public getters for animation system
    public bool GetIsWalking() => isWalking;
    public bool GetIsJumping() => isJumping;
    public bool GetIsFalling() => isFalling;
    public bool GetIsGrounded() => groundedPlayer;
    public float GetVerticalVelocity() => playerVelocity.y;
}