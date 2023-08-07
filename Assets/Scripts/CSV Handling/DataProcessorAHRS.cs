using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using System.IO;
using TMPro;
using UnityEngine.Android;
using UnityEngine.UI;
using System;

public class DataProcessorAHRS : MonoBehaviour
{
    public List<SensorTextPair> sensorTextPairs;
    private Dictionary<string, List<string>> synchronizedDataDict = new Dictionary<string, List<string>>(); // Dictionary to store the synchronized data for each sensor
    private Dictionary<string, StreamWriter> csvWriters = new Dictionary<string, StreamWriter>(); // Dictionary to store the CSV writers for each sensor
    private Dictionary<string, bool> fileCreatedFlags = new Dictionary<string, bool>(); // Dictionary to store the file creation flags for each sensor

    private Dictionary<string, MadgwickAHRS> ahrsObjects = new Dictionary<string, MadgwickAHRS>(); // Dictionary to store the MadgwickAHRS objects for each sensor

    public Dictionary<string, float> PitchValues { get; private set; } = new Dictionary<string, float>(); // Dictionary to store the pitch values for each sensor
    public Dictionary<string, float> RollValues { get; private set; } = new Dictionary<string, float>(); // Dictionary to store the roll values for each sensor

    private Dictionary<string, float> lastMessage = new Dictionary<string, float>(); // Dictionary to store the last message time for each sensor
    
    public static DataProcessorAHRS Instance { get; private set; }  //Create instance

    private DataReceiver dataReceiver; // Define the DataReceiver script

    private string bodyPartName; // Variable to store the body part name received from the DropdownHandler script

    [System.Serializable]
    public class SensorTextPair // Class to store the sensor name and the corresponding text objects
    {
        public string sensorName;
        public TextMeshProUGUI pitchText;
        public TextMeshProUGUI rollText;

        public TextMeshProUGUI GyroText;
        
    }

    public TextMeshProUGUI DisconnectText; // Define the DisconnectText object

    public GameObject DisconnectUI; // Define the DisconnectUI object

    


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

   

