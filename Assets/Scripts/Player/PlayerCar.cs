using UnityEngine;

public class PlayerCar : MonoBehaviour
{
    public float Damage;
    public float Speed;
    public float TurnSpeed;
    public Controls controls;
    public Movement movement;
    public CarSoundbank carSounds;
    Transform car;
    AudioSource sound;

    void Awake()
    {
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
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        Health health = collision.gameObject.GetComponent<Health>();

        if (rb && collision.transform.tag != "Car")
        {
            rb.AddForce(car.up * Damage, ForceMode.Impulse);
        }

        if (health)
        {
            health.Damage(Damage);
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

    void PlaySound(AudioClip clip)
    {
        sound.PlayOneShot(clip);
    }
}
