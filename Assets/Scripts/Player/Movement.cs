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
        // If this script is active on a GameObject, then the GameObject will be designated as a PlayerCar
        // and must be set up for the player to use out of the box if there is no setup detected.
        if (transform.childCount <= 0)
        {
            GameObject car = new GameObject();
            car.transform.parent = transform;
            car.AddComponent<PlayerCar>();
            car.AddComponent<CarSoundbank>();
        }

        rb = GetComponentInChildren<Rigidbody>();
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
        Vector2 engineForce = transform.up * Speed;
        rb.AddRelativeForce(engineForce, ForceMode.Acceleration);

        // Apply Steering
        float turn = horiz * TurnSpeed * (rb.linearVelocity.magnitude / 10f) + turningInfluence;
        rb.angularVelocity = new Vector3(0, turn, 0);
    }

    public void Reverse(float horiz, float Speed, float TurnSpeed)
    {
        Vector2 engineForce = (-transform.up * Speed) / 2;
        rb.AddRelativeForce(engineForce, ForceMode.Acceleration);

        float turn = horiz * TurnSpeed * (rb.linearVelocity.magnitude / 10f) + turningInfluence;
        rb.angularVelocity = new Vector3(0, -turn, 0);
    }
}
