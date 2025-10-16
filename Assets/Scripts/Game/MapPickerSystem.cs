using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using System.Collections.Generic;
using System;

public class MapPickerSystem : MonoBehaviour
{
    public GameObject Car;
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
        HandleSelectedMap();
        environment = Instantiate(selectedMapEnvironment);
        spawnedEnvironments.Add(environment);
    }

    private void Update()
    {
        HandleSelectedMap();
        HandleRacersSlider();
        SetEnvironment();
        
        if (spawnedEnvironments.Count > 0)
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

        if (environment)
        {
            if (Car.transform.parent != environment.transform)
            {
                Car.transform.parent = environment.transform;
            }
            else
            {
                WaitingPoint waitingPoint = environment.GetComponentInChildren<WaitingPoint>();
                Car.transform.position = waitingPoint.transform.position;
                Car.transform.rotation = waitingPoint.transform.rotation;
            }
        }
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

    public void Race()
    {
        racePreferences.DealDamage = damageToggle.isOn;
        racePreferences.TotalRacers = (int)totalRacersSlider.value;
        StartCoroutine(InternalRace());
    }

    IEnumerator<object> InternalRace()
    {
        //SceneManager.LoadScene(mapPicker.options[mapPicker.value].text);
        SceneManager.LoadScene(mapPicker.options[mapPicker.value].text);
        yield return null;
    }
}
