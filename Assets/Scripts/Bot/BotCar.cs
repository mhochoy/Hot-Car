using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class BotCar : Car
{
    [Header("Bot Properties")]
    [Tooltip("The first waypoint that the bot car will have to follow. Necessary for bot to stay on track!")]
    public Waypoint FirstWaypoint;
    [Tooltip("How quickly the bot car gets up to speed (n * 10).")]
    public float Acceleration;
    [Tooltip("The range that the bot car will avoid danger.")]
    public float ObstacleAvoidanceRadius;
    [Tooltip("Is set from the GameSystem script. Used during countdowns.")]
    public bool Stop;

    [Header("Bot Components")]
    public NavMeshAgent agent;


    Transform target;


    protected override void Awake()
    {
        base.Awake();
        agent.speed = base.Speed;
        agent.acceleration = Acceleration * 10;
        agent.angularSpeed = base.TurnSpeed * 100;
        agent.radius = ObstacleAvoidanceRadius;
        agent.stoppingDistance = 0.81f;
    }

    private void Start()
    {
        agent.destination = FirstWaypoint.transform.position;
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        base.currentVelocity = agent.velocity;
        base.FixedUpdate();
        agent.isStopped = Stop;
        Damage = Mathf.Abs(agent.velocity.magnitude) / 1.25f;
        agent.updateRotation = true;
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);
        //movement.Accelerate(Vector3.Angle(rotation.eulerAngles, transform.rotation.eulerAngles), Speed, TurnSpeed);
    }

    public void Go(Vector3 point)
    {

        agent.SetDestination(point);
    }

    protected override void Death()
    {
        base.Death();
        gameObject.SetActive(false);
    }

    protected override void SetNextWaypoint(Waypoint waypoint)
    {
        base.SetNextWaypoint(waypoint);
        Go(waypoint.transform.position);
    }
}
