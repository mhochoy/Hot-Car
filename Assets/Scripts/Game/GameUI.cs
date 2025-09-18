using UnityEngine;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;

public class GameUI : MonoBehaviour
{
    public Animator animator;
    public TMP_Text LapText;
    public TMP_Text LeaderText;
    public TMP_Text WinnerText;
    public TMP_Text TimeText;
    public TMP_Text HealthText;
    public TMP_Text CurrentDamagePotentialText;
    public TMP_Text CurrentBoostText;
    public TMP_Text CurrentCarCountText;
    [SerializeField] List<AudioClip> TimeSounds = new List<AudioClip>();
    [SerializeField] AudioClip LapSound;
    AnimatorStateInfo animatorStateInfo;
    [SerializeField] AudioSource sound;

    void Awake()
    {
        animator = GetComponent<Animator>();
        animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        WinnerText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
    }

    public bool IsInCountdown()
    {
        return animatorStateInfo.IsName("Countdown");
    }

    public void SetLapText(string text)
    {
        LapText.text = text;
    }

    public void SetLeaderText(string text)
    {
        LeaderText.text = text;
    }

    public void SetWinnerText(string text, float time)
    {
        WinnerText.gameObject.SetActive(true);
        WinnerText.text = text;
        TimeText.text = $"total time: {time.ToString("0.0")}/s";
    }

    public void SetHealthText(float value)
    {
        HealthText.text = $"Health: {value.ToString("0.00")}";
    }

    public void SetDamagePotentialText(float value)
    {
        CurrentDamagePotentialText.text = $"Damage Potential: {value.ToString("0.00")}";
    }

    public void SetBoostText(Boost boost)
    {
        string boostType = boost && boost is DamageBoost ? "Damage" : boost && boost is SpeedBoost ? "Speed" : "None";

        CurrentBoostText.text = $"Current Boost: {boostType} - {boost?.timer}";
    }

    public void SetCourseInformationText(int alive, int total)
    {
        CurrentCarCountText.text = $"Cars: {alive} Alive / {total} Total / {total - alive} Destroyed";
    }

    public void EnableLapText()
    {
        LapText.gameObject.SetActive(true);
    }

    public void EnableLeaderText()
    {
        LeaderText.gameObject.SetActive(true);
    }

    public void EnableHealthText()
    {
        HealthText.gameObject.SetActive(true);
    }

    public void EnableDamagePotentialText()
    {
        CurrentDamagePotentialText.gameObject.SetActive(true); 
    }

    public void EnableCurrentBoostText()
    {
        CurrentBoostText.gameObject.SetActive(true);
    }

    public void EnableCourseInformationText()
    {
        CurrentCarCountText.gameObject.SetActive(true);
    }

    public void DisableLapText()
    {
        LapText.gameObject.SetActive(false);
    }

    public void DisableLeaderText()
    {
        LeaderText.gameObject.SetActive(false);
    }

    public void DisableHealthText()
    {
        HealthText.gameObject.SetActive(false);
    }

    public void DisableDamagePotentialText()
    {
        CurrentDamagePotentialText.gameObject.SetActive(false);
    }

    public void DisableCurrentBoostText()
    {
        CurrentBoostText.gameObject.SetActive(false);
    }

    public void DisableCourseInformationText()
    {
        CurrentCarCountText.gameObject.SetActive(false);
    }

    public void PlayTimeTick()
    {
        sound.PlayOneShot(TimeSounds[0]);
    }

    public void PlayTimeTock()
    {
        sound.PlayOneShot(TimeSounds[1]);
    }

    public void TickLapNoise()
    {
        sound.PlayOneShot(LapSound);
    }
}
