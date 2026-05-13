using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Course.PrototypeScripting
{
    public class ActionGameState : Action
    {
        public RuntimeGlobal.GameState gameState;

        override public void ExecuteAction()
        {
            RuntimeGlobal.SwitchGameState(gameState);
            RuntimeGlobal.ClearSelection();
            ReportActionEnd();
        }

        override public string GetAdditionalInfo()
        {

            return gameState.ToString();
        }

    }
}