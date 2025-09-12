using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CarSoundbank : MonoBehaviour
{
    public AudioClip Idle;
    public AudioClip Acceleration;
    public AudioClip AccelerationLoop;
    public AudioClip Deacceleration;
    public AudioClip Crash;
    public AudioClip Pass;
    public AudioClip TireScratch;

    void Awake()
    {
        
    }

    void Update()
    {
        
    }
}
