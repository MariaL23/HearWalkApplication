using UnityEngine;
using System.Collections;
using TMPro;
using extOSC;
using UnityEngine.UI;


	public class ConnectionCheck : MonoBehaviour
	{
		[Header("OSC Settings")]
		public string Address = "/S2"; // Address to listen
		public OSCReceiver Receiver; // Receiver component

        [Header("UI Settings")] 
		public int count; // Receiver component
		public TextMeshProUGUI ConnictionText; // Receiver component

		

		protected virtual void Start()
		{
			Receiver.Bind(Address, ReceivedMessage); // Bind address
		}

	
       // updates count and closes the receiver
	   private void Update() {
		   if (count > 0) {
			   ConnictionText.text = "Connected"; //updates button text
			   ConnictionText.color = Color.green;
			   Receiver.Close();
		   }
	   }


        // Invokes when message received and add 1 to count
		private void ReceivedMessage(OSCMessage message)
		{ 
			count++;
			Debug.Log(message);
			;
		}

	
	}
