using UnityEngine;

public interface I_TooltipProvider
{
    public void ActivateTooltip(UnityEngine.XR.Interaction.Toolkit.Interactors.IXRInteractor interactor,
                                UnityEngine.XR.Interaction.Toolkit.Interactables.IXRHoverInteractable interactable)
    {
      /*  InGameLog.Log($"ActivateTooltip tried");
        OnControllerTooltip side = OnControllerTooltip.GetCorrectSide(interactor);
        if (side == null)
            InGameLog.Log($"Fail");
        if (interactable == null)
            InGameLog.Log($"Fail2");*/
        OnControllerTooltip.GetCorrectSide(interactor).SetTooltip(GetTooltipMessage(), interactable.transform.position);
    }

    public void ClearTooltip(UnityEngine.XR.Interaction.Toolkit.Interactors.IXRInteractor interactor)
    {
        OnControllerTooltip.GetCorrectSide(interactor).ClearTooltip();
    }

    public string GetTooltipMessage();

    public void GetOwnGameObject()
    {

    }
}
