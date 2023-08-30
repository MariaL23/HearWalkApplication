using extOSC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
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
    public TextMeshProUGUI Debug3;
    public TextMeshProUGUI Debug4;


    private bool recordtoCSV = false;

    // Text Elements for the Movement Features
    public TextMeshProUGUI ShankLtext;
    public TextMeshProUGUI ShankRtext;
    public TextMeshProUGUI ThighLtext;
    public TextMeshProUGUI ThighRtext;

    public TextMeshProUGUI KneeLtext;
    public TextMeshProUGUI KneeRtext;
    public TextMeshProUGUI FootLvsRtext;

    public MovementFeature[] movementFeatures = new MovementFeature[7];
    public Preprocessor[] preprocessors = new Preprocessor[6];
    public Postprocessor[] postprocessors = new Postprocessor[6];

    public TextMeshProUGUI[] preprocessorInputText = new TextMeshProUGUI[6];
    public TextMeshProUGUI[] preprocessorOutputText = new TextMeshProUGUI[6];
    public TextMeshProUGUI[] postprocessorOutputText = new TextMeshProUGUI[6];

    public TextMeshProUGUI preprocessorInput1Text;
    public int preproc_InUse = 1;
    public int postproc_InUse = 1;

    public RTransmitter[] transmitters = new RTransmitter[6];


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }




 


    private void Start()
    {
        // get persmissions   
        RequestAndroidPermissions();
        InvokeRepeating("UpdateData", 0.033f, 0.033f);
     
       

        movementFeatures[0] = new MovementFeature("Angular_Velocity_ThighL", -110, 110);
        movementFeatures[1] = new MovementFeature("Angular_Velocity_ThighR", -110, 110);
        movementFeatures[2] = new MovementFeature("Angular_Velocity_ShankL", -250, 250);
        movementFeatures[3] = new MovementFeature("Angular_Velocity_ShankR", -250, 250);
        movementFeatures[4] = new MovementFeature("Joint_Angle_KneeL", -10, 90);
        movementFeatures[5] = new MovementFeature("Joint_Angle_KneeR", -10, 90);
        movementFeatures[6] = new MovementFeature("Foot_Position_Lvs/R", -1, 1);

        preprocessors[0] = new Preprocessor(-110, 110, 0, 110, 5, false);
        preprocessors[1] = new Preprocessor(-110, 110, 0, 110, 5, false);
        preprocessors[2] = new Preprocessor(-110, 110, 0, 110, 5, false);
        preprocessors[3] = new Preprocessor(-110, 110, 0, 110, 5, false);
        preprocessors[4] = new Preprocessor(-110, 110, 0, 110, 5, false);
        preprocessors[5] = new Preprocessor(-110, 110, 0, 110, 5, false);
       

        postprocessors[0] = new Postprocessor(1,false,800);
        postprocessors[1] = new Postprocessor(1, false, 0);
        postprocessors[2] = new Postprocessor(1, false, 0);
        postprocessors[3] = new Postprocessor(1, false, 0);
        postprocessors[4] = new Postprocessor(1, false, 0);
        postprocessors[5] = new Postprocessor(1, false, 0);


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


        ComputeMovFeatures(gyroZSensor2,gyroZSensor1, gyroZSensor3, gyroZSensor4,
            GetRollSensor2(), GetRollSensor1(), GetRollSensor3(), GetRollSensor4() );

        
        

        ShankLtext.text = movementFeatures[2].getValue().ToString("F2");
        ShankRtext.text = movementFeatures[3].getValue().ToString("F2");
        ThighLtext.text = movementFeatures[0].getValue().ToString("F2");
        ThighRtext.text = movementFeatures[1].getValue().ToString("F2");
        KneeLtext.text = movementFeatures[4].getValue().ToString("F2");
        KneeRtext.text = movementFeatures[5].getValue().ToString("F2");
        FootLvsRtext.text = movementFeatures[6].getValue().ToString("F2");

       
        preprocessors[0].Process(movementFeatures[0].getValue());
        postprocessors[0].Process(preprocessors[0].outputVal);
    



        for (int i = 0; i < 6; i++)
        {
            preprocessorInputText[i].text = preprocessors[i].inputVal.ToString("F2");
            preprocessorOutputText[i].text = preprocessors[i].outputVal.ToString("F2");
            postprocessorOutputText[i].text = postprocessors[i].outputVal.ToString("F2");
        }


        for (int i = 0; i < 6; i++)
        {
            string messageContent = postprocessors[i].outputVal.ToString("F2");
            transmitters[i].SendOSCMessage(messageContent);
           
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

    public float gyroZSensor1;
    public float gyroZSensor2;
    public float gyroZSensor3;
    public float gyroZSensor4;

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

            gyroZSensor1 = gyroZ;

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
            gyroZSensor2 = gyroZ;

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
            gyroZSensor3 = gyroZ;



            foreach (var pair in sensorTextPairs)
            {
                if (pair.sensorName == "Sensor3")
                {
                    pair.GyroText.text = 
                  
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

            gyroZSensor4 = gyroZ;

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




    public void ComputeMovFeatures(float gyrZ_Thigh_L, float gyrZ_Thigh_R, float gyrZ_Shank_L, float gyrZ_Shank_R,
    float pitchDeg_Thigh_L, float pitchDeg_Thigh_R, float pitchDeg_Shank_L, float pitchDeg_Shank_R)
    {
        float angVel_Thigh_L = gyrZ_Thigh_L;
        float angVel_Thigh_R = gyrZ_Thigh_R;
        float angVel_Shank_L = gyrZ_Shank_L;
        float angVel_Shank_R = gyrZ_Shank_R;
        float kneeAng_L = pitchDeg_Thigh_L - pitchDeg_Shank_L ;
        float kneeAng_R = pitchDeg_Shank_R - pitchDeg_Thigh_R;
        float footPos_L = (float)(0.511 * Math.Sin(pitchDeg_Thigh_L * Math.PI / 180.0) + 0.489 * Math.Sin(pitchDeg_Shank_L * Math.PI / 180.0));
        float footPos_R = (float)(0.511 * Math.Sin(pitchDeg_Thigh_R * Math.PI / 180.0) + 0.489 * Math.Sin(pitchDeg_Shank_R * Math.PI / 180.0));

        // Assuming movementFeatures is an accessible object with a storeValue method
        movementFeatures[0].storeValue(angVel_Thigh_L);
        movementFeatures[1].storeValue(angVel_Thigh_R);
        movementFeatures[2].storeValue(angVel_Shank_L);
        movementFeatures[3].storeValue(angVel_Shank_R);
        movementFeatures[4].storeValue(kneeAng_L);
        movementFeatures[5].storeValue(kneeAng_R);
        movementFeatures[6].storeValue(footPos_L + footPos_R);
    }

    void PreProcessor()
    {


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


    //permissions from android
    private void RequestAndroidPermissions()
    {

        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.ExternalStorageWrite))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.ExternalStorageWrite);
        }

    }



}


