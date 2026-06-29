using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SimpleTooltip : MonoBehaviour, I_TooltipProvider
{
    public string tooltip;

    void Awake()
    {
        XRGrabInteractable grabable = GetComponent<XRGrabInteractable>();
        if(grabable)
        {
            grabable.hoverEntered.AddListener(VR_HoverEnter);
            grabable.hoverExited.AddListener(VR_HoverExit);
        }
        XRBaseInteractable baseInteract = GetComponent<XRBaseInteractable>();
        if (baseInteract)
        {
            baseInteract.hoverEntered.AddListener(VR_HoverEnter);
            baseInteract.hoverExited.AddListener(VR_HoverExit);
        }
    }

    public void VR_HoverEnter(HoverEnterEventArgs args)
    {
        if (ControllerGrabSituationManager.instance.AnyControllerIsHoldingObject())
            return;
        
        ((I_TooltipProvider)this).ActivateTooltip(args.interactorObject, args.interactableObject);
    }

    public void VR_HoverExit(HoverExitEventArgs args)
    {
        ((I_TooltipProvider)this).ClearTooltip(args.interactorObject);
    }

    public string GetTooltipMessage()
    {
        return tooltip;
    }
}
