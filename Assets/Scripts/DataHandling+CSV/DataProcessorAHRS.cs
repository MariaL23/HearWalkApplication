using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DataProcessorAHRS : MonoBehaviour
{

    public List<SensorTextPair> sensorTextPairs;
    private Dictionary<string, List<string>> synchronizedDataDict = new Dictionary<string, List<string>>(); // Dictionary to store the synchronized data for each sensor
    private Dictionary<string, StreamWriter> csvWriters = new Dictionary<string, StreamWriter>(); // Dictionary to store the CSV writers for each sensor
    private Dictionary<string, bool> fileCreatedFlags = new Dictionary<string, bool>(); // Dictionary to store the file creation flags for each sensor

    private Dictionary<string, List<string>> recordDataDict = new Dictionary<string, List<string>>(); // Store additional data for each sensor

    private Dictionary<string, float> lastMessage = new Dictionary<string, float>(); // Dictionary to store the last message time for each sensor

    public static DataProcessorAHRS Instance { get; private set; }  //Create instance

    private DataReceiver dataReceiver; // Define the DataReceiver script

    public string bodyPartName; // Variable to store the body part name received from the DropdownHandler script

    public MainDataProcessor mainDataProcessor;

    [System.Serializable]
    public class SensorTextPair // Class to store the sensor name and the corresponding text objects
    {
        public string sensorName;


        public TextMeshProUGUI GyroText;

    }

    public TextMeshProUGUI DisconnectText; // Define the DisconnectText object

    public GameObject DisconnectUI; // Define the DisconnectUI object

    public TextMeshProUGUI Debug1; // Define the Debug1 object

    public TextMeshProUGUI Debug2; // Define the Debug2 object

    private bool recordtoCSV = false;

  


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

    public List<string> GetAllSensorNames()
    {
        List<string> sensorNames = new List<string>();

        foreach (var pair in sensorTextPairs)
        {
            sensorNames.Add(pair.sensorName);
        }

        return sensorNames;
    }

    private void Start()
    {
        // get persmissions   
        RequestAndroidPermissions();
        InvokeRepeating("UpdateData", 0.033f, 0.033f);

    }

    private void UpdateData()
    {
        foreach (var pair in sensorTextPairs)
        {

            string sensorName = pair.sensorName;
            List<string> sensorData = GetSynchronizedData(sensorName);


            // Run data processing for each sensor
            if (sensorData != null && sensorData.Count > 0)
            {
                string[] sensorDataArray = sensorData[sensorData.Count - 1].Split(',');

                switch (sensorName)
                {
                    case "Sensor1":
                        ProcessSensorData1(sensorDataArray);
                        break;

                    case "Sensor2":
                        ProcessSensorData2(sensorDataArray);
                        break;

                    case "Sensor3":
                        ProcessSensorData3(sensorDataArray);
                        break;

                    case "Sensor4":
                        ProcessSensorData4(sensorDataArray);
                        break;
                }
            }

        }
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


        if (recordtoCSV && !string.IsNullOrEmpty(sensorName))
        {
            if (!recordDataDict.ContainsKey(sensorName))
            {
                recordDataDict[sensorName] = new List<string>();
            }

            // Add the data to the additionalFileDataDict only when writeToAdditionalFile is true
            recordDataDict[sensorName].Add(data);

        }

        MainDataProcessor mainDataProcessor = FindObjectOfType<MainDataProcessor>();
        if (mainDataProcessor != null)
        {
            
        }
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
        }
    }

    //write data to csv
    public void WriteToCSV(string sensorName, string data)
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
    public List<string> GetSynchronizedData(string sensorName)
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

    public void OnWriteRecord()
    {
        if (recordtoCSV)
        {
            foreach (var pair in recordDataDict)
            {
                string sensorName = pair.Key;
                List<string> sensorData = pair.Value;

                if (sensorData.Count == 0)
                {
                    Debug.Log($"No data to write for sensor: {sensorName}");
                    continue; // Skip this sensor's data if no data is available
                }

                string recordFolderPath = Path.Combine(GetAndroidExternalStoragePath(), "Documents", "Records", sensorName);
                if (!Directory.Exists(recordFolderPath))
                {
                    Directory.CreateDirectory(recordFolderPath);
                }

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string recordFileName = $"{bodyPartName}_{sensorName}_{timestamp}.csv";
                string recordFilePath = Path.Combine(recordFolderPath, recordFileName);

                using (StreamWriter writer = new StreamWriter(recordFilePath, true))
                {
                    writer.WriteLine("Timestamp,accX,accY,accZ,gyroX,gyroY,gyroZ,Battery");

                    foreach (string data in sensorData)
                    {
                        string timestampData = System.DateTime.Now.ToString("mm.ss.fff"); // Seconds and milliseconds
                        writer.WriteLine($"{timestampData},{data}");
                    }
                }

                Debug.Log($"Data written to file for sensor: {sensorName}");
            }
        }
    }



    public void OnStartButtonClicked()
    {
        recordtoCSV = true;
    }


    public void OnStopButtonClicked()
    {
        OnWriteRecord();
        recordtoCSV = false;

        // Clear the recordDataDict for each sensor
        foreach (var pair in recordDataDict)
        {
            pair.Value.Clear();
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
            writer.Dispose(); // Dispose the writer
        }
    }


    public PitchRollCalculations calculationsSensor1 = new PitchRollCalculations();
    public PitchRollCalculations calculationsSensor2 = new PitchRollCalculations();
    public PitchRollCalculations calculationsSensor3 = new PitchRollCalculations();
    public PitchRollCalculations calculationsSensor4 = new PitchRollCalculations();

    //Handle the data from the sensors
    public void ProcessSensorData1(string[] sensorDataArray)
    {
        List<string> dataSensor1 = GetSynchronizedData("Sensor1");
        if (sensorDataArray.Length >= 6)
        {
            string[] sensorData = dataSensor1[dataSensor1.Count - 1].Split(',');
            float accX = float.Parse(sensorData[0]);
            float accY = float.Parse(sensorData[1]);
            float accZ = float.Parse(sensorData[2]);
            float gyroX = float.Parse(sensorData[3]);
            float gyroY = float.Parse(sensorData[4]);
            float gyroZ = float.Parse(sensorData[5]);

            calculationsSensor1.SetAccBuf(accX, accY, accZ);
            calculationsSensor1.SetGyrBuf(gyroX, gyroY, gyroZ);
            calculationsSensor1.GetOrientation_Quaternion();
            float rollSensor1 = calculationsSensor1.GetPitch();
            float pitchSensor1 = calculationsSensor1.GetRoll();

            gyroZ = +gyroZ;
            pitchSensor1 = -pitchSensor1;
            rollSensor1 = -rollSensor1;

            foreach (var pair in sensorTextPairs)
            {
                if (pair.sensorName == "Sensor1")
                {
                    pair.GyroText.text = 
                   
                    "Gyro Z:  " + (gyroZ >= 0 ? "+" : "") + gyroZ.ToString("F2").PadRight(15) + "  " +
                    "\n" +
                    "Pitch: " + (pitchSensor1 >= 0 ? "+" : "") + pitchSensor1.ToString("F2").PadRight(15) + "  " +
                     "\n" +
                    "Roll: " + (rollSensor1 >= 0 ? "+" : "") + rollSensor1.ToString("F2").PadRight(15) + "  ";
                }
            }
        }

    }

    public float GetPitchSensor1()
    {
        float pitchSensor1 = calculationsSensor1.GetPitch();
        return pitchSensor1;
    }

    public float GetRollSensor1()
    {
        float rollSensor1 = calculationsSensor1.GetRoll();
        return rollSensor1;
    }

    /*
    public float GetRawDataSensor1() 
    {
        float GyroZ = gyrox;
        return GyroZ;
    }
    */
  

    public void ProcessSensorData2(string[] sensorDataArray)
    {
        List<string> dataSensor2 = GetSynchronizedData("Sensor2");
        if (sensorDataArray.Length >= 6)
        {
            string[] sensorData = dataSensor2[dataSensor2.Count - 1].Split(',');
            float accX = float.Parse(sensorData[0]);
            float accY = float.Parse(sensorData[1]);
            float accZ = float.Parse(sensorData[2]);
            float gyroX = float.Parse(sensorData[3]);
            float gyroY = float.Parse(sensorData[4]);
            float gyroZ = float.Parse(sensorData[5]);

            calculationsSensor2.SetAccBuf(accX, accY, accZ);
            calculationsSensor2.SetGyrBuf(gyroX, gyroY, gyroZ);
            calculationsSensor2.GetOrientation_Quaternion();
            float rollSensor2 = calculationsSensor2.GetPitch();
            float pitchSensor2 = calculationsSensor2.GetRoll();

            gyroZ = -gyroZ;
            pitchSensor2 = +pitchSensor2;
            rollSensor2 = -rollSensor2;

            foreach (var pair in sensorTextPairs)
            {
                if (pair.sensorName == "Sensor2")
                {
                    pair.GyroText.text =
                    
                    "Gyro Z: " + (gyroZ >= 0 ? "+" : "") + gyroZ.ToString("F2").PadRight(15) + "  " +
                    "\n" +
                    "Pitch: " + (pitchSensor2 >= 0 ? "+" : "") + pitchSensor2.ToString("F2").PadRight(15) + "  " +
                     "\n" +
                    "Roll: " + (rollSensor2 >= 0 ? "+" : "") + rollSensor2.ToString("F2").PadRight(15) + "  ";
                }
            }
        }

    }

    public float GetPitchSensor2()
    {
        float pitchSensor2 = calculationsSensor2.GetPitch();
        return pitchSensor2;
    }

    public float GetRollSensor2()
    {
        float rollSensor2 = calculationsSensor2.GetRoll();
        return rollSensor2;
    }



    public void ProcessSensorData3(string[] sensorDataArray)
    {
        List<string> dataSensor3 = GetSynchronizedData("Sensor3");
        if (sensorDataArray.Length >= 6)
        {
            string[] sensorData = dataSensor3[dataSensor3.Count - 1].Split(',');
            float accX = float.Parse(sensorData[0]);
            float accY = float.Parse(sensorData[1]);
            float accZ = float.Parse(sensorData[2]);
            float gyroX = float.Parse(sensorData[3]);
            float gyroY = float.Parse(sensorData[4]);
            float gyroZ = float.Parse(sensorData[5]);

            calculationsSensor3.SetAccBuf(accX, accY, accZ);
            calculationsSensor3.SetGyrBuf(gyroX, gyroY, gyroZ);
            calculationsSensor3.GetOrientation_Quaternion();
            float rollSensor3 = calculationsSensor3.GetPitch();
            float pitchSensor3 = calculationsSensor3.GetRoll();

            gyroZ = -gyroZ;
            pitchSensor3 = +pitchSensor3;
            rollSensor3 = -rollSensor3;



            foreach (var pair in sensorTextPairs)
            {
                if (pair.sensorName == "Sensor3")
                {
                    pair.GyroText.text = 
                   bodyPartName +
                    "Gyro Z: " + (gyroZ >= 0 ? "+" : "") + gyroZ.ToString("F2").PadRight(15) + "  " +
                    "\n" +
                    "Pitch: " + (pitchSensor3 >= 0 ? "+" : "") + pitchSensor3.ToString("F2").PadRight(15) + "  " +
                     "\n" +
                    "Roll: " + (rollSensor3 >= 0 ? "+" : "") + rollSensor3.ToString("F2").PadRight(15) + "  ";
                }
            }
        }
    }

    public float GetPitchSensor3()
    {
        float pitchSensor3 = calculationsSensor3.GetPitch();
        return pitchSensor3;
    }

    public float GetRollSensor3()
    {
        float rollSensor3 = calculationsSensor3.GetRoll();
        return rollSensor3;
    }



    public void ProcessSensorData4(string[] sensorDataArray)
    {
        List<string> dataSensor4 = GetSynchronizedData("Sensor4");
        if (sensorDataArray.Length >= 6)
        {
            string[] sensorData = dataSensor4[dataSensor4.Count - 1].Split(',');
            float accX = float.Parse(sensorData[0]);
            float accY = float.Parse(sensorData[1]);
            float accZ = float.Parse(sensorData[2]);
            float gyroX = float.Parse(sensorData[3]);
            float gyroY = float.Parse(sensorData[4]);
            float gyroZ = float.Parse(sensorData[5]);

            calculationsSensor4.SetAccBuf(accX, accY, accZ);
            calculationsSensor4.SetGyrBuf(gyroX, gyroY, gyroZ);
            calculationsSensor4.GetOrientation_Quaternion();
            float rollSensor4 = calculationsSensor4.GetPitch();
            float pitchSensor4 = calculationsSensor4.GetRoll();
           
            gyroZ = +gyroZ;
            pitchSensor4 = -pitchSensor4;
            rollSensor4 = -rollSensor4;

            foreach (var pair in sensorTextPairs)
            {
                if (pair.sensorName == "Sensor4")
                {
                    pair.GyroText.text = 
                     
                    "Gyro Z: " + (gyroZ >= 0 ? "+" : "") + gyroZ.ToString("F2").PadRight(15) + "  " +
                    "\n" +
                    "Pitch: " + (pitchSensor4 >= 0 ? "+" : "") + pitchSensor4.ToString("F2").PadRight(15) + "  " +
                     "\n" +
                    "Roll: " + (rollSensor4 >= 0 ? "+" : "") + rollSensor4.ToString("F2").PadRight(15) + "  ";

                   
                }
            }

            
        }

    }

    public float GetPitchSensor4()
    {
        float pitchSensor4 = calculationsSensor4.GetPitch();
        return pitchSensor4;
    }

    public float GetRollSensor4()
    {
        float rollSensor4 = calculationsSensor4.GetRoll();
        return rollSensor4;
    }

}


