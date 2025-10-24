using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using System.Collections.Generic;
using System;

public class MapPickerSystem : MonoBehaviour
{
    // Car Details
    public Transform CarParent;
    public PlayerRacers availableRacers;
    GameObject selectedCar;
    Car selectedCarDetails;
    public int SelectedCarIndex = 0;
    GameObject car;
    List<GameObject> spawnedCars = new List<GameObject>();

    // Car UI Elements
    public TMP_Text CarName;
    public TMP_Text CarSpeed;
    public TMP_Text CarTurningSpeed;
    public TMP_Text CarMass;
    public TMP_Text CarHealth;

    public TMP_Dropdown mapPicker;
    public Toggle damageToggle;
    public TMP_Text totalRacersText;
    public Slider totalRacersSlider;
    public List<Scene> scenes = new List<Scene>();
    public RacePreferences racePreferences;

    public MapSettings selectedMapSettings = null;
    string selectedMap;
    [SerializeField] GameObject selectedMapEnvironment = null;

    [SerializeField] GameObject environment = null;
    List<GameObject> spawnedEnvironments = new List<GameObject>();
    

    void Awake()
    {
        // Map
        HandleSelectedMap();
        environment = Instantiate(selectedMapEnvironment);
        spawnedEnvironments.Add(environment);
        // Car
        HandleSelectedCar();
        car = Instantiate(selectedCar, CarParent);
        spawnedCars.Add(car);
    }

    private void Update()
    {
        HandleSelectedMap();
        HandleRacersSlider();
        SetEnvironment();
        
        if (spawnedEnvironments.Count > 0)
        {
            HandleActiveEnvironment();
        }

        if (environment)
        {
            HandleSelectedCar();
            HandleActiveCar();
            SetCar();

            selectedCarDetails = selectedCar.GetComponent<Car>();

            HandleCarUI();
            SyncCarTransform();
        }

        car.GetComponent<Car>().physics.isKinematic = true;
    }

    void HandleActiveCar()
    {
        foreach (GameObject spawn in spawnedCars)
        {
            if (spawn.name != selectedCar.name + "(Clone)") // Makes sure unselected cars are not active
            {
                spawn.SetActive(false);
            }
        }
    }

    void HandleCarUI()
    {
        CarName.text = $"Name: {selectedCarDetails.name}";
        CarSpeed.text = $"Speed: {selectedCarDetails.Speed}";
        CarTurningSpeed.text = $"Turn Speed: {selectedCarDetails.TurnSpeed}";
        CarMass.text = $"Weight: {selectedCarDetails.physics.mass}";
        CarHealth.text = $"Health: {selectedCarDetails.health.value}";
    }

    void HandleActiveEnvironment()
    {
        foreach (GameObject env in spawnedEnvironments)
        {
            if (env.name != selectedMapEnvironment.name + "(Clone)") // Makes sure unselected map environments are not active
            {
                env.SetActive(false);
            }
            else
            {
                if (env.activeSelf == false)
                {
                    env.SetActive(true);
                }
                environment = env;
            }
        }
    }

    void HandleSelectedCar()
    {
        selectedCar = availableRacers.PlayerCars[SelectedCarIndex];
    }

    void HandleSelectedMap()
    {
        selectedMap = mapPicker.options[mapPicker.value].text;
        selectedMapSettings = (MapSettings)Resources.Load($"Maps/{selectedMap}/{selectedMap}Settings");
        selectedMapEnvironment = (GameObject)Resources.Load($"Maps/{selectedMap}/{selectedMap}Environment");
    }

    void HandleRacersSlider()
    {
        if (selectedMapSettings)
        {
            totalRacersSlider.maxValue = selectedMapSettings.TotalRacers;
            totalRacersText.text = "Total Racers: " + totalRacersSlider.value;
        }
    }

    public void PreviousPlayerRacer()
    {
        if (SelectedCarIndex - 1 < 0)
        {
            SelectedCarIndex = 0;
        }
        else
        {
            SelectedCarIndex--;
        }
    }

    public void NextPlayerRacer()
    {
        if (SelectedCarIndex + 1 > availableRacers.PlayerCars.Count - 1)
        {
            SelectedCarIndex = availableRacers.PlayerCars.Count - 1;
        }
        else
        {
            SelectedCarIndex++;
        }
    }
    void SyncCarTransform()
    {
        if (car == null)
        {
            return;
        }

        if (CarParent.transform.parent != environment.transform)
        {
            CarParent.transform.parent = environment.transform;
        }
        else
        {
            WaitingPoint waitingPoint = environment.GetComponentInChildren<WaitingPoint>();
            CarParent.transform.position = waitingPoint.transform.position;
            CarParent.transform.rotation = waitingPoint.transform.rotation;
        }
    }

    public void SetEnvironment()
    {
        if (!spawnedEnvironments.Exists(env => env.name == selectedMapEnvironment.name + "(Clone)")) // if the environment hasn't already been spawned...
        {
            environment = Instantiate(selectedMapEnvironment);
            spawnedEnvironments.Add(environment);
        }
        else // ...otherwise make sure it is enabled
        {
            GameObject background = spawnedEnvironments.Find((env) => env.name == selectedMapEnvironment.name + "(Clone)");
            background.SetActive(true);
        }
    }

    public void SetCar()
    {
        if (spawnedCars.Exists(c => c.name == selectedCar.name + "(Clone)") == false)
        {
            Debug.Log("Spawning Car!");
            car = Instantiate(selectedCar, CarParent);
            car.transform.rotation = Quaternion.Euler(180, 192.89f, 180);
            spawnedCars.Add(car);
        }
        else // ...otherwise make sure it is enabled
        {
            car = spawnedCars.Find((c) => c.name == selectedCar.name + "(Clone)");
            car.SetActive(true);
        }
    }

    public void Race()
    {
        racePreferences.DealDamage = damageToggle.isOn;
        racePreferences.TotalRacers = (int)totalRacersSlider.value;
        racePreferences.PlayerCar = selectedCar;
        StartCoroutine(InternalRace());
    }

    IEnumerator<object> InternalRace()
    {
        //SceneManager.LoadScene(mapPicker.options[mapPicker.value].text);
        SceneManager.LoadScene(mapPicker.options[mapPicker.value].text);
        yield return null;
    }
}
