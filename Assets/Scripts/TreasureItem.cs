using UnityEngine;
using UnityEngine.UI;

public class TreasureItem : MonoBehaviour // Meant to be used for a test case, testing
{
    public int treasureAmount = 1;
    public bool hasGravity;

    private void Awake()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision) // I don't want to know why, but removing the treasure from the scene through this class was the only way to remove the coins.
    {
        if(collision.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
}