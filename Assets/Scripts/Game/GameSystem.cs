using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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

        GameObject[] bots = GameObject.FindGameObjectsWithTag("Bot");

        foreach (var bot in bots)
        {
            botCars.Add(bot.GetComponent<BotCar>());
        }
    }

    void Update()
    {
        playerCar = PlayerCar.instance? PlayerCar.instance : null;
        IsGameOver = done;
        botCars = botCars.OrderByDescending((bot) => bot.CurrentLap).ToList();
        BotCar winningBot = Array.Find<BotCar>(botCars.ToArray(), (bot) => bot.CurrentLap == Laps);
        bool DidPlayerWin = IsGameOver && ((playerCar && playerCar.CurrentLap > Laps) && (botCars[0] && botCars[0].CurrentLap <= Laps));
        bool DidBotWin = IsGameOver && ((playerCar && playerCar.CurrentLap <= Laps) && (botCars[0] && botCars[0].CurrentLap > Laps));

        HandleUI();

        if (playerCar && playerCar.CurrentLap > Laps || botCars[0] && botCars[0].CurrentLap > Laps)
        {
            EndGame();
        }

        if (!done)
        {
            if (Time.timeScale != originalTimeScale)
            {
                Time.timeScale = originalTimeScale;
            }
            TotalTime += Time.deltaTime;
        }
        else
        {
            Time.timeScale = .25f;
            Camera.main.transform.position += Camera.main.transform.forward * Time.deltaTime;
            gameUI.DisableLapText();
            gameUI.DisableLeaderText();
            if (DidPlayerWin)
            {
                gameUI.SetWinnerText("You won!");
            }
            else if  (DidBotWin)
            {
                gameUI.SetWinnerText($"{winningBot.name} won!");
            }

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
        done = true;
    }
}
