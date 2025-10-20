using UnityEngine;
using UnityEngine.InputSystem;

public class Controls : MonoBehaviour
{
    public InputSystemActions inputActions;
    public bool accelerate { get; private set; }
    public bool deaccelerate { get; private set; }
    public float turn { get; private set; }
    public bool brake { get; private set; }
    public bool switchCam { get; private set; }
    public bool pause;
    public bool Paused = false;
    public bool Lock;

    private void Awake()
    {
        inputActions = new InputSystemActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

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

        pause = inputActions.Player.Pause.WasPressedThisFrame();
        accelerate = inputActions.Player.Accelerate.ReadValue<float>() > 0 ? true : false;
        //deaccelerate = Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow);
        brake = inputActions.Player.Reverse.ReadValue<float>() > 0 ? true : false;
        switchCam = inputActions.Player.Switch.ReadValue<float>() > 0 ? true : false;
        turn = -inputActions.Player.Turn.ReadValue<float>();
        if (pause)
        {
            Paused = !Paused;
        }
    }
}
