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

    // Debug text Elements
   

   



    //Declare movement features array
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
        
        // Call the UpdateData function every 0.033 seconds
        InvokeRepeating("UpdateData", 0.01f, 0.01f);
    }

    public void MainCallback()
    {

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
      
        
    }

    public void SensorData2() 
    {
        float gyroZSensor2 = dataProcessorAHRS.gyroZSensor2;
        float rollSensor2  = dataProcessorAHRS.GetPitchSensor2();
        float pitchsensor2 = dataProcessorAHRS.GetRollSensor2();
       

       
    }
    public void SensorData3()
    {
        float gyroZSensor3 = dataProcessorAHRS.gyroZSensor3;
        float rollSensor3 = dataProcessorAHRS.GetPitchSensor3();
        float pitchsensor3 = dataProcessorAHRS.GetRollSensor3();
      
      
    }

    public void SensorData4()
    {

        float gyroZSensor4 = dataProcessorAHRS.gyroZSensor4;
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

    }



    public void ComputeMovFeatures(float gyrZ_Thigh_L, float gyrZ_Thigh_R, float gyrZ_Shank_L, float gyrZ_Shank_R,
     float pitchDeg_Thigh_L, float pitchDeg_Thigh_R, float pitchDeg_Shank_L, float pitchDeg_Shank_R)
    {
        float angVel_Thigh_L = gyrZ_Thigh_L;
        float angVel_Thigh_R = gyrZ_Thigh_R;
        float angVel_Shank_L = gyrZ_Shank_L;
        float angVel_Shank_R = gyrZ_Shank_R;
        float kneeAng_L = pitchDeg_Shank_L - pitchDeg_Thigh_L;
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
        movementFeatures[6].storeValue(footPos_L - footPos_R);
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
