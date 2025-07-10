using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Level Display")]
    [Tooltip("Text component to display current level")]
    public TextMeshProUGUI levelText;
    
    [Tooltip("Text component to display level completion message")]
    public TextMeshProUGUI completionText;
    
    [Header("Boss Battle UI")]
    [Tooltip("Text component to display boss health")]
    public TextMeshProUGUI bossHealthText;
    
    [Tooltip("Slider to show boss health bar")]
    public Slider bossHealthSlider;
    
    [Tooltip("Panel to show when boss battle starts")]
    public GameObject bossBattlePanel;
    
    [Header("Player Health UI")]
    [Tooltip("Text component to display player health")]
    public TextMeshProUGUI playerHealthText;
    
    [Tooltip("Slider to show player health bar")]
    public Slider playerHealthSlider;
    
    [Tooltip("Panel containing player health UI")]
    public GameObject playerHealthPanel;
    
    [Header("General UI")]
    [Tooltip("Canvas group for fade effects")]
    public CanvasGroup mainCanvasGroup;
    
    private LevelManager levelManager;
    private static UIManager instance;
    
    public static UIManager Instance => instance;
    
    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        InitializeUI();
    }
    
    private void InitializeUI()
    {
        // Set up level text
        UpdateLevelDisplay();
        
        // Hide completion text initially
        if (completionText != null)
        {
            completionText.gameObject.SetActive(false);
        }
        
        // Hide boss UI initially
        HideBossUI();
        
        // Show player health UI
        ShowPlayerHealthUI();
    }
    
    public void UpdateLevelDisplay()
    {
        if (levelText != null)
        {
            if (levelManager != null)
            {
                levelText.text = levelManager.levelName;
            }
            else
            {
                // Fallback: use scene name
                string sceneName = SceneManager.GetActiveScene().name;
                if (sceneName == "MainGame")
                {
                    levelText.text = "Level 1";
                }
                else if (sceneName.StartsWith("Level"))
                {
                    levelText.text = sceneName.Replace("Level", "Level ");
                }
                else
                {
                    levelText.text = sceneName;
                }
            }
        }
    }
    
    public void ShowCompletionMessage(string message)
    {
        if (completionText != null)
        {
            completionText.text = message;
            completionText.gameObject.SetActive(true);
        }
    }
    
    public void HideCompletionMessage()
    {
        if (completionText != null)
        {
            completionText.gameObject.SetActive(false);
        }
    }
    
    // Boss Battle UI Methods
    public void ShowBossUI()
    {
        if (bossBattlePanel != null)
        {
            bossBattlePanel.SetActive(true);
        }
    }
    
    public void HideBossUI()
    {
        if (bossBattlePanel != null)
        {
            bossBattlePanel.SetActive(false);
        }
    }
    
    public void UpdateBossHealth(int currentHealth, int maxHealth)
    {
        // Update boss health text
        if (bossHealthText != null)
        {
            bossHealthText.text = $"Boss Health: {currentHealth}/{maxHealth}";
        }
        
        // Update boss health slider
        if (bossHealthSlider != null)
        {
            bossHealthSlider.maxValue = maxHealth;
            bossHealthSlider.value = currentHealth;
        }
    }
    
    // Player Health UI Methods
    public void ShowPlayerHealthUI()
    {
        if (playerHealthPanel != null)
        {
            playerHealthPanel.SetActive(true);
        }
    }
    
    public void HidePlayerHealthUI()
    {
        if (playerHealthPanel != null)
        {
            playerHealthPanel.SetActive(false);
        }
    }
    
    public void UpdatePlayerHealth(int currentHealth, int maxHealth)
    {
        // Update player health text
        if (playerHealthText != null)
        {
            playerHealthText.text = $"Health: {currentHealth}/{maxHealth}";
        }
        
        // Update player health slider
        if (playerHealthSlider != null)
        {
            playerHealthSlider.maxValue = maxHealth;
            playerHealthSlider.value = currentHealth;
        }
    }
    
    // Utility methods for UI effects
    public void FadeIn(float duration = 1f)
    {
        if (mainCanvasGroup != null)
        {
            StartCoroutine(FadeCanvasGroup(mainCanvasGroup, 0f, 1f, duration));
        }
    }
    
    public void FadeOut(float duration = 1f)
    {
        if (mainCanvasGroup != null)
        {
            StartCoroutine(FadeCanvasGroup(mainCanvasGroup, 1f, 0f, duration));
        }
    }
    
    private System.Collections.IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            canvasGroup.alpha = alpha;
            yield return null;
        }
        
        canvasGroup.alpha = endAlpha;
    }
} 