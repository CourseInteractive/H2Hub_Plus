using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace Course.PrototypeScripting
{
    public class ActionNavigationArea : Action
    {
        public NavMeshAgent navAgent;
        public int areaIndex;
        public bool walkable;

        override public void ExecuteAction()
        {
            if (walkable)
                navAgent.areaMask = navAgent.areaMask | (1 << areaIndex);
            else
                navAgent.areaMask = navAgent.areaMask ^ (1 << areaIndex);
        }

        override public string GetAdditionalInfo()
        {
            if (navAgent == null)
                return "- No Nav Agent set!";
            if (walkable)
                return navAgent.name + " walks on " + areaIndex;
            else
                return navAgent.name + " can not walk on " + areaIndex;

        }
    }
}