public class PitchRollCalculations {
    float beta = 1.5f;
    float[] q = { 1.0f, 0.0f, 0.0f, 0.0f };
    float[] eInt = { 0.0f, 0.0f, 0.0f };
    float[] eulerAngs = { 0.0f, 0.0f, 0.0f };

    float[] accBuf = { 0.0f, 0.0f, 0.0f };
    float[] gyrBuf = { 0.0f, 0.0f, 0.0f };
    float sampleFreq = 30;

    float pitch = 0;
    public float GetPitch() { return pitch; }

    float roll = 0;
    public float GetRoll() { return roll; }

    public void SetAccBuf(float accX, float accY, float accZ)
    {
        accBuf[0] = accX;
        accBuf[1] = accY;
        accBuf[2] = accZ;
    }

    public void SetGyrBuf(float gyrX, float gyrY, float gyrZ)
    {
        gyrBuf[0] = gyrX * (float)Math.PI / 180.0f;
        gyrBuf[1] = gyrY * (float)Math.PI / 180.0f;
        gyrBuf[2] = gyrZ * (float)Math.PI / 180.0f;
    }

    public void GetOrientation_Quaternion()
    {
        MadgwickAHRSupdateIMU(gyrBuf[0] * (float)Math.PI / 180.0f, gyrBuf[1] * (float)Math.PI / 180.0f, gyrBuf[2] * (float)Math.PI / 180.0f,
            accBuf[0], accBuf[1], accBuf[2]);

        roll = -(float)Math.Asin(2.0f * (q[1] * q[3] - q[0] * q[2]));
        pitch = (float)Math.Atan2(2.0f * (q[0] * q[1] + q[2] * q[3]), q[0] * q[0] - q[1] * q[1] - q[2] * q[2] + q[3] * q[3]);

        roll *= 180.0f / (float)Math.PI;
        pitch *= 180.0f / (float)Math.PI;
        pitch -= 90;
        roll = float.IsNaN(roll) ? 0 : roll;
        pitch = float.IsNaN(pitch) ? 0 : pitch;
    }