      //permissions from android
    private void RequestAndroidPermissions()
    {

        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.ExternalStorageWrite))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.ExternalStorageWrite);
        }

    }

    private void Start()
    {
        // get persmissions   
        RequestAndroidPermissions();
        
    }

      public void SetBodyPartName(string name)
    {
        bodyPartName = name; // Set the body part name received from the DropdownController script
    }

    public void ProcessData(string sensorName, string data) // Process the data received from the DataReceiver script
    {
          if (!lastMessage.ContainsKey(sensorName))
        {
            lastMessage.Add(sensorName, Time.time);
        }
        else
        {
            lastMessage[sensorName] = Time.time;
        }


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
    
    //create csv file settings
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
        //check sensor connection
        CheckForInactivity();
        
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
              // Run data processing for each sensor
             if (sensorData != null)
            {
             
             switch (sensorName)
            {
                case "Sensor1":
                    ProcessSensorData1(sensorData);
                    
                break;

                case "Sensor2":
                    ProcessSensorData2(sensorData);
                    
                break;

                case "Sensor3":
                    ProcessSensorData3(sensorData);
                    
                break;

                case "Sensor4":
                    ProcessSensorData4(sensorData);
                    
                break;

               
            }   
            }

        }
    }

    //write data to csv
    private void WriteToCSV(string sensorName, string data)
    {
         if (!csvWriters.ContainsKey(sensorName))
    {
        Debug.LogError("CSV writer not found for sensor: " + sensorName);
        return;
    }

    StreamWriter writer = csvWriters[sensorName];
    string timestamp = System.DateTime.Now.ToString("mm.ss.fff"); // Seconds and milliseconds
    string line = $"{timestamp},{data}";
    writer.WriteLine(line);
    }
      
    //getting synced data
    private List<string> GetSynchronizedData(string sensorName)
    {
        if (synchronizedDataDict.ContainsKey(sensorName))
        {
            return synchronizedDataDict[sensorName];
        }

        return null;
    }

     //get android storage path
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

    
     //Handle the data from the sensors
     private void ProcessSensorData1(List<string> data)
    {
        ProcessSensorData("Sensor1", data);
       
    }

    private void ProcessSensorData2(List<string> data)
    {
       ProcessSensorData("Sensor2", data);
    }

    private void ProcessSensorData3(List<string> data)
    {
       ProcessSensorData("Sensor3", data);
    }

    private void ProcessSensorData4(List<string> data)
    {
        ProcessSensorData("Sensor4", data);
    }



  private void ProcessSensorData(string sensorName, List<string> data)
{
    // Process data here for the given sensor
    string[] sensorData = data[data.Count - 1].Split(',');

    // Extract accelerometer and gyroscope data from the sensorData array
    float accX = float.Parse(sensorData[0]);
    float accY = float.Parse(sensorData[1]);
    float accZ = float.Parse(sensorData[2]);
    float gyroX = float.Parse(sensorData[3]);
    float gyroY = float.Parse(sensorData[4]);
    float gyroZ = float.Parse(sensorData[5]);
  
 Vector3 accelerometerData = new Vector3(
    accX * Mathf.Deg2Rad,
    accY * Mathf.Deg2Rad,
    accZ * Mathf.Deg2Rad
);
Vector3 gyroscopeData = new Vector3(
    gyroX * Mathf.Deg2Rad,
    gyroY * Mathf.Deg2Rad,
    gyroZ * Mathf.Deg2Rad
);

    // Create a new MadgwickAHRS object for the sensor if it doesn't exist yet
    if (!ahrsObjects.ContainsKey(sensorName))
    {
        ahrsObjects[sensorName] = new MadgwickAHRS();
    }

    // Update the MadgwickAHRS object with the latest sensor data
    MadgwickAHRS ahrs = ahrsObjects[sensorName];
    ahrs.Update(gyroX, gyroY, gyroZ, accX, accY, accZ); // Pass the sensor data to the Madgwick algorithm

    // Get the orientation quaternion from the MadgwickAHRS object
    float[] q = ahrs.Quaternion;
    Quaternion orientation = new Quaternion(q[1], q[2], q[3], q[0]);

    // Get the pitch and roll angles from the orientation quaternion
    float pitch = Mathf.Atan2(2 * (orientation.x * orientation.w - orientation.y * orientation.z),
                              1 - 2 * (orientation.x * orientation.x + orientation.y * orientation.y)) * Mathf.Rad2Deg;
    float roll = Mathf.Atan2(2 * (orientation.y * orientation.w + orientation.x * orientation.z),
                             1 - 2 * (orientation.y * orientation.y + orientation.x * orientation.x)) * Mathf.Rad2Deg;

    //pitch and roll angles for the given sensor, log the values.
    Debug.Log(sensorName + " - Pitch: " + pitch + ", Roll: " + roll);
    
    // Store the pitch and roll values in the dictionaries
    PitchValues[sensorName] = pitch;
    RollValues[sensorName] = roll;

    foreach (var pair in sensorTextPairs)
    {
        if (pair.sensorName == sensorName)
        {
            pair.pitchText.text = "Pitch: " + pitch.ToString("F2");
            pair.rollText.text = "Roll: " + roll.ToString("F2");
            pair.GyroText.text = bodyPartName + "\nGyro X:" + gyroX.ToString("F2") + "\nGyroY " + gyroY.ToString("F2") + "\nGyroZ " + gyroZ.ToString("F2");          
        }
    }
}


private void CheckForInactivity() //check for inactivity
{
    List<string> disconnectedSensors = new List<string>();

    foreach (var kvp in lastMessage)
    {
        string sensorName = kvp.Key;
        float lastMessageTime = kvp.Value;

        // Check if no messages have been received for this sensor for more than 1 second
        if (Time.time - lastMessageTime >= 1f)
        {
            Debug.Log($"No OSC messages received from {sensorName} in the last second.");
            disconnectedSensors.Add(bodyPartName);
        }
    }

    if (disconnectedSensors.Count > 0) //if no messages have been received for this sensor for more than 1 second
    {
        DisconnectText.text = string.Join(", ", disconnectedSensors) + " Disconnected";
        DisconnectUI.SetActive(true);
    }
    else
    {
        DisconnectText.text = "";
    }
}



    private void OnApplicationQuit()
    {
        foreach (var writer in csvWriters.Values)
        {
            writer.Close(); // Close the CSV writer
        }
    }



    public class MadgwickAHRS
{
    

