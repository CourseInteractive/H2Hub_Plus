using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class TeleportModeEvent : MonoBehaviour
{
    public XRRayInteractor interactor;

    public void OnEnable()
    {
        if(TeleportPointContainer.Instance && interactor.enabled)
            TeleportPointContainer.Instance.Activate();
    }

    public void OnDisable()
    {
        if (TeleportPointContainer.Instance)
            TeleportPointContainer.Instance.Deactivate();
    }
}
