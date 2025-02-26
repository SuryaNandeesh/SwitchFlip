using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference movement;
    [SerializeField] private Canvas canvas;

    Rigidbody rb;

    private PlayerInput playerInput;
    private CharacterController controller;
    private Vector3 playerVelocity;
    public bool groundedPlayer;
    public float speed = 6;
    private float jumpHeight = 4.0f;
    private float gravityValue = -9.81f;
    private InputAction moveAction;
    private InputAction jumpAction;


    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        controller = gameObject.AddComponent<CharacterController>();

        moveAction = playerInput.actions["Movement"];
        jumpAction = playerInput.actions["Jump"];

        if (canvas != null)
        {
            canvas.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        groundedPlayer = controller.isGrounded;

        Vector2 input = moveAction.ReadValue<Vector2>(); // Use cached action
        if (jumpAction.triggered && groundedPlayer)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
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

        //// Makes the player jump
        //if (playerInput.actions["Jump"].triggered && groundedPlayer)
        //{
        //    Debug.Log("Jump Triggered!");
        //    playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        //}


        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (move.magnitude > 0.1f) // Avoid jitter when standing still
        {
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // Smoother rotation
        }
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Ground"))
    //    {
    //        groundedPlayer = true;
    //    }
    //    else
    //    {
    //        groundedPlayer = false;
    //    }
}