using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    public float DamagePotential;

    // Update is called once per frame
    void Update()
    {
        DamagePotential = (rb.linearVelocity + rb.angularVelocity).magnitude;

        Debug.Log("The current damage potential is: " + DamagePotential);
    }

    public void Accelerate(float horiz, float Speed, float TurnSpeed)
    {
        // Apply Engine Force
        Vector2 engineForce = transform.up * Speed;
        rb.AddRelativeForce(engineForce, ForceMode.Acceleration);

        // Apply Steering
        float turn = horiz * TurnSpeed * (rb.linearVelocity.magnitude / 10f);
        rb.angularVelocity = new Vector3(0, turn, 0);
    }

    public void Reverse(float horiz, float Speed, float TurnSpeed)
    {
        Vector2 engineForce = (-transform.up * Speed) / 2;
        rb.AddRelativeForce(engineForce, ForceMode.Acceleration);

        float turn = horiz * TurnSpeed * (rb.linearVelocity.magnitude / 10f);
        rb.angularVelocity = new Vector3(0, -turn, 0);
    }
}
