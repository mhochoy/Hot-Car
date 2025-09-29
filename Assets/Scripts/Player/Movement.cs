using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [Tooltip("The calculation of damage made from the magnitude (square root of x, y, z) of current linear and angular velocities.")]
    public float DamagePotential;
    [Tooltip("The speed at which the car is moving in a straight line.")]
    public Vector3 currentLinearVelocity;
    [Tooltip("The speed at which the car is moving on a curve.")]
    public Vector3 currentAngularVelocity;
    [Tooltip("The rate of influence of the turning velocity. (From things like traps).")]
    public float turningInfluence;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //car = transform.GetChild(0);
    }

    void Update()
    {
        DamagePotential = (rb.linearVelocity + rb.angularVelocity).magnitude;
        
        currentLinearVelocity = rb.linearVelocity;
        currentAngularVelocity = rb.angularVelocity;

        //Debug.Log("The current damage potential is: " + DamagePotential);
    }

    public void Accelerate(float horiz, float Speed, float TurnSpeed)
    {
        // Apply Engine Force
        Vector3 engineForce = transform.forward * Speed;
        rb.AddForce(engineForce, ForceMode.Acceleration);

        // Apply Steering
        float turn = horiz * TurnSpeed * (rb.linearVelocity.magnitude) + turningInfluence;
        rb.angularVelocity = new Vector3(0, turn, 0);
        //rb.rotation = Quaternion.Euler(0, turn, 0);
       // rb.AddTorque(0, turn, 0, ForceMode.Acceleration);
    }

    public void Reverse(float horiz, float Speed, float TurnSpeed)
    {
        Vector3 engineForce = (transform.forward * (Speed * .75f));
        rb.AddForce(-engineForce, ForceMode.Acceleration);

        float turn = horiz * TurnSpeed * (rb.linearVelocity.magnitude) + turningInfluence;
        rb.angularVelocity = new Vector3(0, -turn, 0);
        //rb.AddTorque(0, -turn, 0, ForceMode.Acceleration);
    }
}
