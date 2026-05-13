using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Course.PrototypeScripting
{
    public class Conversation : MonoBehaviour
    {
        
        [System.Serializable]
        public class ConversationOption
        {
            public string name = "-";
            public string text = "-";
            public bool enabled = true;
            public ConversationOptionCondition condition;
            public bool endConversationAfterwards = false;
            public Sequence sequence;
            public bool open = false;
        }

        


        public Sequence welcomeSequence;

        public ConversationOption[] options;

        public void Init()
        {
            options = new ConversationOption[1];
            options[0] = new ConversationOption();
        }
        public void SetOptionState(int index, bool value)
        {
            options[index].enabled = value;
        }

        public List<ConversationOption> GetEnabledOptions()
        {
            List<ConversationOption> list = new List<ConversationOption>();
            foreach (ConversationOption option in options)
            {
                if (option.enabled && option.condition.ConditionIsMet())
                    list.Add(option);
            }
            return list;
        }
        public string[] GetOptionNames()
        {
            List<string> list = new List<string>();
            foreach (ConversationOption option in options)
            {
                list.Add(option.name);
            }
            return list.ToArray();
        }
    }

    public enum OptionConditionType { None, GlobalVar, LocalVar, Inventory }


    [System.Serializable]
    public class ConversationOptionCondition
    {
        public enum Comparison { Equal, Greater, GreaterOrEqual, Less, LessOrEqual }

        public OptionConditionType conditionType;
        public string conditionSetting;
        public Comparison conditionQuestion;
        public int conditionValue;

        public bool ConditionIsMet()
        {
            int variableContent = 0;
            switch(conditionType)
            {
                case OptionConditionType.None:
                    return true;
                case OptionConditionType.GlobalVar:
                    variableContent = VariableManager.Instance.GetVariable(conditionSetting);
                    break;
                case OptionConditionType.LocalVar:
                    variableContent = VariableManager.Instance.GetLocalVariable(conditionSetting);
                    break;
                case OptionConditionType.Inventory:
                    variableContent = InventoryManager.Instance.GetAmount(conditionSetting);
                    break;
                /*case OptionConditionType.Inventory_WTWC:
                    variableContent = Inventory.instance.GetAmountOfItem(int.Parse(conditionSetting));
                    break;*/
            }



            switch (conditionQuestion)
            {
                case Comparison.Equal:
                    return(variableContent == conditionValue);
                case Comparison.Greater:
                    return (variableContent > conditionValue);
                case Comparison.GreaterOrEqual:
                    return (variableContent >= conditionValue);
                case Comparison.Less:
                    return (variableContent < conditionValue);
                case Comparison.LessOrEqual:
                    return (variableContent <= conditionValue);

            }

            return true;
        }

    }


#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ConversationOptionCondition))]
    public class ConversationOptionConditionDrawer : PropertyDrawer
    {
    
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Begin block layout for nested properties
            EditorGUI.BeginProperty(position, label, property);

            // Start a vertical layout block (handles its own spacing)
            EditorGUILayout.BeginVertical(GUI.skin.box);

            // Draw label as foldout (optional)
            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, label, true);
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                SerializedProperty typeProp = property.FindPropertyRelative("conditionType");
                SerializedProperty settingProp = property.FindPropertyRelative("conditionSetting");
                SerializedProperty questionProp = property.FindPropertyRelative("conditionQuestion");
                SerializedProperty valueProp = property.FindPropertyRelative("conditionValue");

                EditorGUILayout.PropertyField(typeProp, new GUIContent("Condition Type"));

                // Only show other fields if type != None
                if ((OptionConditionType)typeProp.enumValueIndex != OptionConditionType.None)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(settingProp, GUIContent.none);
                    EditorGUILayout.PropertyField(questionProp, GUIContent.none, GUILayout.Width(120));
                    EditorGUILayout.PropertyField(valueProp, GUIContent.none, GUILayout.Width(60));
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
            EditorGUI.EndProperty();
        }
    }
#endif

}




