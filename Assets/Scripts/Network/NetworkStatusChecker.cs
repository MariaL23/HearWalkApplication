using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NetworkStatusChecker : MonoBehaviour
{
    
    public TextMeshProUGUI networkStatusText; // UI text to display network status

    private void Start()
    {
        CheckNetworkStatus(); // Check network status on start
    }

    private void CheckNetworkStatus()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            networkStatusText.text = "Connected to network"; // Update UI text if connected
        }
        else
        {
            networkStatusText.text = "Not connected to network"; // Update UI text if not connected
        }
    }
}
