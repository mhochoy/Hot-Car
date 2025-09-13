using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Health))]
public class BotCar : MonoBehaviour
{
    public Health health;
    public NavMeshAgent agent;
    public Transform target;
    public Movement movement;
    public Waypoint FirstWaypoint;
    public float Damage;
    public float Speed;
    public float TurnSpeed;
    public int CurrentLap = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        agent.destination = FirstWaypoint.transform.position;
        health = GetComponent<Health>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Damage = movement.DamagePotential;
        var lookPos = target.position - agent.destination;
        lookPos.x = 0.00f;
        lookPos.z = 0.00f;
        var rotation = Quaternion.LookRotation(lookPos);
        agent.updateRotation = true;
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);
        //movement.Accelerate(Vector3.Angle(rotation.eulerAngles, transform.rotation.eulerAngles), Speed, TurnSpeed);
    }

    public void NextLap()
    {
        CurrentLap++;
    }

    public void Go(Vector3 point)
    {

        agent.SetDestination(point);
    }

    void Death()
    {
        GameFX.instance.SpawnExplosion(transform.position);
        GameFX.instance.SpawnSmokeStreamEffect(transform.position);
        gameObject.SetActive(false);
    }
}
