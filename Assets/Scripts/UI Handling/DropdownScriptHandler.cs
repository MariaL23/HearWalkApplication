using UnityEngine;
using TMPro;

public class DropdownScriptController : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public MonoBehaviour[] scriptsToEnableDisable;
    public int[] desiredOptionIndexes; // Define and assign the desired option indexes for each script

    private void Start()
    {
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    private void OnDropdownValueChanged(int index)
    {
        if (index < desiredOptionIndexes.Length)
        {
            int desiredIndex = desiredOptionIndexes[index];
            if (desiredIndex < scriptsToEnableDisable.Length && desiredIndex >= 0)
            {
                MonoBehaviour scriptToInvoke = scriptsToEnableDisable[desiredIndex];
                if (scriptToInvoke != null)
                {
                    scriptToInvoke.Invoke("whenChosen", 0f);
                }
            }
        }
    }
}




