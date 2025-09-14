using Unity.VisualScripting;
using UnityEngine;

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
    public bool IsDead { get; protected set; }
    public float DistanceFromNextWaypoint;
    protected Vector3 currentVelocity; // Really meant for use with the BotCar
    AudioSource sound;
    float originalPitch;
    Waypoint nextWaypoint;
    Rigidbody physics;


    protected virtual void Awake()
    {
        health = GetComponent<Health>();
        movement = GetComponentInParent<Movement>();
        sound = GetComponent<AudioSource>();
        carSounds = GetComponent<CarSoundbank>();
        physics = GetComponent<Rigidbody>();
        originalPitch = sound.pitch;
    }

    protected virtual void FixedUpdate()
    {
        IsDead = health.value <= 0;
        DistanceFromNextWaypoint = nextWaypoint ? Vector3.Distance(this.transform.position, nextWaypoint.transform.position) : 0.00f;

        if (movement.currentLinearVelocity != Vector3.zero)
        {
            float newPitch = Mathf.Clamp((((this is PlayerCar) ? movement.currentLinearVelocity.magnitude - movement.currentAngularVelocity.magnitude : currentVelocity.magnitude) ) * .05f, .9f, Mathf.Infinity);
            sound.pitch = newPitch;
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
        Car otherCar = collision.gameObject.GetComponent<BotCar>();
        Health otherHealth = collision.gameObject.GetComponent<Health>();

        GameFX.instance.SpawnImpactEffect(collision.GetContact(0).point);

        if (otherCar && Damage > otherCar.Damage)
        {
            otherCar.health.Damage((Damage - otherCar.Damage) * 2.5f);
        }
        else if (!otherCar && otherHealth)
        {
            otherHealth.Damage(Damage);
        }

        else if (!otherCar && !otherHealth && !collision.gameObject.CompareTag("Prop"))
        {
            health.Damage(Damage / 8);
        }

        if (collision.gameObject.CompareTag("Prop"))
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
        IsDead = true;
        GameFX.instance.SpawnExplosion(transform.position);
        GameFX.instance.SpawnSmokeStreamEffect(transform.position);
        OnDeath();
    }


    // Sounds


    protected void PlaySound(AudioClip clip)
    {
        sound.PlayOneShot(clip);
    }

    protected void PlayInterruptingSound(AudioClip clip)
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

    protected void PlayLoopingSound(AudioClip clip)
    {
        sound.clip = clip;
        if (!sound.loop)
        {
            sound.loop = true;
        }

        sound.Play();
    }

    protected void PlayInterruptingLoopSound(AudioClip clip)
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

    public Waypoint GetNextWaypoint()
    {
        return nextWaypoint;
    }
}
