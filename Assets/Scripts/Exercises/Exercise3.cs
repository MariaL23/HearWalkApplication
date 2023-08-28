using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static DataProcessorAHRS;
using UnityEngine.UI;
using System.IO;
using System;


public class Exercise3 : MonoBehaviour
{

    public DataProcessorAHRS dataProcessorAHRS;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SensorData1()
    {
        float rollSensor1 = dataProcessorAHRS.GetPitchSensor1();
        float pitchsensor1 = dataProcessorAHRS.GetRollSensor1();
        float gyroZSensor1 = dataProcessorAHRS.gyroZSensor1;
    }

    public void SensorData2()
    {

        float rollSensor2 = dataProcessorAHRS.GetPitchSensor2();
        float pitchsensor2 = dataProcessorAHRS.GetRollSensor2();
        float gyroZSensor2 = dataProcessorAHRS.gyroZSensor2;


    }
    public void SensorData3()
    {

        float rollSensor3 = dataProcessorAHRS.GetPitchSensor3();
        float pitchsensor3 = dataProcessorAHRS.GetRollSensor3();
        float gyroZSensor3 = dataProcessorAHRS.gyroZSensor3;

    }

    public void SensorData4()
    {


        float rollSensor4 = dataProcessorAHRS.GetPitchSensor4();
        float pitchsensor4 = dataProcessorAHRS.GetRollSensor4();
        float gyroZSensor4 = dataProcessorAHRS.gyroZSensor4;


    }
}