class SmoothingFilter
{
    private const double M_PI = 3.141592653589793238462643383279502884;

    private float m_f_Xz_1; // x z-1 delay element
    private float m_f_Xz_2; // x z-2 delay element
    private float m_f_Yz_1; // y z-1 delay element
    private float m_f_Yz_2; // y z-2 delay element

    private double m_f_a0 = 0;
    private double m_f_a1 = 0;
    private double m_f_a2 = 0;
    private double m_f_b1 = 0;
    private double m_f_b2 = 0;

    public SmoothingFilter(float fc, float q, float fs)
    {
        FlushDelays();
        CalculateLPFCoeffs(fc, q, fs); // Initialize filter at startup
    }

    ~SmoothingFilter()
    {
        // Destructor
    }

    private void FlushDelays()
    {
        m_f_Xz_1 = 0;
        m_f_Xz_2 = 0;
        m_f_Yz_1 = 0;
        m_f_Yz_2 = 0;
    }

    public float DoFiltering(float f_xn)
    {
        // Calculate filter output
        float yn = (float)(m_f_a0 * f_xn + m_f_a1 * m_f_Xz_1 + m_f_a2 * m_f_Xz_2
            - m_f_b1 * m_f_Yz_1 - m_f_b2 * m_f_Yz_2);

        // Delay Shuffle
        m_f_Xz_2 = m_f_Xz_1;
        m_f_Xz_1 = f_xn;
        m_f_Yz_2 = m_f_Yz_1;
        m_f_Yz_1 = yn;

        // Check for NaN and return output
        if (float.IsNaN(yn)) yn = 0;
        return yn;
    }

