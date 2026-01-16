using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class CannonSpawner : MonoBehaviour
{
    public GameObject cannonBall;
    public float spawnRateMin = 1f;
    public float spawnRateMax = 2f;
    public int headLevelRate = 30;
    private float nextSpawn;
    private float timer = 0;

    // Spawn first set at the start of the game
    void Start()
    {
        spawn();
        nextSpawn = getNextSpawn();
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
            nextSpawn = getNextSpawn(); 
            timer = 0;
        }
        
    }

    // Spawn cannonball
    void spawn()
    {
        Instantiate(cannonBall, transform.position, transform.rotation);
    }

    // Get next cannonball spawn time
    private float getNextSpawn()
    {
        float spawn = Random.Range(spawnRateMin, spawnRateMax);

        return Random.Range(spawnRateMin, spawnRateMax);
    }

}
