using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static DataProcessorAHRS;
using UnityEngine.UI;
using System.IO;

public class MainDataProcessor : MonoBehaviour
{
    public List<SensorTextPair> sensorTextPairs;
    private DataProcessorAHRS dataProcessor; // Reference to the DataProcessorAHRS instance

   
    
    public TextMeshProUGUI StartText;

    [System.Serializable]
    public class SensorTextPair // Class to store the sensor name and the corresponding text objects
    {
        public string sensorName;

        public TextMeshProUGUI gyroText;

    }
    private void Start()
    {
       
        // Find the DataProcessorAHRS instance in the scene or use a reference you already have
        dataProcessor = DataProcessorAHRS.Instance; // Replace with your actual instance retrieval method
    }

  
    public void HandleIncomingData(string sensorName, string data)
    {
        if (dataProcessor != null)
        {
            
            
            List<string> sensorData = dataProcessor.GetSynchronizedData(sensorName);

            if (sensorData != null)
            {
                // Process the sensor data as needed
                foreach (string dataEntry in sensorData)
                {
                    // Split the data entry and process individual data points
                    string[] dataPoints = dataEntry.Split(',');
                    float accX = float.Parse(dataPoints[0]);
                    float accY = float.Parse(dataPoints[1]);
                    float accZ = float.Parse(dataPoints[2]);

                    float gyroX = float.Parse(dataPoints[3]);
                    float gyroY = float.Parse(dataPoints[4]);
                    float gyroZ = float.Parse(dataPoints[5]);

                    // Process accX, accY, accZ, gyroX, gyroY, gyroZ as needed

                    float Switchedpitch = dataProcessor.Roll;
                    float Switchedroll = dataProcessor.Pitch;

                    UpdateText(gyroX, Switchedroll,Switchedpitch);
                }
            }
        }
        else
        {
            Debug.LogWarning("DataProcessorAHRS instance not found.");
        }
        void UpdateText(float gyroX,float Swictedroll, float Switchedpitch)
        {
            foreach (var pair in sensorTextPairs)
            {
                if (pair.sensorName == sensorName)
                {
                    pair.gyroText.text = 
                    "\n" +
                    " Gyro X:  " + gyroX.ToString("F2").PadRight(15) + 
                    "\n Pitch  " + Switchedpitch.ToString("F2").PadRight(15) + 
                    " Roll  " + Swictedroll.ToString("F2").PadRight(15);
                }
            }
        }
    }
}
