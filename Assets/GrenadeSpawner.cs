using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

// Similar to how cannonballs spawn, could be redundant. 
public class GrenadeSpawner : MonoBehaviour
{
    public GameObject grenada;
    public float spawnRate = 3f;
    public int headLevelRate = 30;
    private float nextSpawn;
    private float timer = 0;

    // Spawn first set at the start of the game
    void Start()
    {
        spawn();
        nextSpawn = spawnRate;
    }

    // Spawn cannonball when time is met
    void Update()
    {
        if(timer < nextSpawn)
        {
            timer += Time.deltaTime;
        }
        else
        {
            spawn();
            nextSpawn = spawnRate; 
            timer = 0;
        }
        
    }

    // Spawn cannonball
    void spawn()
    {
        Instantiate(grenada, transform.position, transform.rotation);
    }

}
