using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Threading.Tasks;
using extOSC;
public class AsyncTransmitter : MonoBehaviour
{
[Header("OSC Settings")]       
public OSCTransmitter Transmitter; // Define the OSC transmitter
public string Address = "/address2"; // Define the OSC address
public string ReceiverIP = "192.168.0.212"; // Define the IP address of the OSC receiver
public int ReceiverPort = 9000; // Define the port of the OSC receiver

[Header("UI Settings")] 
public Button connectButton; // Define the connect button
private bool isConnected = false; // Define the connection status

// Listen for the start event  and start the coroutine
  private async void Start()
   {
    connectButton.onClick.AddListener(ToggleConnection);
    await SendOSCMessageRepeatedly(); 
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
     message.AddValue(OSCValue.String("192.168.0.212"));
     
      // Send the message
     Transmitter.Send(message);
  }
    // Toggle connection 
    public void ToggleConnection()
   {
     if (isConnected) // If the connection is established
      {
              
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
      Transmitter.RemoteHost = ReceiverIP;
      Transmitter.RemotePort = ReceiverPort;
      Transmitter.Connect();

       // Update connection status
       isConnected = true;
       
    }

    }

