using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Health))]
public class BotCar : Car
{
    public NavMeshAgent agent;
    public Transform target;
    public Waypoint FirstWaypoint;
    public bool Stop;

    protected override void Awake()
    {
        base.Awake();
        agent.destination = FirstWaypoint.transform.position;
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        base.currentVelocity = agent.velocity;
        base.FixedUpdate();
        agent.isStopped = Stop;
        Damage = Mathf.Abs(agent.velocity.magnitude) / 1.25f;
        var lookPos = target.position - agent.destination;
        lookPos.x = 0.00f;
        lookPos.z = 0.00f;
        var rotation = Quaternion.LookRotation(lookPos);
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
}
