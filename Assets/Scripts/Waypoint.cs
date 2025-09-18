using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public int level;
    public Waypoint next;

    private void OnTriggerEnter(Collider other)
    {
        Car car = other.GetComponent<Car>();

        if (car)
        {
            if (car is PlayerCar)
            {
                Debug.Log($"Player has completed {car.GetCompletedWaypoints() + 1} checkpoints");
                Debug.Log($"Player needs {GameSystem.instance.MaxCheckpointLevels} to get to next lap");
                if (gameObject.name == "LapTrigger")
                {
                    return;
                }
            }

            car.gameObject.SendMessage("AddCompletedWaypoints", this);
            car.gameObject.SendMessage("SetNextWaypoint", next);

            if (car is BotCar)
            {
                BotCar bot = (BotCar)car;
                if (bot)
                {
                    bot.Go(next.transform.position);
                }
            }
        }
    }
}
