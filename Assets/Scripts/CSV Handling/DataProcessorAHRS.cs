using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using System.IO;
using TMPro;
using UnityEngine.Android;
using UnityEngine.UI;

public class DataProcessorAHRS : MonoBehaviour
{
    public List<SensorTextPair> sensorTextPairs;
    private Dictionary<string, List<string>> synchronizedDataDict = new Dictionary<string, List<string>>();
    private Dictionary<string, StreamWriter> csvWriters = new Dictionary<string, StreamWriter>();
    private Dictionary<string, bool> fileCreatedFlags = new Dictionary<string, bool>();

    private Dictionary<string, MadgwickAHRS> ahrsObjects = new Dictionary<string, MadgwickAHRS>();

    public Dictionary<string, float> PitchValues { get; private set; } = new Dictionary<string, float>();
    public Dictionary<string, float> RollValues { get; private set; } = new Dictionary<string, float>();
    
    public static DataProcessorAHRS Instance { get; private set; }

    private string bodyPartName; // Variable to store the body part name received from the DropdownHandler script

    [System.Serializable]
    public class SensorTextPair
    {
        public string sensorName;
        public TextMeshProUGUI pitchText;
       public TextMeshProUGUI rollText;
        
    }


    public TextMeshProUGUI UpdateText;

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

             if (sensorData != null)
            {
             
             switch (sensorName)
            {
                case "Sensor1":
                    ProcessSensorData1(sensorData);
                    
                break;

                case "Sensor2":
                    ProcessSensorData2(sensorData);
                    UpdateText.text = ("Update called!");
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

    // Convert the sensor data to AHRS-compatible format
    Vector3 accelerometerData = new Vector3(accX, accY, accZ);
    Vector3 gyroscopeData = new Vector3(gyroX, gyroY, gyroZ);

    // Create a new MadgwickAHRS object for the sensor if it doesn't exist yet
    if (!ahrsObjects.ContainsKey(sensorName))
    {
        ahrsObjects[sensorName] = new MadgwickAHRS();
    }

    // Update the MadgwickAHRS object with the latest sensor data
    MadgwickAHRS ahrs = ahrsObjects[sensorName];
    ahrs.Update(gyroscopeData, accelerometerData);

    // Get the orientation quaternion from the MadgwickAHRS object
    float[] q = ahrs.Quaternion;
    Quaternion orientation = new Quaternion(q[1], q[2], q[3], q[0]);

    // Get the pitch and roll angles from the orientation quaternion
    float pitch = Mathf.Atan2(2 * (orientation.x * orientation.w - orientation.y * orientation.z),
                              1 - 2 * (orientation.x * orientation.x + orientation.y * orientation.y)) * Mathf.Rad2Deg;
    float roll = Mathf.Atan2(2 * (orientation.y * orientation.w + orientation.x * orientation.z),
                             1 - 2 * (orientation.y * orientation.y + orientation.x * orientation.x)) * Mathf.Rad2Deg;

    // Now you have the pitch and roll angles for the given sensor, log the values.
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
        }
    }
}



   

    private void OnApplicationQuit()
    {
        foreach (var writer in csvWriters.Values)
        {
            writer.Close();
        }
    }




    public class MadgwickAHRS
{
    // Implementation of the Madgwick algorithm for AHRS

    // Constructor
    public MadgwickAHRS(float sampleFrequency = 256f)
    {
        // Initialize algorithm parameters
        SamplePeriod = 1f / sampleFrequency;
        Beta = 0.1f;
        Quaternion = new float[] { 1f, 0f, 0f, 0f };
    }

    // Properties
    public float SamplePeriod { get; set; }
    public float Beta { get; set; }
    public float[] Quaternion { get; private set; }

    // Update method
    public void Update(Vector3 gyro, Vector3 accel)
    {
        float[] q = Quaternion;
        float sampleFreq = 1f / SamplePeriod;

        float q0 = q[0], q1 = q[1], q2 = q[2], q3 = q[3]; // Shorten the variable names for clarity
        float ax = accel.x, ay = accel.y, az = accel.z;
        float gx = gyro.x, gy = gyro.y, gz = gyro.z;

        // Compute the derivative of the quaternion
        float qDot0 = 0.5f * (-q1 * gx - q2 * gy - q3 * gz);
        float qDot1 = 0.5f * (q0 * gx + q2 * gz - q3 * gy);
        float qDot2 = 0.5f * (q0 * gy - q1 * gz + q3 * gx);
        float qDot3 = 0.5f * (q0 * gz + q1 * gy - q2 * gx);

        // Compute feedback only if accelerometer measurement valid (avoids NaN in accelerometer normalization)
        if (!((ax == 0.0f) && (ay == 0.0f) && (az == 0.0f)))
        {
            // Normalize accelerometer measurement
            float invSqrt = 1.0f / Mathf.Sqrt(ax * ax + ay * ay + az * az);
            ax *= invSqrt;
            ay *= invSqrt;
            az *= invSqrt;

            // Compute error between estimated and measured direction of gravity
            float halfvx = q1 * q3 - q0 * q2;
            float halfvy = q0 * q1 + q2 * q3;
            float halfvz = q0 * q0 - 0.5f + q3 * q3;

            // Compute and apply integral feedback
            qDot0 -= Beta * halfvx;
            qDot1 -= Beta * halfvy;
            qDot2 -= Beta * halfvz;
        }

        // Integrate rate of change of quaternion to yield quaternion
        q0 += qDot0 * SamplePeriod;
        q1 += qDot1 * SamplePeriod;
        q2 += qDot2 * SamplePeriod;
        q3 += qDot3 * SamplePeriod;

        // Normalize quaternion
        float invSqrtq = 1.0f / Mathf.Sqrt(q0 * q0 + q1 * q1 + q2 * q2 + q3 * q3);
        q[0] = q0 * invSqrtq;
        q[1] = q1 * invSqrtq;
        q[2] = q2 * invSqrtq;
        q[3] = q3 * invSqrtq;

        Quaternion = q;
    }
}
}
