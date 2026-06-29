using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.InputSystem;

[System.Serializable]
public class VR_ButtonInput
{
    public VRButtonActionType actionType;
    public InputActionReference inputAction;
}

public enum VRButtonActionType
{
    Submit,
    Cancel,
    Hint,
    Menu,
    Custom
}

public interface IHoverInputReceiver
{
    void OnHoverInputPressed(VRButtonActionType type);
}

public class HoverInputForwarder : MonoBehaviour
{
    [SerializeField] private NearFarInteractor interactor;
    [SerializeField] private VR_ButtonInput[] buttonInputs;

    private void Awake()
    {
        foreach (VR_ButtonInput buttonInput in buttonInputs)
        {
            buttonInput.inputAction.action.Enable();
        }
    }

    private void Update()
    {
        foreach (VR_ButtonInput buttonInput in buttonInputs)
        {
            if (buttonInput == null)
                continue;

            if (buttonInput.inputAction.action == null)
                continue;

            if (!buttonInput.inputAction.action.WasPressedThisFrame())
                continue;

            ForwardInput(buttonInput.actionType);
        }
    }

    private void ForwardInput(VRButtonActionType type)
    {
        IXRHoverInteractable hovered =
            interactor.GetOldestInteractableHovered();

        if (hovered == null)
            return;

        if (hovered.transform.TryGetComponent(out IHoverInputReceiver receiver))
        {
            receiver.OnHoverInputPressed(type);
        }
    }
}