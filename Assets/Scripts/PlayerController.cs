using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController: MonoBehaviour
{
    [SerializeField] private InputActionReference movement;

    Rigidbody rb;

    public float speed = 6;
    private PlayerInput playerInput;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        controller = gameObject.AddComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        /* //old movement before impememnting in new plugin
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical"); 

        Vector3 directionalInput = new Vector3 (horizontalInput, 0, verticalInput); //side to side, up to down, forward to back

        transform.Translate (directionalInput * speed * Time.deltaTime);

        if (Input.GetKey(KeyCode.Space))
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        */

        Vector2 input = playerInput.actions["Movement"].ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);
        Transform cameraTransform = Camera.main.transform;
        move = move.x * cameraTransform.right + move.z * cameraTransform.forward;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * speed);

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        // Makes the player jump
        if (playerInput.actions["Jump"].triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (input != Vector2.zero)
        { 
            //ngl, i got this from a video about rebinding stuff in unity, i dont remember this much lol: here's the link if needed -- https://www.youtube.com/watch?v=csqVa2Vimao --

            float targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0, targetAngle, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * speed); //might need to change if too high
        }
    }
}
