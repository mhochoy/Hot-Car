using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapPickerSystem : MonoBehaviour
{
    public TMP_Dropdown mapPicker;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Race()
    {
        SceneManager.LoadScene(mapPicker.options[mapPicker.value].text);
    }
}
