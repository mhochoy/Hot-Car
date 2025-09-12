using UnityEngine;

public class Controls : MonoBehaviour
{
    public bool accelerate;
    public bool deaccelerate;
    public float turn;
    public bool brake;
    public bool Lock;

    void Update()
    {
        if (Lock)
        {
            accelerate = false;
            deaccelerate = false;
            brake = false;
            turn = 0.00f;
            return;
        }
        accelerate = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        deaccelerate = Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow);
        brake = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        turn = Input.GetAxis("Horizontal");
    }
}
