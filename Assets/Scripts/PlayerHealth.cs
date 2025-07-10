using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [Tooltip("Maximum health the player can have")]
    public int maxHealth = 3;
    
    [Tooltip("Current health of the player")]
    public int currentHealth;
    
    [Tooltip("How long the player is invulnerable after taking damage")]
    public float invulnerabilityTime = 2f;
    
    [Header("Visual Effects")]
    [Tooltip("Material to use when player is invulnerable")]
    public Material invulnerableMaterial;
    
    [Tooltip("Effect to play when player takes damage")]
    public GameObject damageEffect;
    
    [Tooltip("Effect to play when player heals")]
    public GameObject healEffect;
    
    [Header("Audio")]
    [Tooltip("Sound to play when player takes damage")]
    public AudioSource damageSound;
    
    [Tooltip("Sound to play when player heals")]
    public AudioSource healSound;
    
    [Tooltip("Sound to play when player dies")]
    public AudioSource deathSound;
    
    // Private variables
    private bool isInvulnerable = false;
    private bool isDead = false;
    private Renderer playerRenderer;
    private Material originalMaterial;
    private UIManager uiManager;
    
    // Events
    public System.Action<int, int> OnHealthChanged; // currentHealth, maxHealth
    public System.Action OnPlayerDied;
    public System.Action OnPlayerHealed;
    
    private void Start()
    {
        // Initialize health
        currentHealth = maxHealth;
        
        // Get components
        playerRenderer = GetComponent<Renderer>();
        uiManager = UIManager.Instance;
        
        // Store original material
        if (playerRenderer != null)
        {
            originalMaterial = playerRenderer.material;
        }
        
        // Update UI
        UpdateHealthUI();
        
        Debug.Log($"Player health initialized: {currentHealth}/{maxHealth}");
    }
    
    public void TakeDamage(int damage = 1)
    {
        if (isDead || isInvulnerable) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth); // Ensure health doesn't go below 0
        
        Debug.Log($"Player took {damage} damage! Health: {currentHealth}/{maxHealth}");
        
        // Update UI
        UpdateHealthUI();
        
        // Play damage effects
        if (damageEffect != null)
        {
            Instantiate(damageEffect, transform.position, Quaternion.identity);
        }
        
        if (damageSound != null)
        {
            damageSound.Play();
        }
        
        // Trigger health changed event
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        // Check if player died
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvulnerabilityPeriod());
        }
    }
    
    public void Heal(int healAmount = 1)
    {
        if (isDead) return;
        
        int oldHealth = currentHealth;
        currentHealth += healAmount;
        currentHealth = Mathf.Min(maxHealth, currentHealth); // Ensure health doesn't exceed max
        
        // Only show effects if we actually healed
        if (currentHealth > oldHealth)
        {
            Debug.Log($"Player healed {healAmount} health! Health: {currentHealth}/{maxHealth}");
            
            // Play heal effects
            if (healEffect != null)
            {
                Instantiate(healEffect, transform.position, Quaternion.identity);
            }
            
            if (healSound != null)
            {
                healSound.Play();
            }
            
            // Trigger events
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            OnPlayerHealed?.Invoke();
        }
        
        // Update UI
        UpdateHealthUI();
    }
    
    private void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        Debug.Log("Player died!");
        
        // Play death sound
        if (deathSound != null)
        {
            deathSound.Play();
        }
        
        // Show death message
        if (uiManager != null)
        {
            uiManager.ShowCompletionMessage("You Died!");
        }
        
        // Trigger death event
        OnPlayerDied?.Invoke();
        
        // Restart level after delay
        Invoke("RestartLevel", 2f);
    }
    
    private System.Collections.IEnumerator InvulnerabilityPeriod()
    {
        isInvulnerable = true;
        
        // Visual feedback - change material and flash
        if (playerRenderer != null)
        {
            float flashInterval = 0.1f;
            float elapsedTime = 0f;
            
            while (elapsedTime < invulnerabilityTime)
            {
                // Flash between invulnerable and original material
                if (invulnerableMaterial != null)
                {
                    playerRenderer.material = (elapsedTime % (flashInterval * 2) < flashInterval) ? 
                        invulnerableMaterial : originalMaterial;
                }
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            // Restore original material
            if (originalMaterial != null)
            {
                playerRenderer.material = originalMaterial;
            }
        }
        else
        {
            // Simple wait if no renderer
            yield return new WaitForSeconds(invulnerabilityTime);
        }
        
        isInvulnerable = false;
    }
    
    private void UpdateHealthUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdatePlayerHealth(currentHealth, maxHealth);
        }
    }
    
    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    // Public getters
    public bool IsInvulnerable() => isInvulnerable;
    public bool IsDead() => isDead;
    public bool IsFullHealth() => currentHealth >= maxHealth;
    public float GetHealthPercentage() => (float)currentHealth / maxHealth;
    
    // Method to set health directly (useful for testing or power-ups)
    public void SetHealth(int newHealth)
    {
        currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
        UpdateHealthUI();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    // Method to restore to full health
    public void FullHeal()
    {
        Heal(maxHealth - currentHealth);
    }
} 