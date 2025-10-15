using UnityEngine;

public class RacePreferences : ScriptableObject
{
    public GameObject PlayerCar;
    public int TotalRacers = 3;
    public bool DealDamage = true;
    public RaceType raceType = RaceType.Default;
    public enum RaceType
    {
        Default,
        RaceOnly,
        Demolition
    }
}
