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
            Destroy(other.gameObject);
        }
    }
}
