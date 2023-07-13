using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Threading.Tasks;
using extOSC;


public class AsyncTransmitter : MonoBehaviour
{

[Header("OSC Settings")]       

public OSCTransmitter Transmitter;
public string Address = "/address2";
public string ReceiverIP = "192.168.0.212";
public int ReceiverPort = 9000;

[Header("UI Settings")] 
public Button connectButton;
//public TextMeshProUGUI connectButtonText;

//public TextMeshProUGUI text;
// public TextMeshProUGUI sendText;
//private int count;
private bool isConnected = false;
  private async void Start()
   {
    connectButton.onClick.AddListener(ToggleConnection);
    await SendOSCMessageRepeatedly(); 
   }
  private async Task SendOSCMessageRepeatedly()
 {
   while (true)
  {
    await Task.Delay(1);

    if (isConnected)
     {
      SendOSCMessage();
    }
  }
 }
  private void SendOSCMessage()
  {
     var message = new OSCMessage(Address);
     message.AddValue(OSCValue.String("192.168.0.212"));

     Transmitter.Send(message);
     //sendText.text = message.ToString();
     //count++;
     //text.text =  Transmitter.RemoteHost + "  " + Transmitter.RemotePort.ToString();
  }
    public void ToggleConnection()
   {
     if (isConnected)
      {
              
      }
     else
     {
        Connect();
     }
    }

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

