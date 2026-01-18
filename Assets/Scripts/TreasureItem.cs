using System.Collections;
using System.Data;
using UnityEngine;
using UnityEngine.Events;

public class TreasureItem : MonoBehaviour // Meant to be used for a test case, testing
{
    public int treasureAmount = 1;
    private Rigidbody2D rb;
    private CircleCollider2D cc;
    private AudioSource coinAudio;
    public float blinkAfterDamageTime = 3f;
    public float disappearAfterBlinkingTime = 3f;
    private bool coinCollected = false;
    private bool isCollectible = true;

    void Awake()
    {
        coinAudio = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CircleCollider2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    void Start()
    {
        
    }
    void Update()
    {
        if(coinCollected && !coinAudio.isPlaying)
        {
            Destroy(gameObject);
        }
    }
    public void Dropped(bool directionRight)
    {   
        //cc.isTrigger = false;
        isCollectible = false;       
        rb.gravityScale = 3f;
        if (directionRight)
        {
            rb.linearVelocity = new Vector2(Random.Range(10f, 15f), 9f);
        }
        else
        {
            rb.linearVelocity = new Vector2(Random.Range(-15f, -10f), 9f);
        }
        StartCoroutine(CoinsLimited());
    }
    private void OnTriggerEnter2D(Collider2D collision) // I don't want to know why, but removing the treasure from the scene through this class was the only way to remove the coins.
    {
        if(isCollectible && !coinCollected)
        {
            if(collision.gameObject.tag == "Player")
            {
                coinAudio.Play();
                GetComponent<SpriteRenderer>().enabled = false;
                cc.enabled = false; // You can get multiple coins if you leave the circle collider on.
                coinCollected = true;
            }
        }
        
        if (collision.gameObject.tag == "Ground")
        {
            rb.linearVelocity = new Vector2(0,0);
            rb.gravityScale = 0f;
            //cc.isTrigger = true;
            isCollectible = true;
            StartCoroutine(CoinsLimited());
        }
    }
    /*private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            rb.linearVelocity = new Vector2(0,0);
            rb.gravityScale = 0f;
            //cc.isTrigger = true;
            isCollectible = true;
            StartCoroutine(CoinsLimited());
        }
    }*/
    private IEnumerator CoinsLimited()
    {
        double elapsed = 0f;
        yield return new WaitForSeconds(blinkAfterDamageTime);
        while (elapsed < disappearAfterBlinkingTime)
        {
            GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, 0.2f);
            yield return new WaitForSeconds(0.1f);
            GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, 1f);
            yield return new WaitForSeconds(0.1f);

            elapsed += 0.2f;
        }
        if (!coinAudio.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}