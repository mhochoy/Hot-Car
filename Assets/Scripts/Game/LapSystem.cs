using UnityEngine;

public class LapSystem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameSystem.instance.NextLap();
        }
    }
}
