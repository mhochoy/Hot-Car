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
