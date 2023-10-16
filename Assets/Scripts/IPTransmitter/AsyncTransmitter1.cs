using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Threading.Tasks;
using extOSC;
public class AsyncTransmitter1 : MonoBehaviour
{
[Header("OSC Settings")]       
public OSCTransmitter Transmitter; // Define the OSC transmitter
public string Address = "/address2"; // Define the OSC address

[Header("UI Settings")] 
public Button connectButton; // Define the connect button
private bool isConnected = false; // Define the connection status

// Listen for the start event  and start the coroutine
  private async void Start()
   {
    //connectButton.onClick.AddListener(ToggleConnection);
    await SendOSCMessageRepeatedly();
        Connect();
        isConnected = true;
   }

    private void Update()
    {

        SendOSCMessage();
    }

   // Send OSC message repeatedly 
  private async Task SendOSCMessageRepeatedly()
 {
   while (true)
  {
    // Wait for 1 millisecond
    await Task.Delay(1);
    // Check if the connection is established
    if (isConnected)
     {
      SendOSCMessage();
    }
  }
 }
 // Function for the message that has to be send
  private void SendOSCMessage()
  {
     var message = new OSCMessage(Address);
     // Define message that has to be send
     message.AddValue(OSCValue.String("0"));
     
      // Send the message
     Transmitter.Send(message);
  }
  
    // Toggle connection 
    public void ToggleConnection()
   {
     if (isConnected) // If the connection is established
      {
          //Transmitter.Close(); // Close the connection

      }
     else
     {
        Connect(); // Connect to the OSC receiver
     }
    }
    // Connect to the OSC receiver
    private void Connect()
    {
      // Connect to the OSC receiver
      Transmitter.Connect();

       // Update connection status
       isConnected = true;
       
    }

    }

