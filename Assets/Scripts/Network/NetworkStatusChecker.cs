using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Android;
using System;

public class NetworkStatusChecker : MonoBehaviour
{
    public TextMeshProUGUI networkStatusText; // UI text to display network status
   

    public TextMeshProUGUI errorMessage; // UI text to display error message
    

     private void RequestAndroidPermissions()
    {

        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.FineLocation))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.FineLocation);
        }

    }
  

    private void Start()
    {
        RequestAndroidPermissions();
        CheckNetworkStatus(); // Check network status on start
    }

    private void CheckNetworkStatus()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            networkStatusText.text = "Connected to"; // Update UI text if connected

            // Check for correct network name
            if (CheckCorrectNetwork())
            {
                networkStatusText.text += " " + "the correct network.";
                networkStatusText.color = Color.green;
            }
            else
            {
                networkStatusText.text += "the wrong network!";
                networkStatusText.color = Color.red;
            }
        }
        else
        {
            networkStatusText.text = "Not connected to network"; // Update UI text if not connected
        }
    }

    private bool CheckCorrectNetwork()
    {

        // Check for the correct network name here
        string correctNetworkName = "Prithvi";
        string currentNetworkName = GetCurrentNetworkName();
        Debug.Log("Current Network Name: " + currentNetworkName);
        Debug.Log("Expected Network Name: " + correctNetworkName);
       

        return currentNetworkName.Equals(correctNetworkName);





    }

 private string GetCurrentNetworkName()
{

    if (Application.platform == RuntimePlatform.Android)
    {
        try
        {
            AndroidJavaObject context = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject wifiManager = context.Call<AndroidJavaObject>("getSystemService", "wifi");
            AndroidJavaObject connectionInfo = wifiManager.Call<AndroidJavaObject>("getConnectionInfo");

            // Get the SSID (network name) from the connection info
            string ssid = connectionInfo.Call<string>("getSSID");

            // SSID might include quotes, remove them for comparison
            return ssid.Replace("\"", "");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error while retrieving SSID: " + ex.Message);
            
            return "Unknown SSID";
        }
    }

    return "Unknown SSID";
}

}
