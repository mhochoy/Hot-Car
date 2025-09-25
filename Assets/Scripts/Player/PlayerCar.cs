using System.ComponentModel;
using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using System;

[RequireComponent(typeof(Controls))]
public class PlayerCar : Car
{
    public static PlayerCar instance;
    [Header("Player Components")]
    [Tooltip("The script that handles the recieving of input from a device.")]
    public Controls controls;
    int currentCamIndex = 0;
    int maxCams = 0;
    Camera currentCam;
    List<Camera> cameras = new List<Camera>();

    protected override void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        base.Awake();

        controls = GetComponent<Controls>();
        foreach (Transform ob in transform)
        {
            Camera cam = ob.GetComponent<Camera>();
            if (cam)
            {
                cameras.Add(cam);
            }
        }

        maxCams = cameras.Count - 1;
        if (currentCamIndex > maxCams)
        {
            currentCam = cameras[0];
        }
        currentCam = cameras[currentCamIndex];
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

    void Update()
    {
        if (controls.switchCam)
        {
            SwitchCamera();
        }
    }

    void SwitchCamera()
    {
        try
        {
            currentCamIndex++;
            Camera nextCam = cameras[currentCamIndex];
            currentCam.gameObject.SetActive(false);
            if (nextCam.gameObject.activeSelf == false)
            {
                nextCam.gameObject.SetActive(true);
            }

            currentCam = nextCam;
        }
        catch (ArgumentOutOfRangeException)
        {
            currentCamIndex = 0;
            currentCam.gameObject.SetActive(false);
            Camera baseCamera = cameras[currentCamIndex];
            if (baseCamera.gameObject.activeSelf == false)
            {
                baseCamera.gameObject.SetActive(true);
            }
            currentCam = baseCamera;
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
