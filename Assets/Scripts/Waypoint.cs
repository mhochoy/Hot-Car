using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Waypoint next;

    private void OnTriggerEnter(Collider other)
    {
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
