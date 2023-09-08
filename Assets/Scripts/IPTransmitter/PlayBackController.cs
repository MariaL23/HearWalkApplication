using UnityEngine;
using UnityEngine.UI;
using TMPro;

using System.Collections;
using System.Threading.Tasks;
using extOSC;
public class PlayBackController : MonoBehaviour
{
    
    [Header("OSC Settings")]
    public OSCTransmitter Transmitter; // Define the OSC transmitter
    public string Address = "/address2"; // Define the OSC address

    [Header("UI Settings")]
    public Button connectButton; // Define the connect button
    public TMP_InputField ipAddressInput;
    public TextMeshProUGUI HostText;

    private bool isConnected = false; // Define the connection status
    private bool isSendingOne = false; // Flag to track the message content

    // Listen for the start event and start the coroutine
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

    // Function for the message that has to be sent
    public void SendOSCMessage()
    {
        var message = new OSCMessage(Address);
        message.AddValue(OSCValue.Float(isSendingOne ? 1 : 0)); // Send "1" or "0" based on the flag
        Transmitter.Send(message);

        connectButton.GetComponent<Image>().color = isSendingOne ? Color.green : Color.red;
        connectButton.GetComponentInChildren<TextMeshProUGUI>().text = isSendingOne ? "On" : "Off";


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

        // Toggle the flag to switch between "1" and "0"
        isSendingOne = !isSendingOne;
      
    }

    // Connect to the OSC receiver
   private void Connect()
    {
        string ipAddress = ipAddressInput.text; // Get the IP address from the input field
        Transmitter.RemoteHost = ipAddress; // Set the OSC transmitter's IP address
        // Connect to the OSC receiver
        Transmitter.Connect();

        // Update connection status
        isConnected = true;
    }

}

