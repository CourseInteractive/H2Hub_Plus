
using UnityEngine;

namespace Course.PrototypeScripting
{
    public class ActionConversation : Action
    {
        public Conversation conversation;

        public enum Type { Start, SetOption }
        public Type type;

        public int index;
        public bool enableValue;

        public override void ExecuteAction()
        {
            if (conversation == null)
            {
                Debug.LogError("No Conversation set!");
                return;
            }

            switch (type)
            {
                case Type.Start:
                    ConversationUI.Instance.StartConversation(conversation);
                    break;
                case Type.SetOption:
                    conversation.SetOptionState(index, enableValue);
                    ReportActionEnd();
                    break;
            }


        }

        public override string GetAdditionalInfo()
        {
            if (conversation == null)
                return "- No Conversation set!";
            switch (type)
            {
                case Type.Start:
                    return "Start " + conversation.name;
                case Type.SetOption:
                    return conversation.name + ":Set " + index + " => " + enableValue;
            }
            return "";


        }
    }
}
