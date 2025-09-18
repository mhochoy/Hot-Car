using UnityEngine;

[RequireComponent(typeof(Health))]
public class PlayerCar : Car
{
    public static PlayerCar instance;
    public Controls controls;

    protected override void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        base.Awake();
    }

    protected override void FixedUpdate()
    {
        Damage = movement.DamagePotential;
        base.FixedUpdate();
        if (controls.Lock)
        {
            return;
        }
        if (!controls.accelerate && !controls.brake)
        {
            //base.PlayInterruptingLoopSound(carSounds.Idle);
        }
        if (controls.accelerate)
        {
            //base.PlayInterruptingLoopSound(carSounds.AccelerationLoop);
            movement.Accelerate(controls.turn, (CurrentBoost && CurrentBoost is SpeedBoost) ? base.CurrentBoost.value * Speed : Speed, TurnSpeed);
        }
        if (controls.deaccelerate)
        {
            //base.PlayInterruptingSound(carSounds.Deacceleration);
        }

        if (controls.brake)
        {
            movement.Reverse(controls.turn, (CurrentBoost && CurrentBoost is SpeedBoost) ? base.CurrentBoost.value * Speed : Speed, TurnSpeed);
        }
    }

    protected override void Death()
    {
        base.Death();
        Camera.main.transform.parent = null;
        gameObject.SetActive(false);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        BotCar botCar = collision.gameObject.GetComponent<BotCar>();
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        Health otherHealth = collision.gameObject.GetComponent<Health>();

        GameFX.instance.SpawnImpactEffect(collision.GetContact(0).point);

        if (botCar)
        {
            if (Damage > botCar.Damage && !controls.Lock) // Locked controls would indicate that the game isn't in its normal playable state (i.e.
                                                          // the game is over or the countdown is still active
            {
                botCar.health.Damage((Damage - botCar.Damage) * 2.5f * ((CurrentBoost && CurrentBoost is DamageBoost) ? CurrentBoost.value : 1f));
            }
        }
        else if (!botCar && otherHealth)
        {
            otherHealth.Damage(Damage * ((CurrentBoost && CurrentBoost is DamageBoost) ? CurrentBoost.value : 1f));
        }

        else if (!botCar && !otherHealth && !collision.gameObject.CompareTag("Prop"))
        {
            health.Damage(Damage * ((CurrentBoost && CurrentBoost is DamageBoost) ? CurrentBoost.value : 1f) / 4);
        }

        if (collision.gameObject.CompareTag("Prop"))
        {
            rb?.AddRelativeForce(transform.right * Damage * 3 * ((CurrentBoost && CurrentBoost is DamageBoost) ? CurrentBoost.value : 1f), ForceMode.Impulse);
        }
        
    }
}
