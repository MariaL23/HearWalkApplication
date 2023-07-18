using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;
public class SensorDataProcessor : MonoBehaviour
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
}
