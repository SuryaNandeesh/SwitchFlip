using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseGame : MonoBehaviour
{
    [SerializeField] private InputAction pauseButton;
    [SerializeField] private Canvas canvas;

    private bool paused = false;

    private void OnEnable()
    {
        pauseButton.Enable();
    }
    
    private void OnDisable()
    {
        pauseButton.Disable();
    }

    // Start is called before the first frame update
    private void Start()
    {
        pauseButton.performed += _ => Pause(); //also followed video tutorial for figuring out this one
    }

    public void Pause()
    {
        paused = !paused;
        if (paused)
        {
            Time.timeScale = 0; //the world
            canvas.enabled = true;
        }
        else
        {
            Time.timeScale = 1; //toki wa noki das
            canvas.enabled = false;
        }
    }
}
