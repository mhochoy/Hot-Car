using TMPro;
using UnityEngine;

public class HandleFinalReults : MonoBehaviour
{
    public GameResults results;

    // UI Elements
    public TMP_Text TimeText;
    public TMP_Text DamageTakenText;
    public TMP_Text CarsDestroyed;
    public TMP_Text FinalScore;

    void Awake()
    {
        results = (GameResults)Resources.Load("GameResults");

        TimeText.text = $"Total Time: {results.Time.ToString("0.0")}";
        DamageTakenText.text = $"Damage Taken: {results.DamageTaken}";
        CarsDestroyed.text = $"Cars Destroyed: {results.CarsDestroyed}";

        FinalScore.text = $"Final Score: {results.Score}";
    }
}
