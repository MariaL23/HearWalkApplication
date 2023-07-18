using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;
using UnityEngine.Android;
using UnityEngine.Networking;
using System.IO;

public class SnapshotSensor1: MonoBehaviour
{    
    [Header("OSC Settings")]  

    public OSCReceiver receiver;     // Define the OSC receiver and the OSC address
    public string Address = "/S"; // Define the OSC address
    
    [Header("CSV Settings")]  // Define the folder name and the name of the CSV file
    public string folderName = "Documents/Sensor/Snapshot"  ; // Define the folder name
    public string nameOfFile = "SnapshotSensor"; // Define the name of the CSV file

    // Define the variables to save the messages to a CSV file
    private List<string> messages = new List<string>(); // Initialize the messages list
    private string csvFilePath; // Define the path of the CSV file
    private string csvSeparator = ","; // Define the separator of the CSV file
    private string[] csvHeaders = { "Timestamp", "accX", "accY", "accZ", "gyroX", "gyroY", "gyroZ", "Battery"};
    private float lastWriteTime; // Define the last write time
    
    



     private void Start()
    {
        // Bind the OSC receiver to the address
        receiver.Bind(Address, MessageReceived);
  
    }
     public void whenChosen(){
        // Create the CSV file
        CreateCSVFile();

        // Initialize last write time
        lastWriteTime = Time.realtimeSinceStartup;

        // Start the coroutine to write to the CSV file every frame
        StartCoroutine(WriteToCSVFileCoroutine());

        Debug.Log("Chosen" + Address);
    }

    private void OnDestroy()
    {
        // Stop the coroutine when the object is destroyed
        StopCoroutine(WriteToCSVFileCoroutine());
    }

    // Receive the OSC message
    public void MessageReceived(OSCMessage message)
    {     
        // Extract timestamp and value from the OSC message
        string timestamp = Time.realtimeSinceStartup.ToString();

        List<float> floatValues = new List<float>();

        // Iterate through all values in the OSC message
        foreach (var oscValue in message.Values)
        {
            // Check if the value is a float
            if (oscValue is OSCValue value && value.Value is float floatValue)
            {
                // Add the float value to the list
                floatValues.Add(floatValue);
            }
        }

        // Convert the float values to strings
        List<string> stringValues = floatValues.ConvertAll(value => value.ToString());

        // Combine timestamp with all float values
        string formattedMessage = $"{string.Join(",", stringValues)}";

        // Add the formatted message to the list
        messages.Add(formattedMessage);
    }

    // Create the CSV file
    private void CreateCSVFile()
    {
        csvFilePath = GetCSVFilePath();


        if (!File.Exists(csvFilePath)) // If the file doesn't exist, create it
        {
            using (StreamWriter writer = new StreamWriter(csvFilePath))
            {
                writer.WriteLine(string.Join(csvSeparator, csvHeaders));
            }
        }
    }
 
    private string GetCSVFilePath()
    {   
        // Name the CSV File SnapShots_YYYYMMDDHHMM.csv 
        string timeStamp = System.DateTime.Now.ToString("yyyyMMddHHmm");
        string fileName = nameOfFile + "-" + timeStamp + ".csv";
        string folderPath = Path.Combine(GetExternalStoragePath(), folderName);

        // Create the folder if it doesn't exist
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        return Path.Combine(folderPath, fileName); // Return the path of the CSV file
    }

    // Ensure that the file is saved in the Documents folder on Android
    private string GetExternalStoragePath()
    {
        string externalStoragePath = "";

        // Check if the application is running on Android
        if (Application.platform == RuntimePlatform.Android)
        {
            using (AndroidJavaClass androidEnvironment = new AndroidJavaClass("android.os.Environment"))
            {
                using (AndroidJavaObject externalStorageDirectory = androidEnvironment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory"))
                {
                    externalStoragePath = externalStorageDirectory.Call<string>("getAbsolutePath");
                }
            }
        }

        return externalStoragePath;
    }
   
    // Define the coroutine to write to the CSV file
    private IEnumerator WriteToCSVFileCoroutine()
    {
        while (true)
        {
            yield return null;

            // Check if the interval has passed since the last write
            if (Time.realtimeSinceStartup - lastWriteTime >= 0.01f)
            {
                lastWriteTime = Time.realtimeSinceStartup;
                WriteToCSVFile();
                
            }
        }
    }

    // Write the messages to the CSV file
    private void WriteToCSVFile()
    {
        if (messages.Count > 0)
        {
            using (StreamWriter writer = new StreamWriter(csvFilePath, true))
            {
                // Write each message with its corresponding timestamp
                for (int i = 0; i < messages.Count; i++)
                {
                    string timestamp = (lastWriteTime - 0.01f * (messages.Count - i - 1)).ToString();
                    writer.WriteLine($"{timestamp},{messages[i]}");
                }
            }

            // Clear the list of messages
            messages.Clear();
        }
    }
}
