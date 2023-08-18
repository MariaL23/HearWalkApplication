using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class DataRecorder : MonoBehaviour
{/*
    private Dictionary<string, StreamWriter> RecordCSVWriters = new Dictionary<string, StreamWriter>(); // Dictionary to store the CSV writers for each sensor
     private Dictionary<string, bool> fileCreatedFlags = new Dictionary<string, bool>(); // Dictionary to store the file creation flags for each sensor
    private DataProcessorAHRS dataProcessor;

    private void Start()
    {
        // Find and assign the DataProcessorAHRS instance
        dataProcessor = FindObjectOfType<DataProcessorAHRS>();
        RequestAndroidPermissions();
    }

    private void Update()
    {
        if (dataProcessor != null)
        {
            // Access the variables from DataProcessorAHRS
            string sensorName = dataProcessor.sensorName;
            string bodyPart = dataProcessor.bodyPartName;
            // You can access the 'data' variable within the ProcessData method when it's called.
        }
    }

    private void RecordCSV(string sensorName)
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
            RecordCSVWriters[sensorName] = writer;
            fileCreatedFlags[sensorName] = true;
        }
    }

    public void WriteToRecord(string sensorName, string data)
    {
         if (!RecordCSVWriters.ContainsKey(sensorName))
    {
        Debug.LogError("CSV writer not found for sensor: " + sensorName);
        return;
    }

    StreamWriter writer = RecordCSVWriters[sensorName];
    string timestamp = System.DateTime.Now.ToString("mm.ss.fff"); // Seconds and milliseconds
    string line = $"{timestamp},{data}";
    writer.WriteLine(line);
    }

       private void RequestAndroidPermissions()
    {

        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.ExternalStorageWrite))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.ExternalStorageWrite);
        }

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
*/
}
