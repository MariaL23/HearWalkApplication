using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;
using UnityEngine.Android;
using UnityEngine.Networking;
using System.IO;
using TMPro;

public class DataReceiver: MonoBehaviour
{    
    [Header("OSC Settings")]  

    public OSCReceiver receiver;     // Define the OSC receiver and the OSC address
    public string Address = "/S"; // Define the OSC address

    public DataProcessorAHRS dataProcessor;

    private string bodyPartName; // Variable to store the body part name received from the DropdownHandler script
    
    public TextMeshProUGUI DisconnectText;
    public string sensorName;

     private Dictionary<string, float> lastMessage = new Dictionary<string, float>();
    
    private bool isChosen = false;


     private void Start()
    {
        // Bind the OSC receiver to the address
        receiver.Bind(Address, MessageReceived);
  
    }

    private void Update(){
        //CheckForInactivity();
        }
     public void whenChosen(){

        isChosen = true;
        
        Debug.Log("Chosen" + Address);
    }

    private void OnDestroy()
    {
        // Stop the coroutine when the object is destroyed
      
    }

       public void SetBodyPartName(string name)
    {
        bodyPartName = name; // Set the body part name received from the DropdownController script
    }

    // Receive the OSC message
    public void MessageReceived(OSCMessage message)
    {    
        if (!lastMessage.ContainsKey(sensorName))
        {
            lastMessage.Add(sensorName, Time.time);
        }
        else
        {
            lastMessage[sensorName] = Time.time;
        }

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

       private void CheckForInactivity()
    {
        foreach (var kvp in lastMessage)
        {
            string sensorName = kvp.Key;
            float lastMessageTime = kvp.Value;

            // Check if no messages have been received for this sensor for more than 1 second
            if (Time.time - lastMessageTime >= 1f)
            {
                DisconnectText.text = sensorName +  " Disconnected";
                Debug.Log($"No OSC messages received from {sensorName} in the last second.");
            }
        }
    }

   
}
