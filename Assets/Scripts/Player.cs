using System;// using this for rounding the decimal for the damage system.
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class PlayerTreasure: MonoBehaviour
{
    public double totalTreasure; // The only player value for now, there will be more.
    private TextMeshProUGUI coinText; // This variable allows for the total amount of treasure to be displayed in game.
    public GameObject player;
    public GameObject gold;
    public GameObject silver;
    public GameObject bronze;
    public AudioClip damageTaken;

    void Awake()
    {
        
    }

    void Start()
    {
        coinText = GameObject.FindWithTag("CoinText").GetComponent<TextMeshProUGUI>();
        coinText.text = totalTreasure.ToString();
    }
    public void RemoveAndScatterCoins(bool directionRight) // Function meant to be added later.
    {
        double treasureLost = totalTreasure;
        totalTreasure = Math.Floor(totalTreasure * 0.5);
        treasureLost -= totalTreasure;
        for (int i = 0; i < treasureLost; i++)
        {
            GameObject drop = Instantiate(bronze, player.transform.position, player.transform.rotation);
            TreasureItem treasure = drop.GetComponent<TreasureItem>();
            treasure.Dropped(directionRight);
        }
        coinText.text = totalTreasure.ToString();
    }
    public void AddCoins(int treasure) // Function meant to be added later.
    {
        Debug.Log("coin collected");
        totalTreasure += treasure;
        coinText.text = totalTreasure.ToString();
    }
    private void OnCollisionEnter2D(Collision2D collision) 
    {
        if(collision.gameObject.CompareTag("Lose Treasure")) // The damage system, incredibly basic but it will be added to later (nah it'll be replaced with something better lol)
        {
            // peeDeeAudio.PlayOneShot(damageTaken);
        }
    } 
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if(collision.gameObject.tag == "Gain Treasure") // The coin collection system, this is basic as well but it does well
        {
            TreasureItem treasure = collision.gameObject.GetComponent<TreasureItem>();
            totalTreasure += treasure.treasureAmount;
            coinText.text = totalTreasure.ToString();
        }
    }
}
