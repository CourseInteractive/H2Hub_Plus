using UnityEngine;


namespace Course.PrototypeScripting
{
    public class ActionShowText_Advanced : Action
    {
        public string nameToShow;
        public string textToShow;
        public float time = 4f;
        bool running;
        bool wasSkipped = false;
        public bool skippable = true;
        public enum Speaker { Player, Other, ExecutingObject}
        public Speaker speaker;
        public GameObject speakerObject;

        override public void ExecuteAction()
        {
            /*if (speaker == Speaker.Player)
                speakerObject = CaptainTeague.player.gameObject;
            else */if (speaker == Speaker.ExecutingObject)
                speakerObject = GetExecutingObject();

            TextPanel.Instance.SetText(nameToShow, textToShow, speakerObject);
            Invoke("GoOn", time);
            wasSkipped = false;
            running = true;
            enabled = true;
        }

        private void Awake()
        {
            enabled = false;
        }

        private void Update()
        {
            if (skippable && UnityEngine.Input.anyKeyDown && running)
                Skip();
        }

        void Skip()
        {
            GoOn();
        }

        void GoOn()
        {
            if (wasSkipped)
                return;
            enabled = false;
            running = false;
            wasSkipped = true;
            TextPanel.Instance.Hide();
            ReportActionEnd();
        }

        override public string GetAdditionalInfo()
        {
            return nameToShow + ": " + textToShow;
        }
    }
}
