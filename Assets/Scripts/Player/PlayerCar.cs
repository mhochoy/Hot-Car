using UnityEngine;

[RequireComponent(typeof(Health))]
public class PlayerCar : MonoBehaviour
{
    public static PlayerCar instance;
    public Health health;
    public float Damage;
    public float Speed;
    public float TurnSpeed;
    public Controls controls;
    public Movement movement;
    public CarSoundbank carSounds;
    public int CurrentLap = 1;
    public bool IsDead { get; private set; }
    Transform car;
    AudioSource sound;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        sound = GetComponent<AudioSource>();
        car = transform.GetChild(0);
        health = GetComponent<Health>();
    }

    void FixedUpdate()
    {
        Damage = movement.DamagePotential;
        controls.Lock = GameSystem.instance.IsGameOver;
        IsDead = health.value <= 0;
        if (!controls.accelerate && !controls.brake)
        {
            if (sound.isPlaying && sound.clip != carSounds.Idle)
            {
                sound.Stop();
            }
            if (!sound.isPlaying)
            {
                PlayLoopingSound(carSounds.Idle);
            }
        }
        if (controls.accelerate)
        {
            if (sound.isPlaying && sound.clip != carSounds.AccelerationLoop)
            {
                sound.Stop();
            }
            if (!sound.isPlaying) 
            {
                PlayLoopingSound(carSounds.AccelerationLoop);
            }
            movement.Accelerate(controls.turn, Speed, TurnSpeed);
        }
        if (controls.deaccelerate)
        {
            if (sound.isPlaying && sound.clip != carSounds.Deacceleration)
            {
                sound.Stop();
            }
            if (!sound.isPlaying)
            {
                PlaySound(carSounds.Deacceleration);
            }
        }

        if (controls.brake)
        {
            if (sound.isPlaying && sound.clip != carSounds.TireScratch)
            {
                sound.Stop();
            }
            if (!sound.isPlaying)
            {
                PlaySound(carSounds.TireScratch);
            }
            movement.Reverse(controls.turn, Speed, TurnSpeed);
        }
    }

    void Death()
    {
        GameFX.instance.SpawnExplosion(transform.position);
        GameFX.instance.SpawnSmokeStreamEffect(transform.position);
        Camera.main.transform.parent = null;
        car.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        BotCar botCar = collision.gameObject.GetComponent<BotCar>();
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        Health otherHealth = collision.gameObject.GetComponent<Health>();

        GameFX.instance.SpawnImpactEffect(collision.GetContact(0).point);

        if (collision.gameObject.layer == 6 || collision.gameObject.CompareTag("Prop"))
        {
            if (botCar)
            {
                if (Damage > botCar.Damage && !controls.Lock) // Locked controls would indicate that the game isn't in its normal playable state (i.e.
                                                              // the game is over or the countdown is still active
                {
                    botCar.health.Damage(Damage);
                }
            }
            else if (!botCar && otherHealth)
            {
                otherHealth.Damage(Damage);
            }
            else if (!botCar && !otherHealth)
            {
                health.Damage(Damage / 2);
            }

            sound.PlayOneShot(carSounds.Crash);
        }

        if (rb)
        {
            rb.AddRelativeForce(car.up * Damage * 3, ForceMode.Impulse);
        }
        
    }

    // Sounds

    void PlayLoopingSound(AudioClip clip)
    {
        sound.clip = clip;
        if (!sound.loop)
        {
            sound.loop = true;
        }

        sound.Play();
    }

    public void NextLap()
    {
        CurrentLap++;
    }

    void PlaySound(AudioClip clip)
    {
        sound.PlayOneShot(clip);
    }
}
