using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MapSettings", menuName = "Scriptable Objects/MapSettings")]
public class MapSettings : ScriptableObject
{
    public string mapName;
    [Tooltip("Where the car should be waiting on the map picker screen")]
    public Transform PlayerSpawnTransform;
    public List<Transform> BotSpawnTransforms = new List<Transform>();
    public Vector3 WaitingTransform;
    public int TotalRacers;
    public bool Traps;
    public GameObject Environment;
}
