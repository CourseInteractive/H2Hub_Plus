using UnityEngine;
using Course.PrototypeScripting;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class InteractableObject : SelectableObject
{
    public string label;
    public bool deactivateInteraction;

    public Sequence seqOnInteraction;
    public bool fromEverywhere;
    public GameObject onHover;
    private XRSimpleInteractable comp;
    private XRGrabInteractable grabComp;

    private void Awake()
    {
        if(!enabled)
        {
            deactivateInteraction = true;
            enabled = true;
        }
    }

    private void Start()
    {
        HoverEnd();
        grabComp = GetComponent<XRGrabInteractable>();
        comp = GetComponent<XRSimpleInteractable>();
       // XRSimpleInteractable interact = CopyComponent<XRSimpleInteractable>(VR_InteractionConnector.Instance.simpleInteraction, gameObject);
        comp.activated.AddListener(ExecuteInteraction);
        comp.hoverEntered.AddListener(VR_HoverEnter);
        comp.hoverExited.AddListener(VR_HoverExit);
     /*   if (grabComp)
        {
            grabComp.hoverEntered.AddListener(VR_HoverEnter);
            grabComp.hoverExited.AddListener(VR_HoverExit);
        }
        else
        if (comp == null)
        {
            XRSimpleInteractable interact = CopyComponent<XRSimpleInteractable>(VR_InteractionConnector.Instance.simpleInteraction, gameObject);
            interact.activated.AddListener(ExecuteInteraction);
            interact.hoverEntered.AddListener(VR_HoverEnter);
            interact.hoverExited.AddListener(VR_HoverExit);
        }*/
        if (deactivateInteraction)
            enabled = false;
    }

    public void ExecuteInteraction(ActivateEventArgs args)
    {
        Execute();
    }

    public void VR_HoverEnter(HoverEnterEventArgs args)
    {
        HoverOver();
    }

    public void VR_HoverExit(HoverExitEventArgs args)
    {
        HoverEnd();
    }

    [ContextMenu("Hover")]
    public void HoverOver()
    {
        if (onHover)
            onHover.SetActive(true);
        if (!fromEverywhere)
            RuntimeGlobal.Select(this);
    }
    

    public void HoverEnd()
    {
        if (onHover)
            onHover.SetActive(false);
        RuntimeGlobal.ClearSelection();
    }

    #if UNITY_EDITOR
    [ContextMenu("Execute")]
    public void ExecuteFromMenu()
    {
        Execute();
    }
    #endif
    public void Execute()
    {
        if(fromEverywhere)
        {
            // Teleport
            seqOnInteraction.ExecuteCompleteSequence();
            return;
        }


       // bool toolSelected = InventoryManager.Instance.itemSelected;
        InventoryCombination combi = GetComponent<InventoryCombination>();
        bool combiPresent = combi != null;
        bool combiWithThisTool = false;
        string toolNeeded = "";
      /*  if(combiPresent)
        {

            toolNeeded = combi.invItemName;
            if (toolSelected && combi.invItemName == InventoryManager.Instance.selectedItem.name)
                combiWithThisTool = true;
        }*/

       /* if (toolSelected)
        {
            
            if (combiPresent)
            {
                if(combiWithThisTool)
                {
                    InteractionCondition condition = GetComponent<InteractionCondition>();
                    if (condition)
                    {
                        if (condition.CheckIfConditionMet())
                            combi.sequenceOnCombination.ExecuteCompleteSequence();
                        else
                            InterfaceManager.Instance.ShowHelp("Nicht möglich");
                    }
                    else
                        combi.sequenceOnCombination.ExecuteCompleteSequence();

                }
                else
                {
                    InterfaceManager.Instance.ShowHelp("Brauche ein anderes Werkzeug: " + toolNeeded);
                }
            }
            else
            {
                InterfaceManager.Instance.ShowHelp("Werkzeug weglegen.");
            }
                


        }
        else
        {*/
           /* if(combiPresent)
            {
              //  InterfaceManager.Instance.ShowHelp("Brauche ein Werkzeug: " + toolNeeded);
                return;
            }*/


            /*InteractionCondition condition = GetComponent<InteractionCondition>();
            if(condition)
            {
                if (condition.CheckIfConditionMet())
                    seqOnInteraction.ExecuteCompleteSequence();
                else
                    InterfaceManager.Instance.ShowHelp("Nicht möglich");
            }
            else
                seqOnInteraction.ExecuteCompleteSequence();*/


       // }
            
    }

    T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy as T;
    }
}