    public void MadgwickAHRSupdateIMU(float gx, float gy, float gz, float ax, float ay, float az)
    {
        float recipNorm;
        float s0, s1, s2, s3;
        float qDot1, qDot2, qDot3, qDot4;
        float _2q0, _2q1, _2q2, _2q3, _4q0, _4q1, _4q2, _8q1, _8q2, q0q0, q1q1, q2q2, q3q3;

        // Adjust for Present
        float q0 = q[0];
        float q1 = q[1];
        float q2 = q[2];
        float q3 = q[3];

        // Rate of change of quaternion from gyroscope
        qDot1 = 0.5f * (-q1 * gx - q2 * gy - q3 * gz);
        qDot2 = 0.5f * (q0 * gx + q2 * gz - q3 * gy);
        qDot3 = 0.5f * (q0 * gy - q1 * gz + q3 * gx);
        qDot4 = 0.5f * (q0 * gz + q1 * gy - q2 * gx);

        // Compute feedback only if accelerometer measurement valid (avoids NaN in accelerometer normalization)
        if (!((ax == 0.0f) && (ay == 0.0f) && (az == 0.0f)))
        {
            // Normalise accelerometer measurement
            recipNorm = InvSqrt(ax * ax + ay * ay + az * az);
            ax *= recipNorm;
            ay *= recipNorm;
            az *= recipNorm;

            // Auxiliary variables to avoid repeated arithmetic
            _2q0 = 2.0f * q0;
            _2q1 = 2.0f * q1;
            _2q2 = 2.0f * q2;
            _2q3 = 2.0f * q3;
            _4q0 = 4.0f * q0;
            _4q1 = 4.0f * q1;
            _4q2 = 4.0f * q2;
            _8q1 = 8.0f * q1;
            _8q2 = 8.0f * q2;
            q0q0 = q0 * q0;
            q1q1 = q1 * q1;
            q2q2 = q2 * q2;
            q3q3 = q3 * q3;

            // Gradient descent algorithm corrective step
            s0 = _4q0 * q2q2 + _2q2 * ax + _4q0 * q1q1 - _2q1 * ay;
            s1 = _4q1 * q3q3 - _2q3 * ax + 4.0f * q0q0 * q1 - _2q0 * ay - _4q1 + _8q1 * q1q1 + _8q1 * q2q2 + _4q1 * az;
            s2 = 4.0f * q0q0 * q2 + _2q0 * ax + _4q2 * q3q3 - _2q3 * ay - _4q2 + _8q2 * q1q1 + _8q2 * q2q2 + _4q2 * az;
            s3 = 4.0f * q1q1 * q3 - _2q1 * ax + 4.0f * q2q2 * q3 - _2q2 * ay;
            recipNorm = InvSqrt(s0 * s0 + s1 * s1 + s2 * s2 + s3 * s3); // normalise step magnitude
            s0 *= recipNorm;
            s1 *= recipNorm;
            s2 *= recipNorm;
            s3 *= recipNorm;

            // Apply feedback step
            qDot1 -= beta * s0;
            qDot2 -= beta * s1;
            qDot3 -= beta * s2;
            qDot4 -= beta * s3;
        }

        // Integrate rate of change of quaternion to yield quaternion
        q0 += qDot1 * (1.0f / sampleFreq);
        q1 += qDot2 * (1.0f / sampleFreq);
        q2 += qDot3 * (1.0f / sampleFreq);
        q3 += qDot4 * (1.0f / sampleFreq);

        // Normalize quaternion
        recipNorm = InvSqrt(q0 * q0 + q1 * q1 + q2 * q2 + q3 * q3);
        q[0] = q0 * recipNorm;
        q[1] = q1 * recipNorm;
        q[2] = q2 * recipNorm;
        q[3] = q3 * recipNorm;
    }

    float InvSqrt(float x)
    {
        return 1.0f / (float)Math.Sqrt(x);
    }
}
   

