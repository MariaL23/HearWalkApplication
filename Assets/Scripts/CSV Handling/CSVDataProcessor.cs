using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using System.IO;

public class CSVDataProcessor : MonoBehaviour
{
    public List<SensorTextPair> sensorTextPairs;
    private Dictionary<string, List<string>> synchronizedDataDict = new Dictionary<string, List<string>>();
    private Dictionary<string, StreamWriter> csvWriters = new Dictionary<string, StreamWriter>();
    private Dictionary<string, bool> fileCreatedFlags = new Dictionary<string, bool>();
    
    public static CSVDataProcessor Instance { get; private set; }

    private string bodyPartName; // Variable to store the body part name received from the DropdownController script



    [System.Serializable]
    public class SensorTextPair
    {
        public string sensorName;
        
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

       
    }

 
    private void RequestAndroidPermissions()
    {

        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.ExternalStorageWrite))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.ExternalStorageWrite);
        }

    }

    private void Start()
    {
        RequestAndroidPermissions();
        
    }

      public void SetBodyPartName(string name)
    {
        bodyPartName = name; // Set the body part name received from the DropdownController script
    }

    public void ProcessData(string sensorName, string data)
    {
        if (!synchronizedDataDict.ContainsKey(sensorName))
        {
            synchronizedDataDict[sensorName] = new List<string>();
        }

        // Filter out the data entries older than 0.03 seconds
        float currentTime = Time.time;
        float timeThreshold = currentTime - 0.03f;

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

        // Create the file for the sensor if it hasn't been created yet
        CreateCSV(sensorName);
    }

    private void CreateCSV(string sensorName)
    {
        if (!fileCreatedFlags.ContainsKey(sensorName) || !fileCreatedFlags[sensorName])
        {
            string folderPath = Path.Combine(GetAndroidExternalStoragePath(), "Documents", sensorName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string timestamp = System.DateTime.Now.ToString("dd.MM.yyyy_ss");
            string fileName = $"{bodyPartName}_{timestamp}.csv";
            string filePath = Path.Combine(folderPath, fileName);

            StreamWriter writer = new StreamWriter(filePath, true);
            writer.WriteLine("Timestamp,accX,accY,accZ,gyroX,gyroY,gyroZ,Battery");
            csvWriters[sensorName] = writer;
            fileCreatedFlags[sensorName] = true;
        }
    }

    

    private void Update()
    {
        foreach (var pair in sensorTextPairs)
        {
            string sensorName = pair.sensorName;
            List<string> sensorData = GetSynchronizedData(sensorName);

            if (sensorData != null)
            {
                foreach (string data in sensorData)
                {
                    WriteToCSV(sensorName, data);
                }
            }
        }
    }

    private void WriteToCSV(string sensorName, string data)
    {
         if (!csvWriters.ContainsKey(sensorName))
    {
        Debug.LogError("CSV writer not found for sensor: " + sensorName);
        return;
    }

    StreamWriter writer = csvWriters[sensorName];
     string timestamp = System.DateTime.Now.ToString("ss.fff"); // Seconds and milliseconds
    string line = $"{timestamp},{data}";
    writer.WriteLine(line);
    }

    private List<string> GetSynchronizedData(string sensorName)
    {
        if (synchronizedDataDict.ContainsKey(sensorName))
        {
            return synchronizedDataDict[sensorName];
        }

        return null;
    }


       private string GetAndroidExternalStoragePath()
    {
        string externalStoragePath = "";

        using (AndroidJavaClass androidEnvironment = new AndroidJavaClass("android.os.Environment"))
        {
            using (AndroidJavaObject externalStorageDirectory = androidEnvironment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory"))
            {
                externalStoragePath = externalStorageDirectory.Call<string>("getAbsolutePath");
            }
        }

        return externalStoragePath;
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



   

    private void OnApplicationQuit()
    {
        foreach (var writer in csvWriters.Values)
        {
            writer.Close();
        }
    }
}
