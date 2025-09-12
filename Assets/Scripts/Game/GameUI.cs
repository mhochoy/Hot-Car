using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    public TMP_Text LapText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLapText(string text)
    {
        LapText.text = text;
    }
}
