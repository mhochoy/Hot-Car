using UnityEngine;
using UnityEngine.AI;

public class BotCar : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform target;
    public Movement movement;
    public float Speed;
    public float TurnSpeed;
    public int CurrentLap = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var lookPos = target.position - transform.position;
        lookPos.x = 0.00f;
        lookPos.z = 0.00f;
        var rotation = Quaternion.LookRotation(lookPos);
        //transform.localRotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * TurnSpeed);
        movement.Accelerate(Vector3.Angle(rotation.eulerAngles, transform.rotation.eulerAngles), Speed, TurnSpeed);
    }

    public void NextLap()
    {
        CurrentLap++;
    }
}
