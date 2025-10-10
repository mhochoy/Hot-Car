using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [Header("Properties")]
    [Tooltip("The order of which the waypoint is set from the starting point.")]
    public int level;
    [Tooltip("The waypoint immediately following this one. Used to guide bot cars along the track, and track player's course completion.")]
    public Waypoint next;

    private void Update()
    {
        Debug.DrawLine(transform.position, next.transform.position, Color.red);
    }

    private void OnTriggerEnter(Collider other)
    {
        Car car = other.GetComponent<Car>();

        if (car)
        {
            if (car is PlayerCar)
            {
                if (gameObject.name != "LapTrigger")
                {
                    car.gameObject.SendMessage("AddCompletedWaypoints", this);
                }
                car.gameObject.SendMessage("SetNextWaypoint", next);
            }

            if (car is BotCar)
            {
                if (gameObject.name != "LapTrigger")
                {
                    car.gameObject.SendMessage("AddCompletedWaypoints", this);
                }
                BotCar bot = (BotCar)car;
                
                bot.SendMessage("SetNextWaypoint", next);
            }
        }
    }
}
