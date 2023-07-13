using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NetworkStatusChecker : MonoBehaviour
{

    public TextMeshProUGUI networkStatusText;

    private void Start()
    {
        CheckNetworkStatus();
    }

    private void CheckNetworkStatus()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            networkStatusText.text = "Connected to network";
        }
        else
        {
            networkStatusText.text = "Not connected to network";
        }
    }
}
