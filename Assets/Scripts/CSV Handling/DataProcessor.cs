using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Threading.Tasks;
using System.IO;

public class DataProcessor : MonoBehaviour
{
    public List<SensorTextPair> sensorTextPairs;

    private Dictionary<string, List<string>> synchronizedDataDict = new Dictionary<string, List<string>>();

    [System.Serializable]
    public class SensorTextPair
    {
        public string sensorName;
        public TextMeshProUGUI textComponent;
    }

    

   

   public void ProcessData(string sensorName, string data)
{
    if (!synchronizedDataDict.ContainsKey(sensorName))
    {
        synchronizedDataDict[sensorName] = new List<string>();
    }

    // Filter out the data entries older than 0.01 seconds
    float currentTime = Time.time;
    float timeThreshold = currentTime - 0.01f;

    synchronizedDataDict[sensorName].RemoveAll(entry =>
    {
        string[] entryData = entry.Split(',');
        if (entryData.Length > 0)
        {
            float entryTimestamp = float.Parse(entryData[0]);
            return entryTimestamp < timeThreshold;
        }
        return false;
    });

    synchronizedDataDict[sensorName].Add(data);

    StartCoroutine(UpdateTextCoroutine());

    // Call the WriteDataToCSVAsync method from CSVWriter with the sensor name and data list
    
}



    public void ClearSynchronizedData()
    {
        synchronizedDataDict.Clear();
    }

    private IEnumerator UpdateTextCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.01f);
            UpdateText();
        }
    }

    private void UpdateText()
    {
        foreach (var pair in sensorTextPairs)
        {
            string sensorName = pair.sensorName;
            List<string> sensorData = GetSynchronizedData(sensorName);

            if (sensorData != null)
            {
                string sensorDataString = string.Join("\n", sensorData);
                pair.textComponent.text = sensorDataString;
            }
            else
            {
                pair.textComponent.text = "";
            }
        }
    }

    private List<string> GetSynchronizedData(string sensorName)
    {
        if (synchronizedDataDict.ContainsKey(sensorName))
        {
            return synchronizedDataDict[sensorName];
        }

        return null;
    }

    private void ProcessSensorData1(List<string> data)
    {
        // Process data here for sensor 1
    }

    private void ProcessSensorData2(List<string> data)
    {
        // Process data here for sensor 2
    }

    private void ProcessSensorData3(List<string> data)
    {
        // Process data here for sensor 3
    }

    private void ProcessSensorData4(List<string> data)
    {
        // Process data here for sensor 4
    }

    
}
