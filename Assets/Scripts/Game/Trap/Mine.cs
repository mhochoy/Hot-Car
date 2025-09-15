using UnityEngine;

public class Mine : MonoBehaviour
{
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
