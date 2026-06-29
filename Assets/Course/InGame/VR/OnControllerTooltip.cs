using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class OnControllerTooltip : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text text;

    public Vector3 closePosition;
    public Vector3 rayPosition;

    public bool left;
    public static OnControllerTooltip instanceLeft;
    public static OnControllerTooltip instanceRight;

    public IXRInteractor interactor;
    public GameObject interactorObject;
    public enum Position { Close, Ray}



    public static OnControllerTooltip GetCorrectSide(UnityEngine.XR.Interaction.Toolkit.Interactors.IXRInteractor interactor)
    {
        if (instanceLeft && instanceLeft.interactor == interactor)
            return instanceLeft;
        return instanceRight;
    }

    private void Awake()
    {
        interactor = interactorObject.GetComponent<IXRInteractor>();
        if (left)
            instanceLeft = this;
        else
            instanceRight = this;
        ClearTooltip();
    }

    public void SetTooltip(string message)
    {
        SetTooltip(message, Vector3.positiveInfinity);
    }

    public void SetTooltip(string message, Position position)
    {
        SetTooltip(message, Vector3.positiveInfinity, position);
    }

    public void SetTooltip(string message, Vector3 hitPosition, Position position = Position.Ray)
    {
        string localizedTooltip = SimpleLocalization.GetLocalization(message);
        panel.SetActive(true);
        text.text = localizedTooltip;
        InGameLog.Log(localizedTooltip);
        if (Vector3.Distance(hitPosition, transform.parent.TransformPoint(closePosition)) < Vector3.Distance(transform.parent.TransformPoint(closePosition), transform.parent.TransformPoint(rayPosition)))
            position = Position.Close;
        
        if (position == Position.Close)
            transform.transform.localPosition = closePosition;
        else
            transform.transform.localPosition = rayPosition;

    }

    public void ClearTooltip()
    {
        panel.SetActive(false);
        text.text = "";
        InGameLog.Log("ClearTooltip");
    }


}
