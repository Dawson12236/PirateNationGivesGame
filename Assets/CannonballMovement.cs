using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public float fireSpeed = 3f;
    private float destroyZone = -15;
    void Update()
    {
        // Move cannonball
        transform.position = transform.position + (Vector3.left * fireSpeed) * Time.deltaTime;

        // Destroy when reached destroy zone
        if(Camera.main.WorldToScreenPoint(transform.position).x < destroyZone)
        {
            Destroy(gameObject);
        }
    }
}
