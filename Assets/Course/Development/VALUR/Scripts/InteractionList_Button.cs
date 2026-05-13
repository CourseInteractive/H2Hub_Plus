using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VALUR
{
    public class InteractionList_Button : MonoBehaviour
    {

        public InteractionList main;
        public UnityEngine.UI.Text text;
        int index;

        public void Init(int _index, InteractionList _main, string content)
        {
            index = _index;
            main = _main;
            text.text = content;
            gameObject.SetActive(true);
        }

        public void GotClicked()
        {
            main.ReportClick(index);
        }
    }

}