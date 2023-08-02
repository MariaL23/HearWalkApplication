using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceManager : MonoBehaviour
{
    public static ChoiceManager Instance { get; private set; }
    public List<int> chosenItemIndices = new List<int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

     private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll(); // Clear all PlayerPrefs data when quitting the application
    }
   
}
