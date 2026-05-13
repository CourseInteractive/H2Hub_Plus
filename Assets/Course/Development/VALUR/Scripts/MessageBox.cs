using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*****
 * VALUR - MessageBox ## Version 0.5
 * Last Change: 25.10.2015
 * By: Oli
 * 
 * Nachrichten-Box der Console
 * Soll eine LogMessage angezeigt werden, wird sie  auf solch ein Panel gezogen und für eine bestimmte Zeot angezeigt
 *****/

namespace VALUR
{

public class VALUR_Message
    {
		public GameObject text;
		public float timer;

		public void Init(GameObject obj, string msg)
        {
			text = obj;
			text.GetComponent<UnityEngine.UI.Text>().text = msg;
			text.SetActive(true);
		}

		public bool CountDown()
        {
			timer -= Time.deltaTime;
			if (timer > 0)
				return true;
			return false;
        }
    }

public class MessageBox : MonoBehaviour 
{

		public GameObject textParent;

		// UI-Element, das den Text des Panels hält
		public UnityEngine.UI.Text text;
		public GameObject textPrefab;
		public List<VALUR_Message> messages;

		// ablaufender Timer -> bei 0 wird das Panel geschlossen
		private float timer;

        private void Awake()
        {
			messages = new List<VALUR_Message>();

		}

        // Zählt den Timer herunter, wenn aktiv
        public void Update()
		{
			if (messages.Count == 0)
				return;
			VALUR_Message msgToDelete = null;
			foreach(VALUR_Message msg in messages)
            {
				if (!msg.CountDown())
                {
					msgToDelete = msg;
				
				}
				
            }
			if(msgToDelete != null)
            {
				Delete(msgToDelete);
				if (messages.Count == 0)
					Close();
			}

		/*	if(timer > 0)
			{
				timer -= Time.deltaTime;
				if(timer < 0)
				{
					Close();
				}
			}*/
		}
		void Delete(VALUR_Message msg)
        {
			Destroy(msg.text);
			messages.Remove(msg);
        }

		// Zeigt die Nachricht an und startet den Timer
		public void ShowMessage(string message)
		{
			VALUR_Message mes = new VALUR_Message();
			mes.timer = 2f;
			GameObject newEntry = (GameObject)Instantiate(textPrefab, textParent.transform);
			mes.Init(newEntry, message);
			messages.Add(mes);
			gameObject.SetActive(true);
		}

		// Versteckt die Nachricht
		public void Close()
		{
			text.text = "";
			gameObject.SetActive(false);
		}
}

}