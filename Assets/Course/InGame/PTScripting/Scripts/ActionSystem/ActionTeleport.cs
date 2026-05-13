using UnityEngine;

namespace Course.PrototypeScripting
{
    public class ActionTeleport : Action
    {
        // public bool objIsPlayer;
        public GameObject objectToTeleport;
        public GameObject teleportPoint;
        public bool copyRotation;

        override public void ExecuteAction()
        {

            objectToTeleport.transform.position = teleportPoint.transform.position;
            if (copyRotation)
                objectToTeleport.transform.eulerAngles = new Vector3(objectToTeleport.transform.eulerAngles.x, teleportPoint.transform.eulerAngles.y, objectToTeleport.transform.eulerAngles.z);
            GoOn();              
        }

        void GoOn()
        {
            ReportActionEnd();
        }

        // Optionale Funktion zur Darstellung in der Sequenz
        override public string GetAdditionalInfo()
        {
            if (teleportPoint == null || objectToTeleport == null)
                return "! Objekt oder Teleport-Ziel fehlt";
            return "Teleport " + objectToTeleport.name + " to " + teleportPoint.name; // Rückgabe von Werten als String, um diese in der Sequenz anzuzeigen
        }
    }
}
