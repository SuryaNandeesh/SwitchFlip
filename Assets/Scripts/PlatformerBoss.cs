using System.Collections;
using UnityEngine;

public class PlatformerBoss : MonoBehaviour
{
    [Header("Boss Stats")]
    [Tooltip("Maximum health of the boss")]
    public int maxHealth = 3;
    
    [Tooltip("Current health of the boss")]
    public int currentHealth;
    
    [Header("Movement Settings")]
    [Tooltip("Boss movement speed")]
    public float moveSpeed = 2f;
    
    [Tooltip("Jump force for boss attacks")]
    public float jumpForce = 8f;
    
    [Tooltip("How far the boss can move from its starting position")]
    public float movementRange = 5f;
    
    [Header("Attack Settings")]
    [Tooltip("Time between boss attacks")]
    public float attackCooldown = 3f;
    
    [Tooltip("Force applied to player when boss attacks")]
    public float knockbackForce = 10f;
    
    [Tooltip("Damage dealt to player (if you have a health system)")]
    public int attackDamage = 1;
    
    [Header("Damage Settings")]
    [Tooltip("Time boss is invulnerable after taking damage")]
    public float invulnerabilityTime = 2f;
    
    [Tooltip("How much the boss bounces when hit")]
    public float hitKnockback = 5f;
    
    [Header("Visual Effects")]
    [Tooltip("Effect to play when boss takes damage")]
    public GameObject hitEffect;
    
    [Tooltip("Effect to play when boss is defeated")]
    public GameObject deathEffect;
    
    [Tooltip("Material to use when boss is invulnerable")]
    public Material invulnerableMaterial;
    
    [Header("Audio")]
    [Tooltip("Sound to play when boss takes damage")]
    public AudioSource hitSound;
    
    [Tooltip("Sound to play when boss is defeated")]
    public AudioSource deathSound;
    
    [Tooltip("Sound to play when boss attacks")]
    public AudioSource attackSound;
    
    // Private variables
    private Vector3 startPosition;
    private bool movingRight = true;
    private bool isInvulnerable = false;
    private bool isDead = false;
    private float lastAttackTime;
    private Rigidbody rb;
    private Renderer bossRenderer;
    private Material originalMaterial;
    private Animator animator;
    private UIManager uiManager;
    private GoalPost goalPost;
    
    // Animation hashes (if you have animations)
    private int isWalkingHash;
    private int isAttackingHash;
    private int isHitHash;
    private int isDeadHash;
    
    // Boss states
    public enum BossState
    {
        Idle,
        Moving,
        Attacking,
        Hit,
        Dead
    }
    
    public BossState currentState = BossState.Idle;
    
    private void Start()
    {
        // Initialize boss
        currentHealth = maxHealth;
        startPosition = transform.position;
        
        // Get components
        rb = GetComponent<Rigidbody>();
        bossRenderer = GetComponent<Renderer>();
        animator = GetComponent<Animator>();
        uiManager = UIManager.Instance;
        goalPost = FindObjectOfType<GoalPost>();
        
        // Store original material
        if (bossRenderer != null)
        {
            originalMaterial = bossRenderer.material;
        }
        
        // Initialize animation hashes
        if (animator != null)
        {
            isWalkingHash = Animator.StringToHash("isWalking");
            isAttackingHash = Animator.StringToHash("isAttacking");
            isHitHash = Animator.StringToHash("isHit");
            isDeadHash = Animator.StringToHash("isDead");
        }
        
        // Show boss UI
        if (uiManager != null)
        {
            uiManager.ShowBossUI();
            uiManager.UpdateBossHealth(currentHealth, maxHealth);
        }
        
        Debug.Log("Boss battle started! Boss has " + maxHealth + " health.");
    }
    
    private void Update()
    {
        if (isDead) return;
        
        switch (currentState)
        {
            case BossState.Idle:
                HandleIdleState();
                break;
            case BossState.Moving:
                HandleMovingState();
                break;
            case BossState.Attacking:
                HandleAttackingState();
                break;
            case BossState.Hit:
                HandleHitState();
                break;
        }
        
        // Check if it's time to attack
        if (Time.time - lastAttackTime > attackCooldown && currentState != BossState.Hit)
        {
            TryAttack();
        }
    }
    
    private void HandleIdleState()
    {
        // Transition to moving state
        currentState = BossState.Moving;
    }
    
    private void HandleMovingState()
    {
        MoveBoss();
        
        // Set walking animation
        if (animator != null)
        {
            animator.SetBool(isWalkingHash, true);
        }
    }
    
    private void HandleAttackingState()
    {
        // Attack state is handled by coroutines
        // This method can be used for attack state logic if needed
    }
    
    private void HandleHitState()
    {
        // Hit state is handled by coroutines
        // This method can be used for hit state logic if needed
    }
    
    private void MoveBoss()
    {
        if (rb != null)
        {
            // Calculate movement
            float movement = movingRight ? moveSpeed : -moveSpeed;
            rb.velocity = new Vector3(movement, rb.velocity.y, rb.velocity.z);
            
            // Check if boss should turn around
            float distanceFromStart = transform.position.x - startPosition.x;
            
            if (movingRight && distanceFromStart > movementRange)
            {
                movingRight = false;
                FlipBoss();
            }
            else if (!movingRight && distanceFromStart < -movementRange)
            {
                movingRight = true;
                FlipBoss();
            }
        }
    }
    
    private void FlipBoss()
    {
        // Flip the boss sprite/model
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    
    private void TryAttack()
    {
        // Find player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            
            // Attack if player is within range
            if (distanceToPlayer < 5f)
            {
                StartCoroutine(PerformAttack());
            }
        }
    }
    
