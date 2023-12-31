﻿using UnityEngine;
using System.Collections;
using TMPro;
using extOSC;
using UnityEngine.UI;
using System;

public class ConnectionCheck : MonoBehaviour
{
    [Header("OSC Settings")]
    public string Address = "/S2"; // Address to listen
    public OSCReceiver Receiver; // Receiver component

    [Header("UI Settings")]
    public TextMeshProUGUI ConnictionText; // Receiver component
    private bool sensorTurnedOn = true; // Flag to track if the sensor is turned on
    private DateTime lastMessageReceivedTime; // Store the timestamp of the last received message
    public float sensorTurnedOffTimeout = 0.5f; // Time in seconds to consider the sensor as turned off

   
    protected virtual void Start()
    {
        Receiver.Bind(Address, ReceivedMessage); // Bind address
    }

    // updates count and closes the receiver
    private void Update()
    {
        // Check if the sensor is turned off and no messages have been received for a specified time
        if (sensorTurnedOn && (DateTime.Now - lastMessageReceivedTime).TotalSeconds >= sensorTurnedOffTimeout)
        {
            sensorTurnedOn = false; // Sensor is turned off
            ConnictionText.text = "Ikke tilsluttet\n Tænd Sensoren \n og Tryk Pair"; // updates text
            ConnictionText.color = Color.red;
           
        }
        else if (!sensorTurnedOn)
        {
            ConnictionText.text = "Ikke tilsluttet \n Tryk Pair på Sensoren"; // updates text
            ConnictionText.color = Color.red;
            
        }
        else
        {
            // Sensor is turned on or a message has been received recently
            ConnictionText.text = "Tilsluttet"; // updates button text
            ConnictionText.color = Color.green;
           
        }
    }

    // Invokes when a message is received
    private void ReceivedMessage(OSCMessage message)
    {
        // Update the flag to indicate that the sensor is turned on
        sensorTurnedOn = true;
        lastMessageReceivedTime = DateTime.Now; // Update the timestamp of the last received message
    }

   

}
