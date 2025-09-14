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
    public Car Leader;
    Car Winner;
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

    void FixedUpdate()
    {
        playerCar = PlayerCar.instance? PlayerCar.instance : null;
        IsGameOver = (state == GameState.Completed);
        botCars = botCars.OrderByDescending((bot) => bot.CurrentLap).ToList();
        List<BotCar> aliveBots = Array.FindAll(botCars.ToArray(), (car) => car.IsDead == false).ToList();
        bool AllBotsAreDead = botCars.All((cars) => cars.isActiveAndEnabled == false);
        bool OneBotIsLeft = (aliveBots.Count <= 1);
        bool PlayerBeatTheFinalLapBeforeTheClosestBot = ((playerCar && playerCar.CurrentLap > Laps) && (botCars[0] && botCars[0].CurrentLap <= Laps));
        bool BotBeatTheFinalLapBeforeThePlayer = ((playerCar && playerCar.CurrentLap <= Laps) && (botCars[0] && botCars[0].CurrentLap > Laps));
        bool DidPlayerWin = PlayerBeatTheFinalLapBeforeTheClosestBot || AllBotsAreDead;
        bool DidBotWin = (BotBeatTheFinalLapBeforeThePlayer || (playerCar.IsDead && OneBotIsLeft));
        bool done = false;

        if ((DidPlayerWin || DidBotWin) && !done)
        {
            if (DidBotWin) // We don't want this to run every frame as it is a relatively expensive operation
            {
                Car car = Array.Find<BotCar>(botCars.ToArray(), (bot) => bot.CurrentLap > Laps);
                if (!car)
                {
                    // The player and other bots have died, so find the surviving bot and call them a winner
                    Winner = aliveBots[0];
                }
                else if (car)
                {
                    Winner = car;
                }
            }
            else if (DidPlayerWin)
            {
                Winner = playerCar;
            }
            EndGame();

            done = true; // We do this so we only run this 'if' once
        }
        else if (!DidPlayerWin && !DidBotWin)
        {
            DetermineLeader();
        }

        HandleUI();
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
            gameUI.SetWinnerText($"{Winner.name} won!", TotalTime);

            StartCoroutine(End());
        }
    }

    void HandleUI()
    {
        gameUI.SetLapText($"Lap: {playerCar.CurrentLap}/{Laps}");
        gameUI.SetLeaderText($"{(Leader ? Leader.name : "Nobody")} is leading!");
    }

    void DetermineLeader()
    {
        // Calculate who is in the lead

        Waypoint playerNextWaypoint = playerCar.GetNextWaypoint();
        Waypoint leadingBotNextWaypoint = botCars[0].GetNextWaypoint();
        bool PlayerOverlappingBot = playerCar.CurrentLap > botCars[0].CurrentLap;
        bool BotOverlappingPlayer = playerCar.CurrentLap < botCars[0].CurrentLap;
        bool PlayerAndBotAreEven = playerCar.CurrentLap == botCars[0].CurrentLap;
        bool OneOverlappingAnother = (PlayerOverlappingBot || BotOverlappingPlayer);

        if ((playerNextWaypoint && leadingBotNextWaypoint)) // Ensure that these are not null before working with them
        {
            if (PlayerOverlappingBot)
            {
                Leader = playerCar;
            }
            if (BotOverlappingPlayer)
            {
                Leader = botCars[0];
            }
            if (PlayerAndBotAreEven && !OneOverlappingAnother)
            {
                bool PlayerOnFartherCheckpointThanBot = playerNextWaypoint.level > leadingBotNextWaypoint.level;
                bool BotOnFartherCheckpointThanPlayer = playerNextWaypoint.level < leadingBotNextWaypoint.level;
                bool PlayerAndBotOnSameCheckpoint = playerNextWaypoint.level == leadingBotNextWaypoint.level;
                bool OneFartherThanAnother = (PlayerOnFartherCheckpointThanBot || BotOnFartherCheckpointThanPlayer);
                if (OneOverlappingAnother)
                {
                    return;
                }
                if (PlayerOnFartherCheckpointThanBot && !OneOverlappingAnother)
                {
                    Leader = playerCar;
                }
                if (BotOnFartherCheckpointThanPlayer && !OneOverlappingAnother)
                {
                    Leader = botCars[0];
                }
                if (PlayerAndBotOnSameCheckpoint && !OneFartherThanAnother)
                {
                    bool PlayerClosestToNextCheckpoint = playerCar.DistanceFromNextWaypoint < botCars[0].DistanceFromNextWaypoint;
                    bool BotClosestToNextCheckpoint = playerCar.DistanceFromNextWaypoint > botCars[0].DistanceFromNextWaypoint;
                    if (OneFartherThanAnother)
                    {
                        return;
                    }
                    if (PlayerClosestToNextCheckpoint)
                    {
                        Leader = playerCar;
                    }
                    if (BotClosestToNextCheckpoint)
                    {
                        Leader = botCars[0];
                    }
                }
            }
        }
        else
        {
            // Neither player or bot have crossed the first checkpoint yet, therefore, technically, nobody is leading...
            Leader = null;
        }
    }

    void GatherBots()
    {
        GameObject[] bots = GameObject.FindGameObjectsWithTag("Bot");

        foreach (var bot in bots)
        {
            if (bot.GetComponent<BotCar>() == null)
            {
                continue;
            }
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
