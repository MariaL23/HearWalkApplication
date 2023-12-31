using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;
using TMPro;
using UnityEngine.Android;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;

public class DataHandlerCSV : MonoBehaviour
{    
    [Header("OSC Settings")]  
    
    public OSCReceiver receiver; //Define the OSC receiver and the text object
    public string Address = "/S"; // Define the OSC address
    
    [Header("CSV Settings")]  
    public string folderName = "Documents/Sensor/"  ; // Define the folder name
    
    public string nameOfFile = "Sensor"; // Define the name of the CSV file
    

    [Header("UI Settings")]  
    public TextMeshProUGUI messageText; // Define the text object

    public TextMeshProUGUI countText;  // Define the text object
    public int count; // Define the count variable

    //Define the variables to save the messages to a CSV file
    private List<string> messages = new List<string>(); // Initialize the messages list
    private string csvFilePath;
    private string csvSeparator = ",";
    private string[] csvHeaders = { "Timestamp", "accX", "accY", "accZ", "gyroX", "gyroY", "gyroZ", "Battery"};
    
    // Define the coroutine to save the CSV file
    private Coroutine saveCoroutine;

    public bool isChosen= false;


    public void whenChosen()
    {
        isChosen = true;
        Debug.Log("Chosen" + nameOfFile);
        
        if (isChosen == true)
        {
        // Bind the OSC receiver to the address
        receiver.Bind(Address, MessageReceived);

        // Request necessary permissions
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
        // Create the CSV file
        CreateCSVFile();

        // Start the coroutine to save the CSV file every 5 seconds
        saveCoroutine = StartCoroutine(SaveCSVFileCoroutine(5f));
    }
    }

    private void OnDestroy()
    {
        // Stop the coroutine when the object is destroyed
        if (saveCoroutine != null)
        {
            StopCoroutine(saveCoroutine);
        }
    }
     //Save the CSV file when the application is quit
    private void OnApplicationQuit()
    {
        SaveMessagesToCSV();
    }
     // Receive the OSC message
    public void MessageReceived(OSCMessage message)
    {     
            messageText.text = message.ToString();
            count++;
            countText.text = count.ToString();
            
             // Extract timestamp and value from the OSC message
             string timestamp = Time.time.ToString();

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
    string formattedMessage = $"{timestamp},{string.Join(",", stringValues)}";
    
    // Add the formatted message to the list
    messages.Add(formattedMessage);
        
    }
    // Create the CSV file
    public void CreateCSVFile()
    {
        csvFilePath = GetCSVFilePath();

        if (!File.Exists(csvFilePath))
        {
            using (StreamWriter writer = new StreamWriter(csvFilePath))
            {
                writer.WriteLine(string.Join(csvSeparator, csvHeaders));
            }
        }
    }
 
    private string GetCSVFilePath()

    {    //Name the CSV File SensorData_YYYYMMDDHHMM.csv 
        string timeStamp = System.DateTime.Now.ToString("yyyyMMddHHmm");
        string fileName = nameOfFile +"-" + timeStamp + ".csv";
        string folderPath = Path.Combine(GetExternalStoragePath(), folderName);

        // Create the folder if it doesn't exist
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        return Path.Combine(folderPath, fileName);
    }

    // Ensure that the file is saved in documents folder on Android
    private string GetExternalStoragePath()
    {
        string externalStoragePath = "";

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
   
   //Define the coroutine to save the CSV file 
    public IEnumerator SaveCSVFileCoroutine(float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(00.01f);
            SaveMessagesToCSV();
        }
    }
  // Save the messages to the CSV file
    public void SaveMessagesToCSV()
    {
        if (messages.Count > 0)
        {
            using (StreamWriter writer = new StreamWriter(csvFilePath, true))
            {
                foreach (string message in messages)
                {
                    writer.WriteLine(message);
                }
            }
  // Clear the list of messages
            messages.Clear();
        }
    }
}
