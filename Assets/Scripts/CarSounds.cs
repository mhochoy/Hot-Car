using UnityEngine;

[CreateAssetMenu(fileName = "CarSounds", menuName = "Scriptable Objects/CarSounds")]
public class CarSounds : ScriptableObject
{
    public AudioClip Idle;
    public AudioClip Accelerate;
    public AudioClip AccelerateLoop;
    public AudioClip Deaccelerate;
    public AudioClip Crash;
    public AudioClip TireScratch;
}
