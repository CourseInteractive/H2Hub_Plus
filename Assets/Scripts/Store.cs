using UnityEngine;

public class Store : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetToState(state);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public enum ActivationState { RunningNormally, Untouchable, Deactivated, OnlyDisplay }
    public ActivationState state;
    public GameObject startUpPanel;
    public GameObject mainPanel;
    public void SetToState(ActivationState newState)
    {
        state = newState;

        switch (newState)
        {
            case ActivationState.RunningNormally:
                startUpPanel.SetActive(false);
                mainPanel.SetActive(true);
                break;
            case ActivationState.Deactivated:
                startUpPanel.SetActive(true);
                mainPanel.SetActive(false);
                break;
            case ActivationState.Untouchable:
                startUpPanel.SetActive(false);
                mainPanel.SetActive(false);
                break;
            case ActivationState.OnlyDisplay:
                startUpPanel.SetActive(false);
                mainPanel.SetActive(true);
                break;
        }
    }

    public void SetToState(int i)
    {
        SetToState((ActivationState)i);
    }
}
