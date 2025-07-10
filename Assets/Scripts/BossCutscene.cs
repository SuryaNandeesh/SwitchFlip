using System.Collections;
using UnityEngine;
using TMPro;

public class BossCutscene : MonoBehaviour
{
    [Header("Cutscene Settings")]
    [Tooltip("Duration of the cutscene in seconds")]
    public float cutsceneDuration = 5f;
    
    [Tooltip("Should the player be frozen during cutscene?")]
    public bool freezePlayer = true;
    
    [Header("Camera Movement")]
    [Tooltip("Camera to use for cutscene")]
    public Camera cutsceneCamera;
    
    [Tooltip("Boss transform to focus on")]
    public Transform bossTransform;
    
    [Tooltip("Player transform")]
    public Transform playerTransform;
    
    [Tooltip("Distance from boss for camera")]
    public float cameraDistance = 8f;
    
    [Tooltip("Height offset for camera")]
    public float cameraHeightOffset = 3f;
    
    [Header("UI Elements")]
    [Tooltip("Panel to show during cutscene")]
    public GameObject cutscenePanel;
    
    [Tooltip("Text to display boss name/intro")]
    public TextMeshProUGUI bossNameText;
    
    [Tooltip("Text for additional dialogue")]
    public TextMeshProUGUI dialogueText;
    
    [Header("Cutscene Content")]
    [Tooltip("Boss name to display")]
    public string bossName = "Shadow Guardian";
    
    [Tooltip("Dialogue lines to show during cutscene")]
    public string[] dialogueLines = new string[]
    {
        "A powerful guardian blocks your path!",
        "Jump on its head to damage it!",
        "Defeat it to proceed!"
    };
    
    [Tooltip("Time each dialogue line stays on screen")]
    public float dialogueDisplayTime = 1.5f;
    
    [Header("Audio")]
    [Tooltip("Music to play during cutscene")]
    public AudioSource cutsceneMusic;
    
    [Tooltip("Boss roar or intro sound")]
    public AudioSource bossIntroSound;
    
    // Private variables
    private Camera originalCamera;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    private PlayerController playerController;
    private PlatformerBoss boss;
    private bool cutsceneCompleted = false;
    
    private void Start()
    {
        // Get references
        playerController = FindObjectOfType<PlayerController>();
        boss = FindObjectOfType<PlatformerBoss>();
        
        // Store original camera
        originalCamera = Camera.main;
        if (originalCamera != null)
        {
            originalCameraPosition = originalCamera.transform.position;
            originalCameraRotation = originalCamera.transform.rotation;
        }
        
        // Use main camera if no cutscene camera assigned
        if (cutsceneCamera == null)
        {
            cutsceneCamera = originalCamera;
        }
        
        // Start cutscene automatically
        StartCutscene();
    }
    
    public void StartCutscene()
    {
        if (cutsceneCompleted) return;
        
        Debug.Log("Starting boss cutscene...");
        StartCoroutine(PlayCutscene());
    }
    
    private IEnumerator PlayCutscene()
    {
        // Freeze player
        if (freezePlayer && playerController != null)
        {
            playerController.enabled = false;
        }
        
        // Disable boss initially
        if (boss != null)
        {
            boss.enabled = false;
        }
        
        // Show cutscene UI
        if (cutscenePanel != null)
        {
            cutscenePanel.SetActive(true);
        }
        
        // Play cutscene music
        if (cutsceneMusic != null)
        {
            cutsceneMusic.Play();
        }
        
        // Camera movement and dialogue
        yield return StartCoroutine(CutsceneSequence());
        
        // End cutscene
        EndCutscene();
    }
    
    private IEnumerator CutsceneSequence()
    {
        // Phase 1: Focus on boss (2 seconds)
        yield return StartCoroutine(MoveCameraToPosition(GetBossFocusPosition(), 1f));
        
        // Show boss name
        if (bossNameText != null)
        {
            bossNameText.text = bossName;
            bossNameText.gameObject.SetActive(true);
        }
        
        // Play boss intro sound
        if (bossIntroSound != null)
        {
            bossIntroSound.Play();
        }
        
        yield return new WaitForSeconds(1f);
        
        // Phase 2: Show dialogue
        yield return StartCoroutine(ShowDialogue());
        
        // Phase 3: Pan back to show both player and boss
        Vector3 overviewPosition = GetOverviewPosition();
        yield return StartCoroutine(MoveCameraToPosition(overviewPosition, 1f));
        
        yield return new WaitForSeconds(0.5f);
    }
    
    private IEnumerator ShowDialogue()
    {
        if (dialogueText != null && dialogueLines.Length > 0)
        {
            dialogueText.gameObject.SetActive(true);
            
            foreach (string line in dialogueLines)
            {
                dialogueText.text = line;
                yield return new WaitForSeconds(dialogueDisplayTime);
            }
        }
    }
    
    private IEnumerator MoveCameraToPosition(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = cutsceneCamera.transform.position;
        Quaternion startRotation = cutsceneCamera.transform.rotation;
        
        // Calculate target rotation (look at boss)
        Vector3 directionToBoss = (bossTransform.position - targetPosition).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToBoss);
        
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            
            // Smooth camera movement
            cutsceneCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            cutsceneCamera.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            
            yield return null;
        }
        
        cutsceneCamera.transform.position = targetPosition;
        cutsceneCamera.transform.rotation = targetRotation;
    }
    
    private Vector3 GetBossFocusPosition()
    {
        if (bossTransform == null) return cutsceneCamera.transform.position;
        
        // Position camera to focus on boss
        Vector3 position = bossTransform.position;
        position += Vector3.back * cameraDistance;
        position += Vector3.up * cameraHeightOffset;
        
        return position;
    }
    
    private Vector3 GetOverviewPosition()
    {
        if (bossTransform == null || playerTransform == null) 
            return cutsceneCamera.transform.position;
        
        // Position camera to show both player and boss
        Vector3 midpoint = (bossTransform.position + playerTransform.position) / 2f;
        midpoint += Vector3.back * (cameraDistance * 1.5f);
        midpoint += Vector3.up * (cameraHeightOffset * 0.8f);
        
        return midpoint;
    }
    
    private void EndCutscene()
    {
        cutsceneCompleted = true;
        
        Debug.Log("Boss cutscene completed!");
        
        // Hide cutscene UI
        if (cutscenePanel != null)
        {
            cutscenePanel.SetActive(false);
        }
        
        // Hide dialogue elements
        if (bossNameText != null)
        {
            bossNameText.gameObject.SetActive(false);
        }
        
        if (dialogueText != null)
        {
            dialogueText.gameObject.SetActive(false);
        }
        
        // Restore camera position
        if (originalCamera != null && originalCamera != cutsceneCamera)
        {
            originalCamera.gameObject.SetActive(true);
            cutsceneCamera.gameObject.SetActive(false);
        }
        else if (cutsceneCamera != null)
        {
            cutsceneCamera.transform.position = originalCameraPosition;
            cutsceneCamera.transform.rotation = originalCameraRotation;
        }
        
        // Stop cutscene music
        if (cutsceneMusic != null)
        {
            cutsceneMusic.Stop();
        }
        
        // Unfreeze player
        if (freezePlayer && playerController != null)
        {
            playerController.enabled = true;
        }
        
        // Enable boss
        if (boss != null)
        {
            boss.enabled = true;
        }
        
        // Destroy cutscene object
        Destroy(gameObject);
    }
    
    // Public method to skip cutscene (call this if player presses a button)
    public void SkipCutscene()
    {
        if (!cutsceneCompleted)
        {
            StopAllCoroutines();
            EndCutscene();
        }
    }
} 