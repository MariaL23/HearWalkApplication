using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DismissButtonHandler : MonoBehaviour
{
    public GameObject DisconnectUI; // Reference to your DisconnectUI GameObject

    public TextMeshProUGUI DismissButtonClickText; // Reference to the TextMeshProUGUI component of the button
    

    public void Start()
    {
        // Assuming you have added an OnClick event in the Inspector for the button
        Button dismissButton = GetComponent<Button>();
        dismissButton.onClick.AddListener(DismissButtonClick);
    }

    public void DismissButtonClick()
    {
        
        DisconnectUI.SetActive(false); // Deactivate the DisconnectUI
        DismissButtonClickText.text = "Dismiss"; // Change the text of the button back to "Dismiss"
    }
}

