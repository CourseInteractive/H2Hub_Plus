using UnityEngine;

namespace Course.PrototypeScripting
{
    public class ConversationUI_OptionButton : MonoBehaviour
    {
        public UnityEngine.UI.Text ui_label;
        Conversation.ConversationOption option;
        public UnityEngine.UI.Button btn;
        // Start is called before the first frame update
        public void Initialize(Conversation.ConversationOption _option)
        {
            option = _option;
            ui_label.text = option.text;
       
        }

        public void Execute()
        {
            //ConversationUI.Instance.Hide();
            ConversationUI.Instance.ExecuteOption(option);
        }

    }
}
