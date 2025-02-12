using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseGame : MonoBehaviour
{
    [SerializeField] private InputActionReference pauseButton;
    [SerializeField] private Canvas canvas;

    private bool paused = false;

    private void OnEnable()
    {
        pauseButton.action.performed += _ => Pause();
        pauseButton.action.Enable();

        if (canvas != null)
        {
            canvas.enabled = paused;
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
    }

    public void Pause()
    {
        paused = !paused;
        Time.timeScale = paused ? 0 : 1; // Freeze/unfreeze game time

        if (canvas != null)
        {
            canvas.enabled = paused;
        }
    }
}