    private IEnumerator PerformAttack()
    {
        currentState = BossState.Attacking;
        lastAttackTime = Time.time;
        
        // Play attack animation
        if (animator != null)
        {
            animator.SetBool(isWalkingHash, false);
            animator.SetTrigger(isAttackingHash);
        }
        
        // Play attack sound
        if (attackSound != null)
        {
            attackSound.Play();
        }
        
        // Wait for attack animation
        yield return new WaitForSeconds(0.5f);
        
        // Perform jump attack
        if (rb != null)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }
        
        // Wait for attack to finish
        yield return new WaitForSeconds(1f);
        
        currentState = BossState.Moving;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if player jumped on the boss
        if (other.CompareTag("Player") && !isInvulnerable && !isDead)
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            
            // Check if player is above the boss (jumping on it)
            if (other.transform.position.y > transform.position.y + 0.5f)
            {
                TakeDamage();
                
                // Bounce the player
                if (playerController != null)
                {
                    // Make player bounce
                    Rigidbody playerRb = other.GetComponent<Rigidbody>();
                    CharacterController playerCC = other.GetComponent<CharacterController>();
                    
                    if (playerRb != null)
                    {
                        playerRb.velocity = new Vector3(playerRb.velocity.x, jumpForce * 0.8f, playerRb.velocity.z);
                    }
                    else if (playerCC != null)
                    {
                        // For CharacterController, we need to handle this in PlayerController
                        // This is a simplified approach
                        Debug.Log("Player bounced off boss!");
                    }
                }
            }
            else
            {
                // Player touched boss from side - damage player (if you have player health system)
                DamagePlayer(other.gameObject);
            }
        }
    }
    
    public void TakeDamage()
    {
        if (isInvulnerable || isDead) return;
        
        currentHealth--;
        
        Debug.Log("Boss took damage! Health: " + currentHealth + "/" + maxHealth);
        
        // Update UI
        if (uiManager != null)
        {
            uiManager.UpdateBossHealth(currentHealth, maxHealth);
        }
        
        // Play hit effects
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }
        
        if (hitSound != null)
        {
            hitSound.Play();
        }
        
        // Check if boss is defeated
        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
        else
        {
            StartCoroutine(HitReaction());
        }
    }
    
    private IEnumerator HitReaction()
    {
        currentState = BossState.Hit;
        isInvulnerable = true;
        
        // Play hit animation
        if (animator != null)
        {
            animator.SetTrigger(isHitHash);
        }
        
        // Knockback effect
        if (rb != null)
        {
            Vector3 knockbackDirection = movingRight ? Vector3.left : Vector3.right;
            rb.AddForce(knockbackDirection * hitKnockback + Vector3.up * 2f, ForceMode.Impulse);
        }
        
        // Visual feedback - change material
        if (bossRenderer != null && invulnerableMaterial != null)
        {
            bossRenderer.material = invulnerableMaterial;
        }
        
        // Wait for invulnerability period
        yield return new WaitForSeconds(invulnerabilityTime);
        
        // Restore original material
        if (bossRenderer != null && originalMaterial != null)
        {
            bossRenderer.material = originalMaterial;
        }
        
        isInvulnerable = false;
        currentState = BossState.Moving;
    }
    
    private IEnumerator Die()
    {
        isDead = true;
        currentState = BossState.Dead;
        
        Debug.Log("Boss defeated!");
        
        // Play death animation
        if (animator != null)
        {
            animator.SetTrigger(isDeadHash);
        }
        
        // Play death effects
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        
        if (deathSound != null)
        {
            deathSound.Play();
        }
        
        // Hide boss UI
        if (uiManager != null)
        {
            uiManager.HideBossUI();
            uiManager.ShowCompletionMessage("Boss Defeated!");
        }
        
        // Notify level manager that boss is defeated
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.BossDefeated();
        }
        else
        {
            // Fallback: directly activate goal post if no level manager
            Debug.Log("No LevelManager found, directly activating goal post...");
            GoalPost goalPost = FindObjectOfType<GoalPost>();
            if (goalPost != null)
            {
                goalPost.gameObject.SetActive(true);
                Debug.Log("Goal post activated directly!");
            }
            else
            {
                Debug.LogError("No GoalPost found in scene!");
            }
        }
        
        // Stop boss movement
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }
        
        // Wait a moment
        yield return new WaitForSeconds(2f);
        
        // Double-check goal post is active (additional safety)
        GoalPost finalGoalCheck = FindObjectOfType<GoalPost>(true); // Include inactive objects
        if (finalGoalCheck != null && !finalGoalCheck.gameObject.activeInHierarchy)
        {
            Debug.Log("Goal post still inactive, forcing activation...");
            finalGoalCheck.gameObject.SetActive(true);
        }
        
        // Destroy boss after delay
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
    
    private void DamagePlayer(GameObject player)
    {
        Debug.Log("Boss damaged player!");
        
        // Apply knockback to player
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            Vector3 knockbackDirection = (player.transform.position - transform.position).normalized;
            playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
        }
        
        // Damage the player if they have a health system
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }
        else
        {
            Debug.LogWarning("Player does not have PlayerHealth component!");
        }
    }
    
    // Public method to force boss to take damage (for testing)
    public void ForceTakeDamage()
    {
        TakeDamage();
    }
    
    private void OnDrawGizmosSelected()
    {
        // Draw movement range
        Gizmos.color = Color.yellow;
        Vector3 startPos = Application.isPlaying ? startPosition : transform.position;
        Gizmos.DrawLine(startPos + Vector3.left * movementRange, startPos + Vector3.right * movementRange);
        
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 5f);
    }
} 