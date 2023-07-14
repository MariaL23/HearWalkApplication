using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuNavigation : MonoBehaviour
{
    //Quit application function
   public void QuitApp()
    {
        Debug.Log ("Closing Application...");
        Application.Quit();

    }
   
   //Load scene function
    public void LoadScene(string sceneName)
    {
        Debug.Log ("Loading Scene...");
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
