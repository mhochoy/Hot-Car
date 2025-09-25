using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CarSetupWizard : ScriptableWizard
{
    [Header("Setup")]
    public GameObject CarObject;
    [Header("Properties")]
    public string Name;
    [Header("Physics Properties")]
    public float Mass;
    public float Speed;
    public float SpeedResistance; 
    public float TurnSpeed;
    public float TurnResistance;
    [Header("Game Properties")]
    public bool IsBot;

    [MenuItem("Car/Create")]

    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<CarSetupWizard>("Create Car", "Create");
    }

    private void OnWizardCreate()
    {
        if (IsBot == false)
        {
            MakePlayer();
        }
        else
        {
            MakeBot();
        }
    }

    void MakeBot()
    {
        BotCar botCar = CarObject.GetComponent<BotCar>();
        Rigidbody rb = null;
        GameObject parent = new GameObject(Name + "Parent");

        CarObject.name = Name;
        CarObject.transform.parent = parent.transform;
        CarObject.tag = "Bot";
        CarObject.layer = 6;

        if (botCar == null)
        {
            botCar = CarObject.AddComponent<BotCar>();
            rb = CarObject.GetComponent<Rigidbody>();
        }
        if (rb == null)
        {
            CarObject.AddComponent<Rigidbody>();
        }

        ConfigureCar(botCar, rb);
    }

    void MakePlayer()
    {
        PlayerCar playerCar = CarObject.GetComponent<PlayerCar>();
        Rigidbody rb = null;
        GameObject parent = new GameObject(Name + "Parent");
        GameObject cameras = Instantiate((GameObject)Resources.Load("Cameras"));

        parent.tag = "Player";
        CarObject.name = Name;
        parent.AddComponent<Movement>();
        CarObject.transform.parent = parent.transform;
        CarObject.tag = "Player";
        CarObject.layer = 6;
        cameras.transform.parent = CarObject.transform;
        cameras.transform.position = new Vector3(0, 0, 0);
        cameras.transform.rotation = new Quaternion(-90, 0, 0, 0);

        if (playerCar == null)
        {
            playerCar = CarObject.AddComponent<PlayerCar>();
            rb = playerCar.GetComponent<Rigidbody>();
        }
        if (rb == null)
        {
            CarObject.AddComponent<Rigidbody>();
        }

        ConfigureCar(playerCar, rb);
    }

    void ConfigureCar(Car car, Rigidbody physics)
    {
        if (SpeedResistance == 0.00f)
        {
            SpeedResistance = Speed / Mathf.Abs(((Speed / 2) - 5));
        }
        if (TurnResistance == 0.00f)
        {
            TurnResistance *= 1.00f * 5f;
        }

        car.Speed = Speed;
        car.SpeedResistance = SpeedResistance;
        car.TurnSpeed = TurnSpeed;
        car.TurnResistance = TurnResistance;

        if (physics)
        {
            physics.useGravity = false;
        }
        if (Mass != 0)
        {
            physics.mass = Mass;
        }
        if (physics.collisionDetectionMode != CollisionDetectionMode.ContinuousDynamic)
        {
            physics.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        physics.linearDamping = car.SpeedResistance;
        physics.angularDamping = car.TurnResistance;
    }

    void OnWizardUpdate()
    {
        helpString = "This wizard will help you assemble your car.\nEnsure that your car is facing forward on the x-axis!";
        isValid = false;

        if (CarObject == null || Name == "")
        {
            if (CarObject == null)
            {
                errorString = "You must assign a GameObjet for the car!";
            }
            else if (Name.Length == 0)
            {
                errorString = "You must give your car a name!";
            }

            isValid = false;
        }
        else
        {
            isValid = true;
        }
    }

    void Update()
    {
        
    }
}
