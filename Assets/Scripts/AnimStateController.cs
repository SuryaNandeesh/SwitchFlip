using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimStateController : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("Smoothing factor for animation transitions")]
    [SerializeField] private float transitionSpeed = 10f;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;

    Animator animator;
    int isWalkingHash;
    int isJumpingHash;
    int isFallingHash;
    int isGroundedHash;
    int velocityXHash;
    int velocityYHash;
    
    private PlayerInput playerInput;
    private PlayerController playerController;
    
    // Animation state tracking
    private bool currentWalkingState = false;
    private bool currentJumpingState = false;
    private bool currentFallingState = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        playerController = GetComponent<PlayerController>();
        
        // Hash the animation parameters for better performance
        isWalkingHash = Animator.StringToHash("isWalking");
        isJumpingHash = Animator.StringToHash("isJumping");
        isFallingHash = Animator.StringToHash("isFalling");
        isGroundedHash = Animator.StringToHash("isGrounded");
        velocityXHash = Animator.StringToHash("velocityX");
        velocityYHash = Animator.StringToHash("velocityY");

        if (animator == null)
        {
            Debug.LogError("AnimStateController: No Animator component found!");
        }
        
        if (playerController == null)
        {
            Debug.LogError("AnimStateController: No PlayerController component found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (animator == null || playerController == null) return;

        // Get states from PlayerController (more reliable than input)
        bool shouldWalk = playerController.GetIsWalking();
        bool shouldJump = playerController.GetIsJumping();
        bool shouldFall = playerController.GetIsFalling();
        bool isGrounded = playerController.GetIsGrounded();

        // Update walking animation
        UpdateWalkingAnimation(shouldWalk);
        
        // Update jumping animation
        UpdateJumpingAnimation(shouldJump, isGrounded);
        
        // Update falling animation (if you have one)
        UpdateFallingAnimation(shouldFall);
        
        // Update additional parameters
        UpdateAdditionalParameters(isGrounded);

        // Debug output
        if (showDebugInfo)
        {
            Debug.Log($"Anim States - Walk: {shouldWalk}, Jump: {shouldJump}, Fall: {shouldFall}, Grounded: {isGrounded}");
        }
    }

    private void UpdateWalkingAnimation(bool shouldWalk)
    {
        if (currentWalkingState != shouldWalk)
        {
            animator.SetBool(isWalkingHash, shouldWalk);
            currentWalkingState = shouldWalk;
            
            if (showDebugInfo)
            {
                Debug.Log($"Walking animation: {shouldWalk}");
            }
        }
    }

    private void UpdateJumpingAnimation(bool shouldJump, bool isGrounded)
    {
        // More sophisticated jump animation handling
        if (shouldJump && !currentJumpingState)
        {
            // Start jump animation
            animator.SetBool(isJumpingHash, true);
            currentJumpingState = true;
            
            if (showDebugInfo)
            {
                Debug.Log("Jump animation started");
            }
        }
        else if (!shouldJump && currentJumpingState && isGrounded)
        {
            // End jump animation when landed
            animator.SetBool(isJumpingHash, false);
            currentJumpingState = false;
            
            if (showDebugInfo)
            {
                Debug.Log("Jump animation ended");
            }
        }
    }

    private void UpdateFallingAnimation(bool shouldFall)
    {
        // Only update if the animator has a falling parameter
        if (HasParameter(isFallingHash))
        {
            if (currentFallingState != shouldFall)
            {
                animator.SetBool(isFallingHash, shouldFall);
                currentFallingState = shouldFall;
                
                if (showDebugInfo)
                {
                    Debug.Log($"Falling animation: {shouldFall}");
                }
            }
        }
    }

    private void UpdateAdditionalParameters(bool isGrounded)
    {
        // Set grounded parameter if it exists
        if (HasParameter(isGroundedHash))
        {
            animator.SetBool(isGroundedHash, isGrounded);
        }

        // Set velocity parameters if they exist
        if (HasParameter(velocityYHash))
        {
            float verticalVel = playerController.GetVerticalVelocity();
            animator.SetFloat(velocityYHash, verticalVel);
        }
    }

    // Public method for PlayerController to call
    public void UpdateAnimationStates(bool walking, bool jumping, bool falling, bool grounded)
    {
        // This method allows PlayerController to directly update animation states
        // if needed, but the Update method above should handle most cases
    }

    // Utility method to check if animator has a parameter
    private bool HasParameter(int parameterHash)
    {
        if (animator == null) return false;
        
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.nameHash == parameterHash)
                return true;
        }
        return false;
    }

    // Public methods for external scripts to trigger animations
    public void TriggerJump()
    {
        if (animator != null)
        {
            animator.SetTrigger("jumpTrigger"); // If you have a jump trigger
        }
    }

    public void TriggerLand()
    {
        if (animator != null)
        {
            animator.SetTrigger("landTrigger"); // If you have a land trigger
        }
    }

    // Method to force animation state (useful for debugging)
    public void ForceAnimationState(string parameterName, bool value)
    {
        if (animator != null)
        {
            animator.SetBool(parameterName, value);
        }
    }
}
