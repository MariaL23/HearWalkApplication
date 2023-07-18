using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;
using TMPro;
using UnityEngine.UI;

public class MultiSensorHandler : MonoBehaviour
{
    public TextMeshProUGUI allSensorText;
    public void ProcessSensorData(List<string> sensorDataList)
    {
        // Process the synchronized sensor data here
        //access the data from all the sensors via the 'sensorDataList' parameter

        // Example: Print the sensor data for each message
        foreach (string sensorData in sensorDataList)
        {
            allSensorText.text = sensorData;
            Debug.Log(sensorData);
        }
    }
}
