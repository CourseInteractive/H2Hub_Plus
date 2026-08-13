using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreEntry : MonoBehaviour
{
    public GameObject objToActivate;
    public int price;
    public string labelText;
    public string detailToken;

    public TMP_Text label;
    public Button btn;
    public string identifier;

    public bool activateImmediately;
    public bool repeatable = true;
    public bool instantiate = true;

    public Store store;
    private void Start()
    {
        label.text = $"{labelText}"; // ({price})";
    }

    public void ButtonClick()
    {
        /* if(PurchasePossible())
         {
             Purchase();
             if (!repeatable)
                 btn.interactable = false;
         }*/
        store.SelectEntry(this);
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
