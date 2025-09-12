using UnityEngine;

public class GameSystem : MonoBehaviour
{
    public static GameSystem instance;
    public int Laps;
    public int CurrentLap = 1;
    public float TotalTime;
    public bool IsGameOver { get; private set; }
    GameUI gameUI;
    bool done = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        gameUI = GetComponent<GameUI>();
    }

    void Update()
    {
        IsGameOver = done;
        if (done)
        {
            TotalTime += Time.deltaTime;
        }

        gameUI.SetLapText($"Lap: {CurrentLap}/{Laps}");
    }

    public void NextLap()
    {
        if (CurrentLap < Laps)
        {
            CurrentLap++;
        }
        else
        {
            EndGame();
        }
    }

    void EndGame()
    {
        done = true;
    }
}
