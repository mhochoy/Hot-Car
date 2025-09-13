using UnityEngine;

public class GameFX : MonoBehaviour
{
    public static GameFX instance;

    public GameObject ExplosionEffect;
    public GameObject SmokeStreamEffect;
    public GameObject ImpactEffect;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void SpawnExplosion(Vector3 position)
    {
        GameObject explosion = Instantiate(ExplosionEffect, position, Quaternion.identity);
    }

    public void SpawnSmokeStreamEffect(Vector3 position)
    {
        GameObject smokeStream = Instantiate(SmokeStreamEffect, position, Quaternion.identity);
    }

    public void SpawnImpactEffect(Vector3 position)
    {
        GameObject impact = Instantiate(ImpactEffect, position, Quaternion.identity);
    }
}
