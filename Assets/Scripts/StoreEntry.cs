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

    public bool activateImmediately;

    private void Start()
    {
        label.text = $"{labelText} ({price})";
    }

    public void ButtonClick()
    {
        if(PurchasePossible())
        {
            Purchase();
        }
    }

    public bool PurchasePossible()
    {
        return Workshop.Instance.HasMoney(price);
    }

    public void Purchase()
    {
        Workshop.Instance.SubMoney(price);
        GameObject nObj = Instantiate(objToActivate);
        nObj.SetActive(true);
    }

    void Deactivate()
    {
        btn.interactable = false;
    }
 }
