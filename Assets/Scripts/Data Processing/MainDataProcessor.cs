using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainDataProcessor : MonoBehaviour
{
    private DataProcessorAHRS dataProcessor; // Reference to the DataProcessorAHRS instance

    private void Start()
    {
        // Find the DataProcessorAHRS instance in the scene or use a reference you already have
        dataProcessor = DataProcessorAHRS.Instance; // Replace with your actual instance retrieval method
    }

    private void Update()
    {
        if (dataProcessor != null)
        {
            // Assuming you have a list of sensor names
            List<string> sensorNames = new List<string> { "Sensor1", "Sensor2", "Sensor3", "Sensor4" };

            float pitch = dataProcessor.Pitch;
            float roll = dataProcessor.Roll;

            // Now you can use the pitch and roll values in this script
            // For example, you can print them to the console
            Debug.Log("Pitch: " + pitch + ", Roll: " + roll);
            foreach (string sensorName in sensorNames)
            {
                List<string> sensorData = dataProcessor.GetSynchronizedData(sensorName);

                if (sensorData != null)
                {
                    // Process the sensor data as needed
                    foreach (string dataEntry in sensorData)
                    {
                        // Split the data entry and process individual data points
                        string[] dataPoints = dataEntry.Split(',');
                        // Process dataPoints here...
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("DataProcessorAHRS instance not found.");
        }


    }
}
