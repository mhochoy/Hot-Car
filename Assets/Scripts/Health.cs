using UnityEngine;

public class Health : MonoBehaviour
{
    public float value;
    public float DamageTaken;
    Car killer;
    void Update()
    {
        if (value == 0.00f)
        {
            SendMessage("Death");
        }
    }

    public void Give(float amount)
    {
        if (value + amount > 100.00f)
        {
            return;
        }
        else
        {
            value += amount;
        }
    }

    public void Damage(float damage)
    {
        float remainingHealth = value - damage;
        if (remainingHealth > 0.00f)
        {
            value -= damage;
            DamageTaken += damage;
            SendMessage("Damaged");
        }
        else if (remainingHealth <= 0.00f)
        {
            Die();
        }
    }

    public void Damage(Car car, float damage)
    {
        float remainingHealth = value - damage;
        if (remainingHealth > 0.00f)
        {
            value -= damage;
            DamageTaken += damage;
            SendMessage("Damaged");
        }
        else if (remainingHealth <= 0.00f)
        {
            killer = car;
            Die();
        }
    }

    public Car GetKiller()
    {
        return killer;
    }

    public float GetTotalDamage()
    {
        return this.DamageTaken;
    }

    public void Die()
    {
        SendMessage("Death");
    }
}
