using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Text component to display current level")]
    public TextMeshProUGUI levelText;
    
    [Tooltip("Text component to display completion message")]
    public TextMeshProUGUI completionText;
    
    [Header("Level Settings")]
    [Tooltip("Name of this level")]
    public string levelName = "Level 1";
    
    [Tooltip("Time limit for this level (0 = no time limit)")]
    public float timeLimit = 0f;
    
    [Header("Progress Tracking")]
    [Tooltip("Number of coins required to complete level (0 = not required)")]
    public int requiredCoins = 0;
    
    [Header("Boss Battle")]
    [Tooltip("Is this a boss level?")]
    public bool isBossLevel = false;
    
    [Tooltip("Should the goal post be hidden until boss is defeated?")]
    public bool hideGoalUntilBossDefeated = true;
    
    private float levelStartTime;
    private bool levelCompleted = false;
    private UIManager uiManager;
    
    // Static property to track overall game progress
    public static int CurrentLevel
    {
        get { return PlayerPrefs.GetInt("CurrentLevel", 1); }
        set { PlayerPrefs.SetInt("CurrentLevel", value); }
    }
    
    private void Start()
    {
        levelStartTime = Time.time;
        uiManager = UIManager.Instance;
        
        // Update level display through UIManager if available
        if (uiManager != null)
        {
            uiManager.UpdateLevelDisplay();
        }
        else
        {
            // Fallback to direct UI update
            UpdateLevelDisplayDirect();
        }
        
        // Hide completion text initially
        if (completionText != null)
        {
            completionText.gameObject.SetActive(false);
        }
        
        // Update current level progress
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex > 0) // Don't count main menu as level 1
        {
            CurrentLevel = sceneIndex;
        }
        
        // Handle boss level setup
        if (isBossLevel && hideGoalUntilBossDefeated)
        {
            GoalPost goalPost = FindObjectOfType<GoalPost>();
            if (goalPost != null)
            {
                goalPost.gameObject.SetActive(false);
            }
        }
        
        Debug.Log($"Level Manager initialized for {levelName}");
    }
    
    private void Update()
    {
        // Check time limit if set
        if (timeLimit > 0 && !levelCompleted)
        {
            float elapsedTime = Time.time - levelStartTime;
            if (elapsedTime >= timeLimit)
            {
                LevelFailed("Time's up!");
            }
        }
    }
    
    private void UpdateLevelDisplayDirect()
    {
        if (levelText != null)
        {
            levelText.text = levelName;
        }
    }
    
    public void LevelCompleted()
    {
        if (levelCompleted) return;
        
        levelCompleted = true;
        
        // Show completion message through UIManager if available
        if (uiManager != null)
        {
            uiManager.ShowCompletionMessage("Level Complete!");
        }
        else if (completionText != null)
        {
            completionText.text = "Level Complete!";
            completionText.gameObject.SetActive(true);
        }
        
        // Save progress
        int nextLevel = SceneManager.GetActiveScene().buildIndex + 1;
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        if (nextLevel > unlockedLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", nextLevel);
        }
        
        // Calculate and save completion time
        float completionTime = Time.time - levelStartTime;
        string levelKey = "BestTime_" + SceneManager.GetActiveScene().name;
        float bestTime = PlayerPrefs.GetFloat(levelKey, float.MaxValue);
        if (completionTime < bestTime)
        {
            PlayerPrefs.SetFloat(levelKey, completionTime);
            Debug.Log($"New best time for {levelName}: {completionTime:F2} seconds!");
        }
        
        PlayerPrefs.Save();
        
        Debug.Log($"Level {levelName} completed in {completionTime:F2} seconds!");
    }
    
    public void BossDefeated()
    {
        if (isBossLevel)
        {
            Debug.Log("Boss defeated! Revealing goal post...");
            
            // Show goal post
            GoalPost goalPost = FindObjectOfType<GoalPost>();
            if (goalPost != null)
            {
                goalPost.gameObject.SetActive(true);
                
                // Optional: Add some effect when goal post appears
                GameObject goalEffect = goalPost.goalEffect;
                if (goalEffect != null)
                {
                    Instantiate(goalEffect, goalPost.transform.position, goalPost.transform.rotation);
                }
            }
        }
    }
    
    public void LevelFailed(string reason = "Level Failed")
    {
        if (levelCompleted) return;
        
        Debug.Log($"Level failed: {reason}");
        
        // Show failure message through UIManager if available
        if (uiManager != null)
        {
            uiManager.ShowCompletionMessage(reason);
        }
        else if (completionText != null)
        {
            completionText.text = reason;
            completionText.gameObject.SetActive(true);
        }
        
        // Restart level after a delay
        Invoke("RestartLevel", 2f);
    }
    
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            LoadMainMenu();
        }
    }
    
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0); // Assuming main menu is at index 0
    }
    
    public bool CanCompleteLevel()
    {
        // If this is a boss level, check if boss is defeated
        if (isBossLevel)
        {
            PlatformerBoss boss = FindObjectOfType<PlatformerBoss>();
            if (boss != null)
            {
                return false; // Boss still alive, can't complete level
            }
        }
        
        // Check if required coins are collected
        if (requiredCoins > 0)
        {
            CoinCollection coinCollection = FindObjectOfType<CoinCollection>();
            if (coinCollection != null)
            {
                // This would require modifying CoinCollection to expose coin count
                // For now, we'll assume the level can be completed
                return true;
            }
        }
        
        return true;
    }
    
    // Utility methods for other scripts to use
    public static void UnlockLevel(int levelIndex)
    {
        int currentUnlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);
        if (levelIndex > currentUnlocked)
        {
            PlayerPrefs.SetInt("UnlockedLevel", levelIndex);
            PlayerPrefs.Save();
        }
    }
    
    public static bool IsLevelUnlocked(int levelIndex)
    {
        return levelIndex <= PlayerPrefs.GetInt("UnlockedLevel", 1);
    }
    
    public static float GetBestTime(string sceneName)
    {
        return PlayerPrefs.GetFloat("BestTime_" + sceneName, 0f);
    }
    
    // Method to get the current level name (useful for UI)
    public string GetLevelName()
    {
        return levelName;
    }
    
    // Method to check if level has time limit
    public bool HasTimeLimit()
    {
        return timeLimit > 0;
    }
    
    // Method to get remaining time
    public float GetRemainingTime()
    {
        if (timeLimit <= 0) return 0f;
        float elapsedTime = Time.time - levelStartTime;
        return Mathf.Max(0f, timeLimit - elapsedTime);
    }
} 