using UnityEngine;

public class TreasureItem : MonoBehaviour // Meant to be used for a test case, testing
{
    public int treasureAmount = 1;
    private AudioSource coinAudio;
    private bool coinCollected = false;

    void Awake()
    {
        coinAudio = GetComponent<AudioSource>();
    }
    void Update()
    {
        if(coinCollected && !coinAudio.isPlaying)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision) // I don't want to know why, but removing the treasure from the scene through this class was the only way to remove the coins.
    {
        if(!coinCollected)
        {
            if(collision.gameObject.tag == "Player")
            {
                coinAudio.Play();
                GetComponent<SpriteRenderer>().enabled = false;
                coinCollected = true;
            }
        }
    }
}