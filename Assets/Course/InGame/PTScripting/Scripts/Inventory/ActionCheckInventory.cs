namespace Course.PrototypeScripting
{
    public class ActionCheckInventory : Action
    {
        public string invItemName;

        public enum Comparison { Equal, Greater, GreaterOrEqual, Less, LessOrEqual }
        public Comparison vergleich;


        public int amount;

        public Sequence sequenceIfTrue;
        public Sequence sequenceIfFalse;

        override public void ExecuteAction()
        {
            switch (vergleich)
            {
                case Comparison.Equal:
                    ExecuteResultOfComparison(InventoryManager.Instance.GetAmount(invItemName) == amount);
                    break;
                case Comparison.Greater:
                    ExecuteResultOfComparison(InventoryManager.Instance.GetAmount(invItemName) > amount);
                    break;
                case Comparison.GreaterOrEqual:
                    ExecuteResultOfComparison(InventoryManager.Instance.GetAmount(invItemName) >= amount);
                    break;
                case Comparison.Less:
                    ExecuteResultOfComparison(InventoryManager.Instance.GetAmount(invItemName) < amount);
                    break;
                case Comparison.LessOrEqual:
                    ExecuteResultOfComparison(InventoryManager.Instance.GetAmount(invItemName) <= amount);
                    break;

            }
        }

        void ExecuteResultOfComparison(bool value)
        {

            if (value && sequenceIfTrue != null)
                sequenceIfTrue.ExecuteCompleteSequence();
            else if (!value && sequenceIfFalse != null)
                sequenceIfFalse.ExecuteCompleteSequence();
            else
                SequenceHandler.Instance.ReportActionEnd();
        }
        override public string GetAdditionalInfo()
        {
            if (sequenceIfFalse == null || sequenceIfTrue == null)
                return "- No Sequences set!";
            return "Is " + invItemName + " " + vergleich.ToString() + " " + amount + " ?";
        }
    }
}
