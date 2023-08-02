using UnityEngine;
using TMPro;

public class DropdownHandler : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public MonoBehaviour[] scriptsTochoose;
    public int[] desiredOptionIndexes;

    private void OnEnable()
    {
        string selectedBodyPartName = dropdown.gameObject.name;
        int savedIndex = PlayerPrefs.GetInt(selectedBodyPartName, 0);
        dropdown.value = savedIndex;
        
        OnDropdownValueChanged(savedIndex); // Call whenChosen when the scene is reloaded
    }

    private void Start()
    {
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    public void OnDropdownValueChanged(int index)
    {
        if (index < desiredOptionIndexes.Length)
        {
            int desiredIndex = desiredOptionIndexes[index];
            if (desiredIndex < scriptsTochoose.Length && desiredIndex >= 0)
            {
                MonoBehaviour scriptToInvoke = scriptsTochoose[desiredIndex];
                if (scriptToInvoke != null)
                {
                    scriptToInvoke.Invoke("whenChosen", 0f);
                    string selectedBodyPartName = dropdown.gameObject.name;
                    DataProcessorAHRS.Instance.SetBodyPartName(selectedBodyPartName);

                    PlayerPrefs.SetInt(selectedBodyPartName, index);
                    PlayerPrefs.Save(); // Save the selected index
                }
            }
        }
    }
}
