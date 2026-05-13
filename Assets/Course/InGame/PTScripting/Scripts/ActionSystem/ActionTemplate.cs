using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Course.PrototypeScripting
{
    public class ActionTemplate : Action
    {
        // Eigene Variablen und Parameter eintragen.

        override public void ExecuteAction()
        {
            /***
             * Die Befehle die ausgeführt werden sollen.
             ***/


            // Invoke("GoOn", 5.5);         // Dem System in 5,5 Sekunden melden, dass die Aktion abgeschlossen ist. Kann für Verzögerungen genutzt werden.
            GoOn();                         // Dem System sofort melden, dass die Aktion abgeschlossen ist. 
        }

        void GoOn()
        {
            ReportActionEnd();
        }

        // Optionale Funktion zur Darstellung in der Sequenz
        override public string GetAdditionalInfo()
        {
            return "Hallo"; // Rückgabe von Werten als String, um diese in der Sequenz anzuzeigen
        }
    }
}