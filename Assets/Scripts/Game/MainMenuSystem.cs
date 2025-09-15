using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuSystem : MonoBehaviour
{
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {

    }

    IEnumerator<WaitForSeconds> WaitThenPlay(float time)
    {
        yield return new WaitForSeconds(time);
        LoadChosenScene();
    }

    public void Play()
    {
        animator.Play("Use");
        StartCoroutine(WaitThenPlay(2f));
    }

    void LoadChosenScene()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
