using UnityEngine;

public class LapSystem : MonoBehaviour
{
    Waypoint waypoint;
    private void Awake()
    {
        waypoint = GetComponent<Waypoint>();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            Car car = other.GetComponent<Car>();

            if (car.GetCompletedWaypoints() + 1 == GameSystem.instance.MaxCheckpointLevels)
            {
                other.SendMessage("NextLap");
                other.SendMessage("ClearCompletedWaypoints");
                if (car is PlayerCar)
                {
                    GameSystem.instance.SendMessage("TickLapNoise");
                }
            }
        }
    }
}
