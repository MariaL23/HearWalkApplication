using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using extOSC;
using TMPro;

public class SensorDataHandler : MonoBehaviour
{
    #region Public Vars

		public string Address = "/example/1";
        public TextMeshProUGUI messageText;
        public TextMeshProUGUI countText;

        public int count;

		[Header("OSC Settings")]
		public OSCReceiver Receiver;

		#endregion

		#region Unity Methods

		protected virtual void Start()
		{
			Receiver.Bind(Address, ReceivedMessage);
		}

		#endregion

		#region Private Methods

		private void ReceivedMessage(OSCMessage message)
		{   
            count++;

            countText.text = count.ToString();
            messageText.text = message.ToString();
           

			
			Debug.Log(message);
			//Debug.LogFormat("Received: {0}", message);
		}

		#endregion
}
