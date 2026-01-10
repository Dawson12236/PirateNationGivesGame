using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton Logic
    public static AudioManager Instance {  get; private set; }
    public AudioSource music;

    void Awake()
    {
        // Singleton Logic
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    //public PlayAudio
}
