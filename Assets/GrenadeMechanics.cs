using System.Collections;
using System.Data;
using UnityEngine;
using UnityEngine.Events;

public class Grenade : MonoBehaviour
{
    public float fireSpeed = 4f;
    public float spinSpeed = 200f;
    public float kaboomTime = 0.4f;
    public Sprite explosion;
    private float destroyZone = -15;
    private Rigidbody2D rb;
    private CircleCollider2D cc;
    private SpriteRenderer spriteRenderer;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        // Move cannonball
        transform.position = transform.position + (Vector3.left * fireSpeed) * Time.deltaTime;

        //Spin cannonball
        transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);

        // Destroy when reached destroy zone
        if(Camera.main.WorldToScreenPoint(transform.position).x < destroyZone)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            int ignoreFloor = LayerMask.NameToLayer("Ground"); // Must force the grenade to ignore the floor due to the explosion being force up.
            fireSpeed = 0f;
            spinSpeed = 0f;
            rb.gravityScale = 0f;
            rb.excludeLayers = 1 << ignoreFloor;
            StartCoroutine(Kaboom());
        }
    }

    private IEnumerator Kaboom() // Creates an explosion immediately after the grenade has hit the floor. Doesn't last long.
    {
        int ignoreFloor = LayerMask.NameToLayer("Ground"); // Must force the grenade to ignore the floor due to the explosion being forced up if left unignored.
        fireSpeed = 0f;
        spinSpeed = 0f;
        yield return new WaitForSeconds(0.1f); // Aligns the grenade properly so it detonates in the right place. Not ideal, but it works.
        rb.gravityScale = 0f;
        rb.excludeLayers = 1 << ignoreFloor;
        cc.radius = 0.5f;
        spriteRenderer.sprite = explosion;
        transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        transform.localScale = new Vector3(1.5f, 1.5f, 1f);
        yield return new WaitForSeconds(kaboomTime);
        Destroy(gameObject);
    }
}