    private void CalculateLPFCoeffs(float fCutoffFreq, float fQ, float fs)
    {
        // Use same terms as in book:
        float theta_c = 2.0f * (float)M_PI * fCutoffFreq / fs;
        float d = 1.0f / fQ;

        // Intermediate values
        float fBetaNumerator = 1.0f - ((d / 2.0f) * (float)Math.Sin(theta_c));
        float fBetaDenominator = 1.0f + ((d / 2.0f) * (float)Math.Sin(theta_c));

        // Beta
        float fBeta = 0.5f * (fBetaNumerator / fBetaDenominator);

        // Gamma
        float fGamma = (0.5f + fBeta) * (float)Math.Cos(theta_c);

        // Alpha
        float fAlpha = (0.5f + fBeta - fGamma) / 2.0f;

        // Coefficients
        m_f_a0 = (0.5f + fBeta - fGamma) / 2.0f;
        m_f_a1 = 0.5f + fBeta - fGamma;
        m_f_a2 = (0.5f + fBeta - fGamma) / 2.0f;
        m_f_b1 = -2 * fGamma;
        m_f_b2 = 2 * fBeta;
    }
}

public class EnvelopeFollower
{
    private double attack = 0.0;
    private double release = 0.0;
    private double tc = -4.6051701859880913680359829093687;
    private double env_val = 0;

    public EnvelopeFollower(float releaseMs)
    {
        Set_TC(0.0f, releaseMs);
    }

    ~EnvelopeFollower()
    {
        // Destructor
    }

    private void Set_TC(float attackMs, float releaseMs)
    {
        attack = (attackMs > 0.0001f) ? Math.Exp(tc / (attackMs * 100 * 0.001)) : 0.0;
        release = (releaseMs > 0.0001f) ? Math.Exp(tc / (releaseMs * 100 * 0.001)) : 0.0;
    }

    public double GetEnvelope(double input)
    {
        if (input > env_val)
            env_val = input + attack * (env_val - input);
        else
            env_val = input + release * (env_val - input);

        return env_val; // Return the updated envelope value
    }
}


public class Preprocessor
{
    // Processing Helper Elements
    private SmoothingFilter filt;

    // Fixed Parameters
    private float feat_MIN_Global;
    private float feat_MAX_Global;
    private float filt_Fc_LPF;

    // User-Modifiable Parameters
    private float feat_MIN_OfInterest;
    private float feat_MAX_OfInterest;
    private bool isInverted = false;

    // Helper variables
    public float inputVal = 0;
    public float outputVal = 0;
    public bool isInitialized = false;

    // Constructor
    public Preprocessor(float featMinG, float featMaxG, float featMinI, float featMaxI, float filt_fc_L, bool isInv)
    {
        // Initialize filt as needed
        feat_MIN_Global = featMinG;
        feat_MAX_Global = featMaxG;
        feat_MIN_OfInterest = featMinI;
        feat_MAX_OfInterest = featMaxI;
        filt_Fc_LPF = filt_fc_L;
        filt = new SmoothingFilter(filt_Fc_LPF, 0.7f, 30); // Replace parameters as needed
        isInverted = isInv;
        isInitialized = true;
    }

    // Destructor
    ~Preprocessor()
    {
        // Destructor
    }

    // Initialize - has to be called prior to use (!)
    public void Initialize()
    {
     
    }

