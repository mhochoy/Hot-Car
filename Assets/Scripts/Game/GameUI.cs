using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    public TMP_Text LapText;
    public TMP_Text LeaderText;
    public TMP_Text WinnerText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        WinnerText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLapText(string text)
    {
        LapText.text = text;
    }

    public void SetLeaderText(string text)
    {
        LeaderText.text = text;
    }

    public void SetWinnerText(string text)
    {
        WinnerText.gameObject.SetActive(true);
        WinnerText.text = text;
    }

    public void EnableLapText()
    {
        LapText.gameObject.SetActive(true);
    }

    public void EnableLeaderText()
    {
        LeaderText.gameObject.SetActive(true);
    }

    public void DisableLapText()
    {
        LapText.gameObject.SetActive(false);
    }

    public void DisableLeaderText()
    {
        LeaderText.gameObject.SetActive(false);
    }
}
