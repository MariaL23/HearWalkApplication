using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static DataProcessorAHRS;
using System.Collections;

using extOSC;
public class RTransmitter : MonoBehaviour
{
 public DataProcessorAHRS dataProcessorAHRS;
 [Header("OSC Settings")]       
public OSCTransmitter Transmitter; // Define the OSC transmitter
public string Address = "/address2"; // Define the OSC address

[Header("UI Settings")] 
public Button connectButton; // Define the connect button


public TMP_InputField ipAddressInput;

public TextMeshProUGUI HostText;
public TextMeshProUGUI statusText;
private bool isConnected = false; // Define the connection status
private string messageContent;


    // Listen for the start event  and start the coroutine
    public void Start()
   {

        ipAddressInput.onEndEdit.AddListener(delegate { OnIpAddressEntered(); });
        if (!string.IsNullOrEmpty(ipAddressInput.text))
        {
            OnIpAddressEntered();
        }

        SendOSCMessageRepeatedly();
 
    }

   // Send OSC message repeatedly 
public void SendOSCMessageRepeatedly()
 {
        // Check if the connection is established
        if (isConnected && !string.IsNullOrEmpty(messageContent))
            {
            statusText.text = "Sending";
            SendOSCMessage(messageContent);
               


        }
        else
            {
            
         }
        
 }
    // Function for the message that has to be send
    public void SendOSCMessage(string messageContent)
    {
        var message = new OSCMessage(Address);
        message.AddValue(OSCValue.String(messageContent));
        Transmitter.Send(message);
       
    }

    // Toggle connection 


    // Connect to the OSC receiver
    public void OnIpAddressEntered()
    {
        string ipAddress = ipAddressInput.text; // Get the IP address from the input field
        Transmitter.RemoteHost = ipAddress; // Set the OSC transmitter's IP address
        HostText.text = "Receiver IP: " + Transmitter.RemoteHost; // Set the IP address text

        // Connect to the OSC receiver
        Transmitter.Connect();
       // ipAddressInput.interactable = false; // Disable input after connecting
        isConnected = true; // Update connection status
    }


   



}

