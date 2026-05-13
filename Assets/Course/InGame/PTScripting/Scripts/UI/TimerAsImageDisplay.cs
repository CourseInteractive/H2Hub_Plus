using UnityEngine.UI;

namespace Course.PrototypeScripting
{
    public class TimerAsImageDisplay : DisplayTimer
    {
        public enum VisualDisplayType { FillAmount }
        public VisualDisplayType displayType;

        public Image uiImage;

        // Start is called before the first frame update
        void Awake()
        {
            uiImage = GetComponent<Image>();
        }

        public override void AdjustUI()
        {
            uiImage.fillAmount = myTimer.GetPercent();
        }
    }
}
