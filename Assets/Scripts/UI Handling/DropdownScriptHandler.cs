using UnityEngine;
using TMPro;

public class DropdownScriptController : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public GameObject[] gameObjectsToEnableDisable;
    public int[] desiredOptionIndexes; // Define and assign the desired option indexes for each GameObject

    private void Start()
    {
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    private void OnDropdownValueChanged(int index)
    {
        for (int i = 0; i < gameObjectsToEnableDisable.Length; i++)
        {
            gameObjectsToEnableDisable[i].SetActive(i == desiredOptionIndexes[index]);
        }
    }
}
