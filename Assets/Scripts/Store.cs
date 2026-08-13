using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Store : MonoBehaviour
{


    public enum ActivationState { RunningNormally, Untouchable, Deactivated, OnlyDisplay }
    public ActivationState state;
    public GameObject startUpPanel;
    public GameObject mainPanel;

    StoreEntry activeEntry = null;

    public GameObject detailView;

    public TMP_Text detailHeader;
    public Button buyButton;
    public TMP_Text buyButtonText;
    public TMP_Text detail01_Text;
    public TMP_Text detail02_Text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetToState(state);
        SelectEntry(null);
    }

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
                SelectEntry(null);
                break;
            case ActivationState.Untouchable:
                startUpPanel.SetActive(false);
                mainPanel.SetActive(false);
                SelectEntry(null);
                break;
            case ActivationState.OnlyDisplay:
                startUpPanel.SetActive(false);
                mainPanel.SetActive(true);
                SelectEntry(null);
                break;
        }
    }

    public void SelectEntry(StoreEntry entry)
    {
      //  Debug.Log("Jo");
        activeEntry = entry;
        

        UpdateDetailDisplay();
        
    }

    void UpdateDetailDisplay()
    {
        if (activeEntry == null)
        {
            detailView.SetActive(false);
            return;
        }
        detailHeader.text = activeEntry.labelText;
        buyButtonText.text = $"Kaufen ({ activeEntry.price})";
        if (activeEntry.PurchasePossible())
            buyButton.interactable = true;
        else
            buyButton.interactable = false;
        string details = activeEntry.detailToken;
        detail01_Text.text = "";
        detail02_Text.text = "";
        if (details.Trim() != "")
        {
            string[] detailContent = (SimpleLocalization.GetLocalization(details)).Split('|');
            detail01_Text.text = detailContent[0];
            if (detailContent.Length > 1)
            {
                detail02_Text.text = detailContent[1];
            }
        }
        detailView.SetActive(true);
    }

    public void BuyCurrentSelection()
    {
        activeEntry.Purchase();
        UpdateDetailDisplay();
    }

    public void SetToState(int i)
    {
        SetToState((ActivationState)i);
    }
}
