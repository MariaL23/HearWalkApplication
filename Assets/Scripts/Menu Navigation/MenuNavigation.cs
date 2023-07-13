using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNavigation : MonoBehaviour
{
   public void QuitApp()
    {
        Debug.Log ("Closing Application...");
        Application.Quit();

    }

    public void LoadScene(string sceneName)
    {
        Debug.Log ("Loading Scene...");
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
