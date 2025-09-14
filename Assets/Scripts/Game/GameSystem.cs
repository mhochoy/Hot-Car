using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    public enum GameState
    {
        InProgress,
        Completed,
        Countdown
    }
    GameState state;
    public static GameSystem instance;
    public int Laps;
    public float TotalTime;
    public bool IsGameOver { get; private set; }
    public Car Winner;
    GameUI gameUI;
    float originalTimeScale;
    PlayerCar playerCar;
    List<BotCar> botCars = new List<BotCar>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        gameUI = GetComponent<GameUI>();
        originalTimeScale = Time.timeScale;
        state = GameState.InProgress;

        GatherBots();
    }

    void Update()
    {
        playerCar = PlayerCar.instance? PlayerCar.instance : null;
        IsGameOver = (state == GameState.Completed);
        botCars = botCars.OrderByDescending((bot) => bot.CurrentLap).ToList();
        bool AllBotsAreDead = botCars.All((cars) => cars.isActiveAndEnabled == false);
        bool PlayerBeatTheFinalLapBeforeTheClosestBot = ((playerCar && playerCar.CurrentLap > Laps) && (botCars[0] && botCars[0].CurrentLap <= Laps));
        bool BotBeatTheFinalLapBeforeThePlayer = ((playerCar && playerCar.CurrentLap <= Laps) && (botCars[0] && botCars[0].CurrentLap > Laps));
        bool DidPlayerWin = (IsGameOver && PlayerBeatTheFinalLapBeforeTheClosestBot) || AllBotsAreDead;
        bool DidBotWin = (IsGameOver && BotBeatTheFinalLapBeforeThePlayer) || playerCar.IsDead;

        HandleUI();

        if (DidPlayerWin || DidBotWin)
        {
            bool done = false;
            if (DidBotWin && !done) // We don't want this to run every frame as it is a relatively expensive operation
            {
                Winner = Array.Find<BotCar>(botCars.ToArray(), (bot) => bot.CurrentLap > Laps);
                done = true;
            }
            else if (DidPlayerWin)
            {
                Winner = playerCar;
            }
            EndGame();
        }

        HandleGameExtras();
    }

    void HandleGameExtras()
    {
        if (state == GameState.InProgress)
        {
            if (Time.timeScale != originalTimeScale)
            {
                Time.timeScale = originalTimeScale;
            }
            TotalTime += Time.deltaTime;
        }
        else if (state == GameState.Completed) 
        {
            Time.timeScale = .25f;
            Camera.main.transform.position += Camera.main.transform.forward * Time.deltaTime;
            gameUI.DisableLapText();
            gameUI.DisableLeaderText();
            gameUI.SetWinnerText($"{Winner.name} won!");

            StartCoroutine(End());
        }
    }

    void HandleUI()
    {
        gameUI.SetLapText($"Lap: {playerCar.CurrentLap}/{Laps}");

        if (botCars[0].CurrentLap > playerCar.CurrentLap)
        {
            gameUI.SetLeaderText($"{botCars[0].name} is leading!");
        }
        else if (playerCar.CurrentLap > botCars[0].CurrentLap)
        {
            gameUI.SetLeaderText($"You are leading!");
        }
        else if (playerCar.CurrentLap ==  botCars[0].CurrentLap)
        {
            gameUI.SetLeaderText($"You and {botCars[0].name} are tied!");
        }
    }

    void GatherBots()
    {
        GameObject[] bots = GameObject.FindGameObjectsWithTag("Bot");

        foreach (var bot in bots)
        {
            botCars.Add(bot.GetComponentInChildren<BotCar>());
        }
    }

    IEnumerator<WaitForSeconds> End()
    {
        yield return new WaitForSeconds(3);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    void EndGame()
    {
        state = GameState.Completed;
    }
}
