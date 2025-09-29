using UnityEngine;

public class Controls : MonoBehaviour
{
    public bool accelerate { get; private set; }
    public bool deaccelerate { get; private set; }
    public float turn { get; private set; }
    public bool brake { get; private set; }
    public bool switchCam { get; private set; }
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
        switchCam = Input.GetKeyDown(KeyCode.E);
        turn = Input.GetAxis("Horizontal");
    }
}
