using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public int level;
    public Waypoint next;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            other.gameObject.SendMessage("SetNextWaypoint", next);
        }
        if (other.CompareTag("Bot"))
        {
            BotCar car = other.GetComponent<BotCar>();
            if (car)
            {
                car.Go(next.transform.position);
            }
        }
    }
}
