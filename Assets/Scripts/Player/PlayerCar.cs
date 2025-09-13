using UnityEngine;

public class PlayerCar : MonoBehaviour
{
    public static PlayerCar instance;
    public float Damage;
    public float Speed;
    public float TurnSpeed;
    public Controls controls;
    public Movement movement;
    public CarSoundbank carSounds;
    public int CurrentLap = 1;
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
    }

    void FixedUpdate()
    {
        Damage = movement.DamagePotential;
        controls.Lock = GameSystem.instance.IsGameOver;
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

    private void OnCollisionEnter(Collision collision)
    {
        BotCar botCar = collision.gameObject.GetComponent<BotCar>();
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        Health health = collision.gameObject.GetComponent<Health>();

        GameFX.instance.SpawnImpactEffect(collision.GetContact(0).point);

        if (collision.gameObject.layer == 6)
        {
            sound.PlayOneShot(carSounds.Crash);
        }

        if (rb)
        {
            rb.AddRelativeForce(car.up * Damage * 3, ForceMode.Impulse);
        }
        if (botCar)
        {
            if (Damage > botCar.Damage)
            {
                botCar.health.Damage(Damage);
            }
        }
        else if (!botCar)
        {
            if (health)
            {
                health.Damage(Damage);
            }
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
