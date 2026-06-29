using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
public class GrabTooltip : MonoBehaviour, I_TooltipProvider
{
    void Awake()
    {
        XRGrabInteractable grabable = GetComponent<XRGrabInteractable>();
        if (grabable)
        {
            grabable.hoverEntered.AddListener(VR_HoverEnter);
            grabable.hoverExited.AddListener(VR_HoverExit);
        }
    }

    public void VR_HoverEnter(HoverEnterEventArgs args)
    {
    //    InGameLog.Log($"Objekt {name} Tooltip Hover ENTER");
        ((I_TooltipProvider)this).ActivateTooltip(args.interactorObject, args.interactableObject);
    }

    public void VR_HoverExit(HoverExitEventArgs args)
    {
        ((I_TooltipProvider)this).ClearTooltip(args.interactorObject);
    }

    public string GetTooltipMessage()
    {
        return GameData.Instance.Values.defaultGrabTooltip;
    }
}
