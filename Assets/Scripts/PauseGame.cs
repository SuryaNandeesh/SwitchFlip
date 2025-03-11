using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseGame : MonoBehaviour
{
    [SerializeField] private InputActionReference pauseButton;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Canvas canvas2;
    private PlayerInput playerInput;
    private bool paused = false;
    private InputAction pauseAction;

    public void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        pauseAction = playerInput.actions["Pause"];
        bool pausePressed = playerInput.actions["Pause"].IsPressed();
    }

    private void OnEnable()
    {
        pauseButton.action.performed += _ => Pause();
        pauseButton.action.Enable();


        if (canvas != null)
        {
            canvas.enabled = paused;
        }

        if (canvas2 != null)
        {
            canvas2.enabled = paused;
        }

    }

    private void OnDisable()
    {
        pauseButton.action.performed -= _ => Pause();
        pauseButton.action.Disable();

        if (canvas != null)
        {
            canvas.enabled = paused;
        }

        if (canvas2 != null)
        {
            canvas2.enabled = paused;
        }
    }

    public void Pause()
    {
        paused = !paused;
        Time.timeScale = paused ? 0 : 1; // Freeze/unfreeze game time

        if (canvas != null)
        {
            canvas.enabled = paused;
        }

        if (canvas2 != null)
        {
            canvas2.enabled = paused;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
