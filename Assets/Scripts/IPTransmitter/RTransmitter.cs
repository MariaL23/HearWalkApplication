using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static DataProcessorAHRS;
using System.Collections;
using System.Threading.Tasks;
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
private bool isConnected = false; // Define the connection status
    public string messageContent;

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
      SendOSCMessage(messageContent);
    }
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
    public void ToggleConnection()
   {
     if (isConnected) // If the connection is established
      {
          Transmitter.Close(); // Close the connection

      }
     else
     {
        Connect(); // Connect to the OSC receiver
     }
    }
    // Connect to the OSC receiver
    private void Connect()
    {
      string ipAddress = ipAddressInput.text; // Get the IP address from the input field
      Transmitter.RemoteHost = ipAddress; // Set the OSC transmitter's IP address
      HostText.text = "Reciver ip: " + Transmitter.RemoteHost; // Set the IP address text
      // Connect to the OSC receiver
      Transmitter.Connect();
      connectButton.GetComponent<Image>().color = Color.green; // Change the color of the connect button 
      

       // Update connection status
       isConnected = true;
       
    }

    }

