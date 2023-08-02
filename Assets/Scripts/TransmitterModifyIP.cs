using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;
using TMPro;

public class TransmitterModifyIP : MonoBehaviour
{
  // Creating a transmitter.

  public TMP_InputField inputFieldRemoteIP;
  public int Port;

  public void IPModify()
  {
  var transmitter = gameObject.AddComponent<OSCTransmitter>();

 // Set remote host address.
 transmitter.RemoteHost = inputFieldRemoteIP.text.Trim();   

 // Set remote port;
 transmitter.RemotePort = Port; 


 var message = new OSCMessage("/message/address");

// Populate values.
   message.AddValue(OSCValue.String("Hello, world!"));
   message.AddValue(OSCValue.Float(1337f));

// Send message
   transmitter.Send(message);
  }

}
