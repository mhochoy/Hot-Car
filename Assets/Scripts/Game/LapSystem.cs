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
            if (car == null)
            {
                car = other.GetComponentInParent<Car>();
            }

            if (car.GetCompletedWaypoints() + 1 == GameSystem.instance.GetMaxCheckpoints())
            {
                if (car is BotCar)
                {
                    car.SendMessage("ClearCompletedWaypoints");
                    car.SendMessage("NextLap");
                }                
                if (car is PlayerCar)
                {
                    car.SendMessage("ClearCompletedWaypoints");
                    car.SendMessage("NextLap");
                    GameSystem.instance.SendMessage("TickLapNoise");
                }
            }
        }
    }
}
