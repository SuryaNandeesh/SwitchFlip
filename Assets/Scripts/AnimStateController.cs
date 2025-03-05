using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimStateController : MonoBehaviour
{
    Animator animator;
    int isWalkingHash;
    int isJumpingHash;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isJumpingHash = Animator.StringToHash("isJumping");

        moveAction = playerInput.actions["Movement"];
        jumpAction = playerInput.actions["Jump"];
    }

    // Update is called once per frame
    void Update()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isJumping = animator.GetBool(isJumpingHash);
        bool movementPressed = playerInput.actions["Movement"].IsPressed();
        bool jumpPressed = playerInput.actions["Jump"].IsPressed();
        
        if(!isWalking && movementPressed)
        {
            animator.SetBool("isWalking", true);
        }
        
        if(isWalking && !movementPressed)
        {
            animator.SetBool("isWalking", false);
        }
        
        if(!isJumping && jumpPressed)
        {
            animator.SetBool("isJumping", true);
        }
        
        if(isJumping && !jumpPressed)
        {
            animator.SetBool("isJumping", false);
        }
        
    }
}
