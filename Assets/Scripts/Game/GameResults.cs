using UnityEngine;

[CreateAssetMenu(fileName = "GameResults", menuName = "Scriptable Objects/GameResults")]
public class GameResults : ScriptableObject
{
    public Car WinnerCar;
    public float Time;
    public float DamageTaken;
    public int CarsDestroyed;

    public float Score;
}