    // Real-time setters for movement feature range, specify whether modifying min value or max value
    public void SetFeatRange_OfInterest(float val, bool isMin)
    {
        if (isMin)
            feat_MIN_OfInterest = val;
        else
            feat_MAX_OfInterest = val;
    }

    // Real-time setter for inversion flag
    public void SetIsInverted(bool isInv)
    {
        isInverted = isInv;
    }

    // Helper functions
    private float ApplyNormalize(float input)
    {
        if (input <= feat_MIN_OfInterest) return 0;
        if (input >= feat_MAX_OfInterest) return 1;
        return (input - feat_MIN_OfInterest) / (feat_MAX_OfInterest - feat_MIN_OfInterest);
    }

    // Movement feature is processed in every callback using this function
    public void Process(float input)
    {
        if (isInitialized)
        {
            inputVal = input;
            outputVal = filt.DoFiltering(inputVal);
            outputVal = ApplyNormalize(outputVal);
            if (isInverted) outputVal = 1 - outputVal;
        }
    }
}


public class Postprocessor
{
    // Processing Helper Elements
    private EnvelopeFollower envFol;

    // Fixed Parameters
    private short mapFuncType = 1; // 1 = Exp, 2 = Sig, 3 = Lgt
    private bool isInverted = false;
    private float envRel_ms = 0;

    // User-Modifiable Parameters
    private float mapFunc_shape = 1f;

    // Helper Variables
   public float outputVal = 0;
    private bool isInitialized = false;

    // Constructor
    public Postprocessor(short mapfn_type, bool isInv, float envReleaseMS)
    {
        // Initialize envFol as needed
        mapFuncType = mapfn_type;
        isInverted = isInv;
        envRel_ms = envReleaseMS;
        envFol = new EnvelopeFollower(envRel_ms); // Replace the parameter with an appropriate value
        isInitialized = true;
    }

    // Destructor
    ~Postprocessor()
    {
        // Destructor
    }

    // Initializer Function - has to be called for each instance prior to use (!)
    public void Initialize(short mapfn_type, bool isInv, float envReleaseMS)
    {
     
    }

    // Real-time Setter - Mapping Function Shape
    public void SetMapFuncShape(float val)
    {
        mapFunc_shape = val;
    }

    // Apply nonlinear mapping function
    private float ApplyMapFunc(float input)
    {
        float output = 0;

        switch (mapFuncType)
        {
            case 1:
                output = (float)Math.Pow(input, mapFunc_shape);
                break;
            case 2:
                output = 1.0f / (1 + (float)Math.Exp(-mapFunc_shape * (10 * input - 5)));
                break;
            case 3:
                output = 0.5f + (float)(mapFunc_shape * Math.Log(input) / (1 - input * 1 / (16 + 8 * (mapFunc_shape - 1))));
                break;
            case 4:
                // If you still need to use sinPhase, declare it and update it here
                // float sinPhase = ...; // Initialize and update it appropriately
                output = (1 + (float)Math.Sin(input * mapFunc_shape)) * 0.5f;
                break;
        }

        if (float.IsNaN(output)) output = 0;
        output = Math.Clamp(output, 0.0f, 1.0f);
        return output;
    }

    // Preprocessor output is processed in every callback using this function
    public void Process(float input)
    {
        if (isInitialized)
        {
            double envelopeValue = envFol.GetEnvelope(input);
            outputVal = ApplyMapFunc((float)envelopeValue);
            if (isInverted) outputVal = 1 - outputVal;
        }
    }
}


public class MovementFeature
{
    public float minVal = 0;
    public float maxVal = 0;
    public string mpName = "PLACEHOLDER";
    public float value = 0;

    public MovementFeature(string name, float mini, float maxi)
    {
        mpName = name;
        minVal = mini;
        maxVal = maxi;
    }

    public void storeValue(float newVal)
    {
       // if (!double.IsNaN(newVal)) // Check for NaN
       // {
            value = Math.Min(newVal, maxVal);
            value = Math.Max(value, minVal);
       /* }
       // else
        {
            newVal = minVal;
        }
       */
    }

    public float getValue()
    {
        return value;
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
   

