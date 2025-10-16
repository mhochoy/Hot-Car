using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MapSettings", menuName = "Scriptable Objects/MapSettings")]
public class MapSettings : ScriptableObject
{
    public string mapName;
    public Transform PlayerSpawnTransform;
    public int TotalRacers;
    public bool Traps;
    public GameObject Environment;
}
