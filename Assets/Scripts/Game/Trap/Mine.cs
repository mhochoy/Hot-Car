using UnityEngine;

public class Mine : MonoBehaviour
{
    // This component can be used for any GameObject that will explode/give damage any time it is triggered.
    public float Damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6) // if gameObject is a car...
        {
            Health health = other.gameObject.GetComponent<Health>();
            GameFX.instance.SpawnExplosion(transform.position);
            if (health)
            {
                health.Damage(Damage);
            }
            gameObject.SetActive(false);
        }
    }
}
