using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static DataProcessorAHRS;
using UnityEngine.UI;
using System.IO;
using System;

public class MainDataProcessor : MonoBehaviour
{
    
    public DataProcessorAHRS dataProcessorAHRS;
    public DropdownHandler dropdownHandler;

    public TextMeshProUGUI ShankLtext;
    public TextMeshProUGUI ShankRtext;
    public TextMeshProUGUI ThighLtext;
    public TextMeshProUGUI ThighRtext;

    public TextMeshProUGUI KneeLtext;
    public TextMeshProUGUI KneeRtext;
    public TextMeshProUGUI FootLvsRtext;

    public MovementFeature[] movementFeatures = new MovementFeature[7];
    public void Start()
    {
        
            movementFeatures[0] = new MovementFeature("Angular_Velocity_ThighL", -110, 110);
            movementFeatures[1] = new MovementFeature("Angular_Velocity_ThighR", -110, 110);
            movementFeatures[2] = new MovementFeature("Angular_Velocity_ShankL", -250, 250);
            movementFeatures[3] = new MovementFeature("Angular_Velocity_ShankR", -250, 250);
            movementFeatures[4] = new MovementFeature("Joint_Angle_KneeL", -10, 90);
            movementFeatures[5] = new MovementFeature("Joint_Angle_KneeR", -10, 90);
            movementFeatures[6] = new MovementFeature("Foot_Position_Lvs/R", -1, 1);
        
        // Call the UpdateData function every 0.33 seconds, starting after 0.33 seconds
        InvokeRepeating("UpdateData", 0.033f, 0.033f);
    }

    private void UpdateData()
    {
        SensorData1();
        SensorData2();  
        SensorData3();
        SensorData4();
        UpdateValue();
    }

    public void GetBodyPartName()
    {

       
    }

    public void SensorData1()
    {
        float rollSensor1 = dataProcessorAHRS.GetPitchSensor1();
        float pitchsensor1 = dataProcessorAHRS.GetRollSensor1(); 
    }

    public void SensorData2() 
    {

        float rollSensor2  = dataProcessorAHRS.GetPitchSensor2();
        float pitchsensor2 = dataProcessorAHRS.GetRollSensor2();
    }
    public void SensorData3()
    {

        float rollSensor3 = dataProcessorAHRS.GetPitchSensor3();
        float pitchsensor3 = dataProcessorAHRS.GetRollSensor3();
    }

    public void SensorData4()
    {
       

        float rollSensor4 = dataProcessorAHRS.GetPitchSensor4();
        float pitchsensor4 = dataProcessorAHRS.GetRollSensor4();    

        
    }

    public void UpdateSensorData(int sensorIndex)
    {
        switch (sensorIndex)
        {
            case 1:
                SensorData1();
                break;

            case 2:
                SensorData2();
                break;

            case 3:
                SensorData3();
                break;

            case 4:
                SensorData4();
                break;


            default:
                break;
        }
    }

    public void UpdateValue()
    {
        ShankLtext.text =  movementFeatures[2].getValue().ToString();
        ShankRtext.text = movementFeatures[3].getValue().ToString();
        ThighLtext.text = movementFeatures[0].getValue().ToString();
        ThighRtext.text = movementFeatures[1].getValue().ToString();
        KneeLtext.text = movementFeatures[4].getValue().ToString();
        KneeRtext.text = movementFeatures[5].getValue().ToString();
        FootLvsRtext.text = movementFeatures[6].getValue().ToString();

    }






    public class MovementFeature
    {
        public double minVal = 0;
        public double maxVal = 0;
        public string mpName = "PLACEHOLDER";
        public double value = 0;

        public MovementFeature(string name, double mini, double maxi)
        {
            mpName = name;
            minVal = mini;
            maxVal = maxi;
        }

        public void storeValue(double newVal)
        {
            if (!double.IsNaN(newVal)) // Check for NaN
            {
                value = Math.Min(newVal, maxVal);
                value = Math.Max(value, minVal);
            }
            else
            {
                newVal = minVal;
            }
        }

        public double getValue()
        {
            return value;
        }

    }

}
