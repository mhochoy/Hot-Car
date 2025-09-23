using UnityEngine;

public class Oil : MonoBehaviour
{
    [Tooltip("The amount of sliding put upon whatever car is within the trigger zone.")]
    public float TurningInfluence;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Movement movement = other.GetComponentInParent<Movement>();

            if (movement)
            {
                movement.turningInfluence = TurningInfluence;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Movement movement = other.GetComponentInParent<Movement>();

            if (movement)
            {
                movement.turningInfluence = 0.00f;
            }
        }
    }
}
