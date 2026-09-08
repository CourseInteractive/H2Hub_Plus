using UnityEngine;
using TMPro;

public class VillageUI : MonoBehaviour
{

    [SerializeField] private TMP_Text costLabel;
    public string costFormat;

    public GameObject waterProblemObject;
    public GameObject energyProblemObject;
    public GameObject warmthProblemObject;
    // Update is called once per frame
    void Update()
    {
        costLabel.text = string.Format(costFormat, Village.Instance.waterPrice, Village.Instance.energyPrice, Village.Instance.warmthPrice);
        if (Village.Instance.energyProblemRunning)
            energyProblemObject.SetActive(true);
        else
            energyProblemObject.SetActive(false);
        if (Village.Instance.waterProblemRunning)
            waterProblemObject.SetActive(true);
        else
            waterProblemObject.SetActive(false);
        if (Village.Instance.warmthProblemRunning)
            warmthProblemObject.SetActive(true);
        else
            warmthProblemObject.SetActive(false);

    }


}
