using UnityEngine;

public class Health : MonoBehaviour
{
    public float value;

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
            SendMessage("Damaged");
        }
        else if (remainingHealth <= 0.00f)
        {
            Die();
        }
    }

    public void Die()
    {
        SendMessage("Death");
    }
}
