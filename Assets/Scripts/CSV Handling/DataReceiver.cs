using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;
using UnityEngine.Android;
using UnityEngine.Networking;
using System.IO;

public class DataReceiver: MonoBehaviour
{    
    [Header("OSC Settings")]  

    public OSCReceiver receiver;     // Define the OSC receiver and the OSC address
    public string Address = "/S"; // Define the OSC address

    public DataProcessorAHRS dataProcessor;
    

    public string sensorName;
    
    private bool isChosen = false;


     private void Start()
    {
        // Bind the OSC receiver to the address
        receiver.Bind(Address, MessageReceived);
  
    }
     public void whenChosen(){
        // Create the CSV file
      

        isChosen = true;

        // Initialize last write time
      

        // Start the coroutine to write to the CSV file every frame
        

        Debug.Log("Chosen" + Address);
    }

    private void OnDestroy()
    {
        // Stop the coroutine when the object is destroyed
      
    }

    // Receive the OSC message
    public void MessageReceived(OSCMessage message)
    {     
        if (isChosen == true)
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
        

        dataProcessor.ProcessData(sensorName, formattedMessage);
        }
    }

   
}
