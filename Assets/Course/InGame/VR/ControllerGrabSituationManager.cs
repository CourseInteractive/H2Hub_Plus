using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
public class ControllerGrabSituationManager : MonoBehaviour
{
    public static ControllerGrabSituationManager instance;

    public NearFarInteractor leftGrab;
    public NearFarInteractor rightGrab;

    public XRRayInteractor teleportLeft;
    public XRRayInteractor teleportRight;

    public bool leftHoldingObject;
    public bool rightHoldingObject;

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        leftHoldingObject = leftGrab.firstInteractableSelected != null;
        if (leftHoldingObject)
            teleportLeft.enabled = false;
        else
            teleportLeft.enabled = true;
        rightHoldingObject = rightGrab.firstInteractableSelected != null;
        if (rightHoldingObject)
            teleportRight.enabled = false;
        else
            teleportRight.enabled = true;
    }

    public bool LeftIsHolding(GameObject obj)
    {
        InGameLog.Log($"LeftIsHolding");
        if (leftGrab.firstInteractableSelected == null)
            return false;
        return ((MonoBehaviour)leftGrab.firstInteractableSelected).gameObject == obj;
    }

    public bool RightIsHolding(GameObject obj)
    {
        InGameLog.Log($"RightIsHolding");
        if (rightGrab.firstInteractableSelected == null)
            return false;
        return ((MonoBehaviour)rightGrab.firstInteractableSelected).gameObject == obj;
    }
    public bool ObjectIsHoldByController(GameObject obj)
    {
        bool holded = false;
        holded = RightIsHolding(obj);
        if(!holded)
            holded = LeftIsHolding(obj);
        InGameLog.Log($"{obj} is holded");
        return holded;
    }

    public bool AnyControllerIsHoldingObject()
    {
        return leftHoldingObject || rightHoldingObject;
    }
}
