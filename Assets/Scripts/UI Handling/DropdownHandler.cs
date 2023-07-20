using UnityEngine;
using TMPro;

public class DropdownHandler : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public MonoBehaviour[] scriptsTochoose;
    public int[] desiredOptionIndexes; // Define and assign the desired option indexes for each script



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
                    DataProcessorAHRS.Instance.SetBodyPartName(selectedBodyPartName); // Pass the Dropdown GameObject name to the DataProcessorAHRS script
                    
                }
            }
        }
    }

    
}




