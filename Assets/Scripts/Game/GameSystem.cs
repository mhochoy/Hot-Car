using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour
{
    public enum GameState
    {
        InProgress,
        Completed,
        Countdown
    }
    GameState state;

    [Header("Components")]
    [ReadOnly(true)]
    public static GameSystem instance;
    public RacePreferences racePreferences;
    public MapSettings mapSettings;
    
    [Header("Race Properties")]
    public bool IsGameOver { get; private set; }
    public Car Leader;
    [Tooltip("The maximum amount of laps in this race.")]
    public int Laps;
    [Tooltip("The amount of time passed since the beginning of the race.")]
    public float TotalTime;

    // Private
    [SerializeField] BotRacers botRacers;
    [SerializeField] List<Transform> botSpawnPoints = new List<Transform>();
    int MaxCheckpointLevels;
    Car Winner = null;
    GameUI gameUI;
    float originalTimeScale;
    PlayerCar playerCar;
    List<BotCar> botCars = new List<BotCar>();
    List<Waypoint> allWaypoints = new List<Waypoint>();
    bool AnyCheckpoints;

    List<BotCar> aliveBots = new List<BotCar>();

    void Awake()
    {
        Setup();
        SpawnCars();
        GatherBots();
    }

    void Setup()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        gameUI = GetComponent<GameUI>();
        originalTimeScale = Time.timeScale;
        state = GameState.InProgress;
        SetupWaypoints();
    }

    void SetupWaypoints()
    {
        foreach (var waypoint in GameObject.FindGameObjectsWithTag("Waypoint"))
        {
            allWaypoints.Add(waypoint.GetComponent<Waypoint>());
        }

        allWaypoints = allWaypoints.OrderByDescending((waypoint) => waypoint.level).ToList();
        MaxCheckpointLevels = allWaypoints.Count;
        AnyCheckpoints = MaxCheckpointLevels > 0;
    }

    void SpawnCars()
    {
        GameObject player = Instantiate(racePreferences.PlayerCar, mapSettings.PlayerSpawnTransform.position, mapSettings.PlayerSpawnTransform.rotation);
        for (int i = 0; i <= racePreferences.TotalRacers - 1; i++) // We subtract one because the player has already been spawned
        {
            int index = UnityEngine.Random.Range(0, botRacers.Bots.Count);
            GameObject bot = Instantiate(botRacers.Bots[index], botSpawnPoints[i].localPosition, botSpawnPoints[i].localRotation);

            bot.GetComponentInChildren<BotCar>().FirstWaypoint = allWaypoints[allWaypoints.Count - 1];
        }
    }

    void WakePlayerCar()
    {
        if (playerCar && !playerCar.enabled)
        {
            playerCar.enabled = true;
        }
    }

    public int GetMaxCheckpoints()
    {
        return MaxCheckpointLevels;
    }

    private void Update()
    {
        if (playerCar.controls.Paused)
        {
            Time.timeScale = 0.00f;
        }
        else
        {
            if (state == GameState.InProgress)
            {
                Time.timeScale = originalTimeScale;
            }
            else
            {
                Time.timeScale = .2f;
                playerCar.controls.Lock = true;
            }
        }
    }

    void FixedUpdate()
    {
        playerCar = PlayerCar.instance? PlayerCar.instance : null;
        WakePlayerCar();

        if (!AnyCheckpoints) // Free Roam 
        {
            HandleUI();
            HandleCountdownLock();
            HandleGameExtras();
            return;
            // Free Roam
        }
        else // Race or Demolition
        {
            IsGameOver = (state == GameState.Completed);
            botCars = botCars.OrderByDescending((car) => car.GetCurrentLap()).ThenByDescending((car) => car.GetNextWaypoint()?.level).ThenBy((car) => car.GetDistanceFromNextWaypoint()).ToList();
            aliveBots = Array.FindAll(botCars.ToArray(), (car) => car.IsDead == false).ToList();
            bool AllBotsAreDead = botCars.All((cars) => cars.isActiveAndEnabled == false);
            bool OneBotIsLeft = (aliveBots.Count >= 1);
            bool PlayerBeatTheFinalLapBeforeTheClosestBot = ((playerCar && playerCar.GetCurrentLap() > Laps) && (botCars[0] && botCars[0].GetCurrentLap() <= Laps));
            bool BotBeatTheFinalLapBeforeThePlayer = ((playerCar && playerCar.GetCurrentLap() <= Laps) && (botCars[0] && botCars[0].GetCurrentLap() > Laps));
            bool DidPlayerWin = PlayerBeatTheFinalLapBeforeTheClosestBot || AllBotsAreDead;
            bool DidBotWin = (BotBeatTheFinalLapBeforeThePlayer || (playerCar.IsDead && OneBotIsLeft));
            bool done = false;

            if ((DidPlayerWin || DidBotWin) && !done)
            {
                if (DidBotWin) // We don't want this to run every frame as it is a relatively expensive operation
                {
                    FindWinningBot();
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
            HandleCountdownLock();
            HandleGameExtras();
        }
    }

    void HandleCountdownLock()
    {
        if (gameUI.IsInCountdown())
        {
            playerCar.controls.Lock = true;
            foreach (var car in botCars)
            {
                car.Stop = true;
            }
        }
        else
        {
            playerCar.controls.Lock = false;
            foreach (var car in botCars)
            {
                car.Stop = false;
            }
        }
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
            HideRaceUIElements();
            gameUI.SetWinnerText($"{Winner.name} won!", TotalTime);

            StartCoroutine(End());
        }
    }

    void HandleUI()
    {
        // Player
        gameUI.SetHealthText(playerCar.health.value);
        gameUI.SetDamagePotentialText(playerCar.Damage * ((playerCar.CurrentBoost && playerCar.CurrentBoost) ? playerCar.CurrentBoost.value : 1f));
        gameUI.SetBoostText(playerCar.CurrentBoost);

        // General
        gameUI.SetLapText($"Lap: {playerCar.GetCurrentLap()}/{Laps}");
        gameUI.SetLeaderText($"{(Leader ? Leader.name : "Nobody")} is leading!");
        gameUI.SetCourseInformationText(botCars.FindAll((car) => !car.IsDead).Count + (!playerCar.IsDead ? 1 : 0), botCars.Count + 1);
    }

    void FindWinningBot()
    {
        Car car = Array.Find<BotCar>(botCars.ToArray(), (bot) => bot.GetCurrentLap() > Laps);
        if (!car && Winner == null)
        {
            // The player and other bots have died, so find the surviving bot and call them a winner
            Winner = aliveBots[0];
        }
        else if (car && Winner == null)
        {
            Winner = car;
        }
    }

    void ShowRaceUIElements()
    {
        gameUI.EnableHealthText();
        gameUI.EnableDamagePotentialText();
        gameUI.EnableCurrentBoostText();

        gameUI.EnableLapText();
        gameUI.EnableLeaderText();
        gameUI.EnableCourseInformationText();
    }

    void HideRaceUIElements()
    {
        gameUI.DisableHealthText();
        gameUI.DisableDamagePotentialText();
        gameUI.DisableCurrentBoostText();

        gameUI.DisableLapText();
        gameUI.DisableLeaderText();
        gameUI.DisableCourseInformationText();
    }

    void DetermineLeader()
    {
        if (aliveBots.Count <= 0)
        {
            Leader = playerCar;
            return;
        }
        // Calculate who is in the lead
        Waypoint playerNextWaypoint = playerCar.GetNextWaypoint();
        Waypoint leadingBotNextWaypoint = aliveBots[0].GetNextWaypoint();
        BotCar leadingBot = aliveBots.OrderByDescending((car) => car.GetCurrentLap()).ToList()[0];
        bool PlayerOverlappingBot = playerCar.GetCurrentLap() > leadingBot.GetCurrentLap();
        bool BotOverlappingPlayer = playerCar.GetCurrentLap() < leadingBot.GetCurrentLap();
        bool PlayerAndBotAreOnSameLap = playerCar.GetCurrentLap() == leadingBot.GetCurrentLap();
        bool OneOverlappingAnother = (PlayerOverlappingBot || BotOverlappingPlayer);

        if ((playerNextWaypoint || leadingBotNextWaypoint)) // Ensure that these are not null before working with them
        {
            if (PlayerOverlappingBot && !PlayerAndBotAreOnSameLap)
            {
                Debug.Log("Player is leading because they are overlapping opponents");
                Leader = playerCar;
            }
            else if (BotOverlappingPlayer && !PlayerAndBotAreOnSameLap)
            {
                Leader = aliveBots[0];
            }
            else if (PlayerAndBotAreOnSameLap && !OneOverlappingAnother)
            {
                bool PlayerOnFartherCheckpointThanBot = playerCar.GetCompletedWaypoints() > leadingBot.GetCompletedWaypoints() && PlayerAndBotAreOnSameLap;
                bool BotOnFartherCheckpointThanPlayer = playerCar.GetCompletedWaypoints() < leadingBot.GetCompletedWaypoints() && PlayerAndBotAreOnSameLap;
                bool PlayerAndBotOnSameCheckpoint = playerCar.GetCompletedWaypoints() == leadingBot.GetCompletedWaypoints() && PlayerAndBotAreOnSameLap;
                bool OneFartherThanAnother = (PlayerOnFartherCheckpointThanBot || BotOnFartherCheckpointThanPlayer) && PlayerAndBotAreOnSameLap;

                if (OneOverlappingAnother)
                {
                    return;
                }
                else if (PlayerOnFartherCheckpointThanBot && !OneOverlappingAnother)
                {
                    Debug.Log("Player is leading because they have cleared more checkpoints");
                    Leader = playerCar;
                }
                else if (BotOnFartherCheckpointThanPlayer && !OneOverlappingAnother)
                {
                    Leader = leadingBot;
                }
                else if (PlayerAndBotOnSameCheckpoint && !OneFartherThanAnother)
                {
                    bool PlayerClosestToNextCheckpoint = playerCar.GetDistanceFromNextWaypoint() < botCars[0].GetDistanceFromNextWaypoint() && PlayerAndBotAreOnSameLap && !OneFartherThanAnother;
                    bool BotClosestToNextCheckpoint = playerCar.GetDistanceFromNextWaypoint() > botCars[0].GetDistanceFromNextWaypoint() && PlayerAndBotAreOnSameLap && !OneFartherThanAnother;

                    if (OneFartherThanAnother)
                    {
                        return;
                    }
                    else if (PlayerClosestToNextCheckpoint)
                    {
                        Debug.Log("Player is leading because they are closer to the nearest checkpoint");
                        Leader = playerCar;
                    }

                    else if (BotClosestToNextCheckpoint)
                    {
                        Leader = leadingBot;
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
            else
            {
                botCars.Add(bot.GetComponent<BotCar>());
            }
                
        }
    }

    IEnumerator<WaitForSeconds> End()
    {
        yield return new WaitForSeconds(3);
        Time.timeScale = originalTimeScale;
        SceneManager.LoadScene("MainMenu");
    }

    void EndGame()
    {
        state = GameState.Completed;
    }

    void TickLapNoise()
    {
        gameUI.TickLapNoise();
    }
}
