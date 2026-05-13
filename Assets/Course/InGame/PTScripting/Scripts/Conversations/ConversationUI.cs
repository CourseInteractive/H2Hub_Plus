using System.Collections.Generic;
using UnityEngine;

namespace Course.PrototypeScripting
{
    public class ConversationUI : MonoBehaviour
    {

        public GameObject listPanel;
        public GameObject optionReference;

        Conversation currentConversation;
        List<GameObject> currentOptions;

        public static ConversationUI _instance;
        public static ConversationUI Instance
        {
            get
            {
                if (_instance == null)
                    throw new System.Exception("No ConversationUI in the scene. Import the prefab ConversationUI from the project folder.");
                return _instance;
            }
            private set { _instance = value; }
        }

        void Awake()
        {
            if (_instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            optionReference.SetActive(false);

        }

        public void StartConversation(Conversation conv)
        {
            currentConversation = conv;
            if(currentConversation.welcomeSequence != null)
                SequenceHandler.Instance.StartNewSequence(currentConversation.welcomeSequence, Show);
            else
                Show();
        }

        public void Show()
        {

            ShowUI();

        }

        public void ShowUI()
        {
            DeleteEntries();
            List<Conversation.ConversationOption> options = currentConversation.GetEnabledOptions();
            if (options.Count == 0)
                return;
            foreach (Conversation.ConversationOption option in options)
            {
                GameObject go = CreateOptionUI(option);
                currentOptions.Add(go);
            }
            AdjustListPanelToOptionList();
            gameObject.SetActive(true);
            //RuntimeGlobal.SwitchGameState(RuntimeGlobal.GameState.Conversation);
            //Show();
            currentOptions[0].GetComponent<ConversationUI_OptionButton>().btn.Select();
        }

        public void EndConversation()
        {
            DeleteEntries();
            //RuntimeGlobal.SwitchGameState(RuntimeGlobal.GameState.NormalGame);
        }

        private GameObject CreateOptionUI(Conversation.ConversationOption option)
        {
            GameObject go = (GameObject)Instantiate(optionReference);
            ConversationUI_OptionButton _button = go.GetComponent<ConversationUI_OptionButton>();
            _button.Initialize(option);
            go.transform.parent = listPanel.transform;
            go.SetActive(true);
            return go;
        }

        void DeleteEntries()
        {
            if (currentOptions != null && currentOptions.Count > 0)
            {
                foreach (GameObject go in currentOptions)
                {
                    DestroyImmediate(go);
                }
            }
            currentOptions = new List<GameObject>();

        }

        public void ExecuteOption(Conversation.ConversationOption option)
        {
            if (option.endConversationAfterwards)
                SequenceHandler.Instance.StartNewSequence(option.sequence, EndConversation);
            else
                SequenceHandler.Instance.StartNewSequence(option.sequence, ShowUI);
        
        }

        void AdjustListPanelToOptionList()
        {
            if (currentOptions == null || currentOptions.Count == 0)
                return;
            float height = currentOptions[0].GetComponent<RectTransform>().rect.height;
            listPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height * currentOptions.Count);
        }


    }
}
