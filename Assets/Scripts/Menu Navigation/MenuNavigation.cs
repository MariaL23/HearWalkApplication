using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuNavigation : MonoBehaviour
{
    public GameObject GameObject;
    //Quit application function
   public void QuitApp()
    {
        Debug.Log ("Closing Application...");
        Application.Quit();

    }
   
   //Load scene function
    public void LoadScene(string sceneName)
    {
        Debug.Log ("Loading Scene 1...");
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

     public void LoadMainMenu(string sceneName)
    {
        Debug.Log ("Loading Scene...");
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void LoadMainPage(string sceneName)
    {
        Debug.Log("Loading Scene...");
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }
    public void LoadSetupPage(string sceneName)
    {
        Debug.Log("Loading Scene...");
        UnityEngine.SceneManagement.SceneManager.LoadScene(3);
    }
    public void LoadPlacementScene(string sceneName)
    {
        Debug.Log("Loading Scene...");
        UnityEngine.SceneManagement.SceneManager.LoadScene(4);
    }

    public void LoadExercisePage(string sceneName)
    {
        Debug.Log("Loading Scene...");
        UnityEngine.SceneManagement.SceneManager.LoadScene(5);
    }
    public void LoadSummary(string sceneName)
    {
        Debug.Log("Loading Scene...");
        UnityEngine.SceneManagement.SceneManager.LoadScene(6);
    }
    public void LoadNewExercise(string sceneName)
    {
        Debug.Log("Loading Scene...");
        UnityEngine.SceneManagement.SceneManager.LoadScene(7);
    }


    public void DismissUI()
    {
        GameObject.SetActive(false);
    }
}
