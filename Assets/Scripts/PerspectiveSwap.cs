using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class PerspectiveSwap : MonoBehaviour
{
    [SerializeField] public Transform player; // Player reference

    private PlayerInput playerInput;
    private InputAction flipAction;

    public Camera camera2D;
    public Camera camera3D;
    public float flipDuration = 1f; //time to transition
    public Vector3 offset2D = new Vector3(0, 1, -5); // 2D Camera Offset
    public Vector3 offset3D = new Vector3(0, 3, -5);  // 3D Camera Offset

    private bool groundedPlayer;
    private CharacterController controller;
    //private PlayerController playerController; // Reference to PlayerController

    private Vector3 original2DPosition;
    private Quaternion original2DRotation;
    private bool is2D = true; //2d = true, 3d = false
    private Vector3 ogScale; //scale of player for 2d

    // Start is called before the first frame update
    void Start()
    {
        //rb = GetComponent<Rigidbody>();
        ogScale = transform.localScale;
        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>(); // Use existing CharacterController
        //playerController = player.GetComponent<PlayerController>(); // Get the PlayerController script

        // Save the original 2D camera position & rotation
        original2DPosition = camera2D.transform.position;
        original2DRotation = camera2D.transform.rotation;

        flipAction = playerInput.actions["Flip"]; // Cache the input action
        Show2D(); //start in 2d mode
    }

    // Update is called once per frame
    void Update()
    {
        //flip input key
        //if(Input.GetKeyDown(flipKey))
        groundedPlayer = controller.isGrounded;
        if (flipAction.triggered && groundedPlayer)
        {
            is2D = !is2D; // Toggle the state
            if (is2D)
            {
                Show2D();
            }
            else
            {
                Show3D();
            }
            // Update camera follow positions
            UpdateCameraPosition();
        }
    }

    // Call this function to disable FPS camera,
    // and enable overhead camera.
    public void Show2D()
    {
        camera3D.enabled = false;
        camera2D.enabled = true;

        is2D = true;
        transform.localScale = new Vector3(ogScale.x, ogScale.y, 1f); // Flatten Z-axis

        // Reset 2D camera position to its original location
        Vector3 resetPosition = original2DPosition;
        resetPosition.y = original2DPosition.y; // Ensure Y height stays the same
        camera2D.transform.position = resetPosition;

        camera2D.transform.rotation = original2DRotation;
    }

    // Call this function to enable FPS camera,
    // and disable overhead camera.
    public void Show3D()
    {
        camera3D.enabled = true;
        camera2D.enabled = false;

        is2D = false;
        transform.localScale = ogScale; // Restore original scale
    }

    private void UpdateCameraPosition()
    {
        if (is2D)
        {
            // Lock camera to follow the player on X but stay fixed in Z
            camera2D.transform.position = new Vector3(player.position.x, player.position.y + 1, offset2D.z);
        }
        else
        {
            // Move the 3D camera behind the player
            Vector3 targetPosition = player.position + player.transform.forward * offset3D.z + Vector3.up * offset3D.y;
            camera3D.transform.position = Vector3.Lerp(camera3D.transform.position, targetPosition, Time.deltaTime * 5);
            camera3D.transform.LookAt(player); // Make the camera always face the player
        }
    }

}
