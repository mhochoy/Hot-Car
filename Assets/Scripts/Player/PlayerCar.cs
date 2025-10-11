using System.ComponentModel;
using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using System;
using System.Linq;
using Unity.Cinemachine;

public class PlayerCar : Car
{
    public static PlayerCar instance;
    [Header("Player Components")]
    [Tooltip("The script that handles the recieving of input from a device.")]
    public Controls controls;

    // Cameras
    int currentCamIndex = 1;
    GameObject currentCam;
    List<GameObject> cameras = new List<GameObject>();
    CinemachineImpulseSource cameraImpulseSource;

    protected override void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        base.Awake();

        controls = GetComponentInParent<Controls>();

        cameras = GameObject.FindGameObjectsWithTag("MainCamera").ToList();
        cameraImpulseSource = GetComponentInChildren<CinemachineImpulseSource>();

        if (cameras.Count > currentCamIndex)
        {
            currentCam = cameras[currentCamIndex];
        }
        
    }

    protected override void FixedUpdate()
    {
        Damage = movement.DamagePotential + ((Math.Round(movement.currentLinearVelocity.z) != 0f ? physics.mass / 64f : 0f));
        base.FixedUpdate();
        if (controls.Lock || Grounded == false)
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
        if (!currentCam.activeSelf)
        {
            currentCam.SetActive(true);
        }
        foreach (GameObject cam in cameras)
        {
            if (cam != currentCam)
            {
                cam.SetActive(false);
            }
        }
        if (controls.switchCam)
        {
            Debug.Log("Switch Camera Button was pressed (E)");
            SwitchCamera();
        }
    }

    void SwitchCamera()
    {
        if (currentCamIndex + 1 > cameras.Count - 1)
        {
            currentCamIndex = 0;
        }
        else
        {
            currentCamIndex++;
        }

        currentCam = cameras[currentCamIndex];
    }

    protected override void Death()
    {
        base.Death();
        Camera.main.transform.parent = null;
        gameObject.SetActive(false);
    }

    protected override void OnLanding()
    {
        cameraImpulseSource.GenerateImpulse(new Vector3(0, -.175f));
    }

    void GenerateImpulse(Vector3 impulse)
    {
        
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        BotCar botCar = collision.gameObject.GetComponent<BotCar>();
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        Health otherHealth = collision.gameObject.GetComponent<Health>();

        if (botCar)
        {
            if (Damage > botCar.Damage && !controls.Lock) // Locked controls would indicate that the game isn't in its normal playable state (i.e.
                                                          // the game is over or the countdown is still active
            {
                botCar.health.Damage((Damage - botCar.Damage) * 2.5f * ((CurrentBoost && CurrentBoost is DamageBoost) ? CurrentBoost.value : 1f));
            }
            cameraImpulseSource.GenerateImpulse(new Vector3(.25f, -.25f));
        }
        else if (!botCar && otherHealth)
        {
            otherHealth.Damage(Damage * ((CurrentBoost && CurrentBoost is DamageBoost) ? CurrentBoost.value : 1f));
        }

        else if (collision.gameObject.layer == 7 || collision.gameObject.layer == 0 && Grounded)
        {
            GameFX.instance.SpawnImpactEffect(collision.GetContact(0).point);
            cameraImpulseSource.GenerateImpulse(new Vector3(0, -.25f));
            //health.Damage(Damage * ((CurrentBoost && CurrentBoost is DamageBoost) ? CurrentBoost.value : 1f) / 8);
        }

        else if (!botCar && !otherHealth && !collision.gameObject.CompareTag("Prop"))
        {
            //health.Damage(Damage * ((CurrentBoost && CurrentBoost is DamageBoost) ? CurrentBoost.value : 1f) / 4);
        }

        if (collision.gameObject.CompareTag("Prop"))
        {
            rb?.AddRelativeForce(transform.right * Damage * 3 * ((CurrentBoost && CurrentBoost is DamageBoost) ? CurrentBoost.value : 1f), ForceMode.Impulse);
        }
        
    }
}
