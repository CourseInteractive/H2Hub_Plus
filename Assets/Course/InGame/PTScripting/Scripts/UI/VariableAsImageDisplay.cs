using UnityEngine.UI;

namespace Course.PrototypeScripting
{
    public class VariableAsImageDisplay : DisplayVariables
    {
        public enum VisualDisplayType { FillAmount }
        public VisualDisplayType displayType;

        public Image uiImage;
        public int maxAmount = 100;

        // Start is called before the first frame update
        void Awake()
        {
            uiImage = GetComponent<Image>();
        }

        public override void AdjustUI()
        {
            int varAmount = VariableManager.Instance.GetVariable(variableName);
            uiImage.fillAmount = (varAmount * 1f) / (maxAmount * 1f);
        }
    }
}