    // Constructor
    public MadgwickAHRS(float samplePeriod = 0.01f)
    {
        // Initialize algorithm parameters
        Beta = 0.6f;
        Quaternion = new float[] { 1f, 0f, 0f, 0f };
        SamplePeriod = samplePeriod;
    }

    // Properties
    public float Beta 
    public float[] Quaternion 
    public float SamplePeriod 
    // Update method
       public void Update(float gx, float gy, float gz, float ax, float ay, float az)
        {
            float q1 = Quaternion[0], q2 = Quaternion[1], q3 = Quaternion[2], q4 = Quaternion[3];   // short name local variable for readability
            float norm;
            float s1, s2, s3, s4;
            float qDot1, qDot2, qDot3, qDot4;

            // Auxiliary variables to avoid repeated arithmetic
            float _2q1 = 2f * q1;
            float _2q2 = 2f * q2;
            float _2q3 = 2f * q3;
            float _2q4 = 2f * q4;
            float _4q1 = 4f * q1;
            float _4q2 = 4f * q2;
            float _4q3 = 4f * q3;
            float _8q2 = 8f * q2;
            float _8q3 = 8f * q3;
            float q1q1 = q1 * q1;
            float q2q2 = q2 * q2;
            float q3q3 = q3 * q3;
            float q4q4 = q4 * q4;

            // Normalise accelerometer measurement
            norm = (float)Math.Sqrt(ax * ax + ay * ay + az * az);
            if (norm == 0f) return; // handle NaN
            norm = 1 / norm;        // use reciprocal for division
            ax *= norm;
            ay *= norm;
            az *= norm;

            // Gradient decent algorithm corrective step
            s1 = _4q1 * q3q3 + _2q3 * ax + _4q1 * q2q2 - _2q2 * ay;
            s2 = _4q2 * q4q4 - _2q4 * ax + 4f * q1q1 * q2 - _2q1 * ay - _4q2 + _8q2 * q2q2 + _8q2 * q3q3 + _4q2 * az;
            s3 = 4f * q1q1 * q3 + _2q1 * ax + _4q3 * q4q4 - _2q4 * ay - _4q3 + _8q3 * q2q2 + _8q3 * q3q3 + _4q3 * az;
            s4 = 4f * q2q2 * q4 - _2q2 * ax + 4f * q3q3 * q4 - _2q3 * ay;
            norm = 1f / (float)Math.Sqrt(s1 * s1 + s2 * s2 + s3 * s3 + s4 * s4);    // normalise step magnitude
            s1 *= norm;
            s2 *= norm;
            s3 *= norm;
            s4 *= norm;

            // Compute rate of change of quaternion
            qDot1 = 0.5f * (-q2 * gx - q3 * gy - q4 * gz) - Beta * s1;
            qDot2 = 0.5f * (q1 * gx + q3 * gz - q4 * gy) - Beta * s2;
            qDot3 = 0.5f * (q1 * gy - q2 * gz + q4 * gx) - Beta * s3;
            qDot4 = 0.5f * (q1 * gz + q2 * gy - q3 * gx) - Beta * s4;

            // Integrate to yield quaternion
            q1 += qDot1 * SamplePeriod;
            q2 += qDot2 * SamplePeriod;
            q3 += qDot3 * SamplePeriod;
            q4 += qDot4 * SamplePeriod;
            norm = 1f / (float)Math.Sqrt(q1 * q1 + q2 * q2 + q3 * q3 + q4 * q4);    // normalise quaternion
            Quaternion[0] = q1 * norm;
            Quaternion[1] = q2 * norm;
            Quaternion[2] = q3 * norm;
            Quaternion[3] = q4 * norm;
        }
    }
}
