using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static DataProcessorAHRS;
using System.Collections;
using System.Threading.Tasks;
using extOSC;

public class SliderValueSender : MonoBehaviour
{
    
    [Header("OSC Settings")]
    public OSCTransmitter Transmitter; // Define the OSC transmitter
    public string Address = "/slider"; // Define the OSC address for the slider

    public TMP_InputField ipAddressInput;

    [Header("UI Settings")]
    public Slider slider; // Define the slider
    public TextMeshProUGUI sliderValueText; // Display the slider value

    private bool isConnected = false; // Define the connection status
    private bool hasSliderMoved = false;

    // Listen for the start event and start the coroutine
    private async void Start()
    {
        slider.onValueChanged.AddListener(SendSliderValue);
        
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
            if (isConnected && hasSliderMoved)
            {
                SendSliderValue(slider.value);
            }
        }
    }

    // Function for sending the slider value as an OSC message
    public void SendSliderValue(float value)
    {
        hasSliderMoved = true;

        if (!isConnected)
        {
            Connect();
        }

        string formattedValue = value.ToString("F2");
        var message = new OSCMessage(Address);
        message.AddValue(OSCValue.Float(float.Parse(formattedValue)));
        Transmitter.Send(message);
      
        // Update the displayed slider value
        sliderValueText.text = value.ToString("F2"); // Format the value to display with 2 decimal places
    }

    // Connect to the OSC receiver
    private void Connect()
    {
        string ipAddress = ipAddressInput.text; // Get the IP address from the input field
        Transmitter.RemoteHost = ipAddress; // Set the OSC transmitter's IP address
        Transmitter.Connect();
        isConnected = true;
    }
}