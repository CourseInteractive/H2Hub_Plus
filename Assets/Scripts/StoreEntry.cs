using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreEntry : MonoBehaviour
{
    public GameObject objToActivate;
    public int price;
    public string labelText;

    public TMP_Text label;
    public Button btn;
    public string identifier;

    public bool activateImmediately;
    public bool repeatable = true;
    public bool instantiate = true;
    private void Start()
    {
        label.text = $"{labelText} ({price})";
    }

    public void ButtonClick()
    {
        if(PurchasePossible())
        {
            Purchase();
            if (!repeatable)
                btn.interactable = false;
        }
    }

    public bool PurchasePossible()
    {
        return Workshop.Instance.HasMoney(price);
    }

    public void Purchase()
    {
        GameEventManager.Instance.ReportGameEvent("Purchase", identifier);
            Workshop.Instance.SubMoney(price);
        if(instantiate)
        {
            GameObject nObj = Instantiate(objToActivate, objToActivate.transform.position, objToActivate.transform.rotation);
            nObj.SetActive(true);
        }
      else
        {
            objToActivate.SetActive(true);
        }
    }

    void Deactivate()
    {
        btn.interactable = false;
    }
 }
