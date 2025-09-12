using UnityEngine;

public class LapSystem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.SendMessage("NextLap");
        }

        if (other.CompareTag("Bot"))
        {
            other.SendMessage("NextLap");
        }
    }
}
