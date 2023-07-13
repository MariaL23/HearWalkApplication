using UnityEngine;
using System.Collections;
using TMPro;
using extOSC;
using UnityEngine.UI;


	public class ConnectionCheck : MonoBehaviour
	{
		

		public string Address = "/S2";

		[Header("OSC Settings")]
		public OSCReceiver Receiver;

		public int count;

	
	
		public TextMeshProUGUI ConnictionText;

		

		protected virtual void Start()
		{
			Receiver.Bind(Address, ReceivedMessage);
		}

		

		#region Private Methods

	   private void Update() {
		   if (count > 0) {
			   ConnictionText.text = "Connected";
			   ConnictionText.color = Color.green;
			   Receiver.Close();
		   }
	   }



		private void ReceivedMessage(OSCMessage message)
		{ 
			count++;
			
			
			Debug.Log(message);
			//Debug.LogFormat("Received: {0}", message);
		}

		#endregion
	}
