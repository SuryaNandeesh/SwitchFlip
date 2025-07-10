using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalPost : MonoBehaviour
{
    [Header("Level Transition Settings")]
    [Tooltip("Name of the next scene to load. Leave empty to load next scene by build index.")]
    public string nextSceneName = "";
    
    [Tooltip("Delay before transitioning to next level (in seconds)")]
    public float transitionDelay = 1.0f;
    
    [Header("Visual Effects")]
    [Tooltip("Particle effect to play when goal is reached (optional)")]
    public GameObject goalEffect;
    
    [Tooltip("Sound to play when goal is reached (optional)")]
    public AudioSource goalSound;
    
    [Header("Animation")]
    [Tooltip("Should the star rotate when idle?")]
    public bool rotateIdle = true;
    
    [Tooltip("Rotation speed for idle animation")]
    public float rotationSpeed = 50f;
    
    [Header("Requirements")]
    [Tooltip("Require all coins to be collected before allowing level completion")]
    public bool requireAllCoins = false;
    
    private bool goalReached = false;
    private bool playerInRange = false;
    private LevelManager levelManager;
    private CoinCollection coinCollection;
    
    private void Start()
    {
        // Ensure the collider is set as a trigger
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
        else
        {
            Debug.LogWarning("GoalPost: No collider found on " + gameObject.name + ". Adding a trigger collider.");
            // Add a sphere collider as a backup
            SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = 2f; // Adjust as needed
        }
        
        // Find level manager and coin collection references
        levelManager = FindObjectOfType<LevelManager>();
        coinCollection = FindObjectOfType<CoinCollection>();
    }
    
    private void Update()
    {
        // Rotate the star for visual appeal
        if (rotateIdle && !goalReached)
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if the player touched the goal
        if (other.CompareTag("Player") && !goalReached)
        {
            playerInRange = true;
            
            // Check if requirements are met
            if (CanCompleteLevel())
            {
                ReachGoal();
            }
            else
            {
                ShowRequirementMessage();
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
    
    private bool CanCompleteLevel()
    {
        // Check coin requirement if enabled
        if (requireAllCoins && coinCollection != null)
        {
            // You would need to modify CoinCollection to expose total coins and collected coins
            // For now, this is a placeholder that returns true
            return true;
        }
        
        // Check level manager requirements
        if (levelManager != null)
        {
            return levelManager.CanCompleteLevel();
        }
        
        return true;
    }
    
    private void ShowRequirementMessage()
    {
        if (requireAllCoins)
        {
            Debug.Log("Collect all coins before reaching the goal!");
            // You could show a UI message here
        }
    }
    
    private void ReachGoal()
    {
        if (goalReached) return;
        
        goalReached = true;
        
        Debug.Log("Goal reached! Transitioning to next level...");
        
        // Notify level manager
        if (levelManager != null)
        {
            levelManager.LevelCompleted();
        }
        
        // Play visual effect if assigned
        if (goalEffect != null)
        {
            Instantiate(goalEffect, transform.position, transform.rotation);
        }
        
        // Play sound effect if assigned
        if (goalSound != null)
        {
            goalSound.Play();
        }
        
        // Start the transition after a delay
        StartCoroutine(TransitionToNextLevel());
    }
    
    private IEnumerator TransitionToNextLevel()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(transitionDelay);
        
        // Load the next scene
        LoadNextScene();
    }
    
    private void LoadNextScene()
    {
        try
        {
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                // Load scene by name
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                // Use level manager if available
                if (levelManager != null)
                {
                    levelManager.LoadNextLevel();
                }
                else
                {
                    // Fallback to manual scene loading
                    int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                    int nextSceneIndex = currentSceneIndex + 1;
                    
                    // Check if there's a next scene
                    if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
                    {
                        SceneManager.LoadScene(nextSceneIndex);
                    }
                    else
                    {
                        Debug.Log("No more levels! You've completed the game!");
                        // You could load a victory screen or restart the first level here
                        SceneManager.LoadScene(0); // Restart from first scene
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load next scene: " + e.Message);
        }
    }
    
    // Optional: Visual feedback for when player is in range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = goalReached ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 2f);
        
        // Draw additional gizmo if player is in range
        if (playerInRange)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 2.2f);
        }
    }
} 