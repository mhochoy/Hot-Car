using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(CarSoundbank))]
public class Car : MonoBehaviour
{
    public Health health;
    public float Damage;
    public float Speed;
    public float TurnSpeed;
    public Movement movement;
    public CarSoundbank carSounds;
    public int CurrentLap = 1;
    public bool IsDead { get; private set; }
    AudioSource sound;
    float originalPitch;
    Waypoint nextWaypoint;

    void Awake()
    {
        health = GetComponent<Health>();
        movement = GetComponent<Movement>();
        sound = GetComponent<AudioSource>();
        originalPitch = sound.pitch;
    }

    protected virtual void FixedUpdate()
    {
        Damage = movement.DamagePotential;
        IsDead = health.value <= 0;

        if (movement.currentLinearVelocity != Vector3.zero)
        {
            sound.pitch = movement.currentLinearVelocity.y;
            PlayInterruptingLoopSound(carSounds.AccelerationLoop);
        }
        else
        {
            sound.pitch = originalPitch;
            PlayInterruptingLoopSound(carSounds.Idle);
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();

        if (rb)
        {
            rb.AddRelativeForce(transform.up * Damage * 3, ForceMode.Impulse);
        }
    }

    public void NextLap()
    {
        CurrentLap++;
    }

    protected virtual void OnDeath()
    {

    }

    protected virtual void Death()
    {
        GameFX.instance.SpawnExplosion(transform.position);
        GameFX.instance.SpawnSmokeStreamEffect(transform.position);
        Camera.main.transform.parent = null;
        OnDeath();
    }


    // Sounds


    void PlaySound(AudioClip clip)
    {
        sound.PlayOneShot(clip);
    }

    void PlayInterruptingSound(AudioClip clip)
    {
        if (sound.isPlaying && sound.clip != clip)
        {
            sound.Stop();
        }
        if (!sound.isPlaying)
        {
            PlaySound(clip);
        }
    }

    void PlayLoopingSound(AudioClip clip)
    {
        sound.clip = clip;
        if (!sound.loop)
        {
            sound.loop = true;
        }

        sound.Play();
    }

    void PlayInterruptingLoopSound(AudioClip clip)
    {
        if (sound.isPlaying && sound.clip != clip)
        {
            sound.Stop();
        }
        if (!sound.isPlaying)
        {
            PlayLoopingSound(clip);
        }
    }

    // Message Events

    void SetNextWaypoint(Waypoint waypoint)
    {
        nextWaypoint = waypoint;
    }
}
