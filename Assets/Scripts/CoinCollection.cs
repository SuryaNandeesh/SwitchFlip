using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;

public class CoinCollection : MonoBehaviour
{

    private int Coin = 0;
    private int hs = 0;

    public TextMeshProUGUI coinText;
    public TextMeshProUGUI hsText;
    
    [Header("Healing Settings")]
    [Tooltip("Should collecting coins heal the player?")]
    public bool coinsHealPlayer = true;
    
    [Tooltip("How much health each coin restores")]
    public int healAmount = 1;

    private void Start()
    {
        hs = PlayerPrefs.GetInt("HighScore", 0);
        coinText.text = "Coins: " + Coin.ToString();
        hsText.text = "High Score: " + hs.ToString();
    }
    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Coin")
        {
            Coin++;
            coinText.text = "Coins: " + Coin.ToString();
            if(hs < Coin)
            {
                PlayerPrefs.SetInt("HighScore", Coin);
            }
            Debug.Log(Coin);
            
            // Heal player if enabled
            if (coinsHealPlayer)
            {
                PlayerHealth playerHealth = GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.Heal(healAmount);
                    Debug.Log($"Coin collected! Player healed {healAmount} health.");
                }
                else
                {
                    Debug.LogWarning("CoinCollection: Player does not have PlayerHealth component!");
                }
            }
            
            Destroy(other.gameObject);
        }
    }
    
    // Public method to get current coin count
    public int GetCoinCount()
    {
        return Coin;
    }
    
    // Public method to set coin count (useful for testing)
    public void SetCoinCount(int newCount)
    {
        Coin = newCount;
        coinText.text = "Coins: " + Coin.ToString();
    }
}
