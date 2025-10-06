using log4net.Util;
using Unity.Cinemachine;
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
    public float Health;
    public float Mass;
    public float Speed;
    public float TurnSpeed;
    [Header("Game Properties")]
    public bool IsBot;
    float x;
    float y;
    float z;

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

        CarObject.name = Name;
        CarObject.tag = "Bot";
        CarObject.layer = 6;
        SetCollider();

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
        //GameObject cameras = Instantiate((GameObject)Resources.Load("Cameras"));
        GameObject cinemachineCam = Instantiate((GameObject)Resources.Load("CinemachineCamera"));
        CinemachineCamera cinemachineCamProperties = cinemachineCam.GetComponent<CinemachineCamera>();
        CameraTarget target = new CameraTarget();

        CarObject.name = Name;
        CarObject.AddComponent<Movement>();
        CarObject.AddComponent<Controls>();
        CarObject.tag = "Player";
        CarObject.layer = 6;
        SetCollider();

        /*
        cameras.transform.parent = CarObject.transform;
        cameras.transform.localPosition = new Vector3(0, 0, 0);
        cameras.transform.localRotation = Quaternion.Euler(-90f, 0, -90f);
        cameras.transform.localScale = new Vector3(1f, 1f, 1f);
        */

        cinemachineCam.transform.parent = CarObject.transform;
        target.TrackingTarget = CarObject.transform;
        cinemachineCamProperties.Target = target;

        // Link essential components (need to be assigned before the game is run
        if (playerCar == null)
        {
            playerCar = CarObject.AddComponent<PlayerCar>();
            // Best time to assign health
            Health health =  playerCar.AddComponent<Health>();
            playerCar.health = health;
            // ---------
            rb = playerCar.GetComponent<Rigidbody>();
        }
        if (rb == null)
        {
            rb = CarObject.AddComponent<Rigidbody>();
            playerCar.physics = rb;
        }

        ConfigureCar(playerCar, rb);
    }

    void ConfigureCar(Car car, Rigidbody physics)
    {
        if (TurnSpeed > 0)
        {
            TurnSpeed = TurnSpeed * .10f;
        }
        car.health.value = Health;
        car.Speed = Speed;
        car.TurnSpeed = TurnSpeed;
        car.carSounds = (CarSounds)Instantiate(Resources.Load("CarSounds"));

        if (physics)
        {
            physics.useGravity = true;
        }
        if (Mass != 0)
        {
            physics.mass = Mass;
        }
        if (physics.collisionDetectionMode != CollisionDetectionMode.ContinuousDynamic)
        {
            physics.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
    }

    void SetCollider()
    {
        foreach (GameObject t in CarObject.transform)
        {
            MeshRenderer meshRend = t.GetComponent<MeshRenderer>();

            if (meshRend)
            {
                float maxX = meshRend.localBounds.max.x;
                float maxY = meshRend.localBounds.max.y;
                float maxZ = meshRend.localBounds.max.z;

                if (maxX > x)
                {
                    x = maxX;
                }
                if (maxY > y)
                {
                    y = maxY;
                }
                if (maxZ > z)
                {
                    z = maxZ;
                }
            }
        }

        if (CarObject.GetComponent<Collider>() == null)
        {
            BoxCollider boxCollider = CarObject.AddComponent<BoxCollider>();

            boxCollider.size = new Vector3(x, y, z);
        }
    }

    void OnWizardUpdate()
    {
        helpString = "This wizard will help you assemble your car.\nEnsure that your car is facing forward on the z-axis!";

        if (CarObject == null || Name == "")
        {
            if (CarObject == null)
            {
                errorString = "You must assign a GameObject for the car!";
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
