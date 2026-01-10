using JetBrains.Annotations;
using UnityEngine;
using TMPro;
using System; // using this for rounding the decimal for the damage system.

public class Player: MonoBehaviour
{
    public double player_treasure; // The only player value for now, there will be more.
    private TextMeshProUGUI coinText; // This variable allows for the total amount of treasure to be displayed in game.
    private AudioSource peeDeeAudio;
    public AudioClip damageTaken;

    void Awake()
    {
        peeDeeAudio = GetComponent<AudioSource>();
    }

    void Start()
    {
        coinText = GameObject.FindWithTag("CoinText").GetComponent<TextMeshProUGUI>();
        coinText.text = player_treasure.ToString();
    }
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if(collision.gameObject.tag == "Lose Treasure") // The damage system, incredibly basic but it will be added to later
        {
            peeDeeAudio.PlayOneShot(damageTaken);
            player_treasure = Math.Floor(player_treasure * 0.75);
            coinText.text = player_treasure.ToString();
        }
        if(collision.gameObject.tag == "Gain Treasure") // The coin collection system, this is basic as well but it does well
        {
            TreasureItem treasure = collision.gameObject.GetComponent<TreasureItem>();
            player_treasure += treasure.treasureAmount;
            coinText.text = player_treasure.ToString();
        }
    }
}
