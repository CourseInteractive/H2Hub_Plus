namespace Course.PrototypeScripting
{
    public class VariableAs_Template : DisplayVariables
    {
        // Darf keine Start-Funktion enthalten!

        public override void AdjustUI()
        {
            // Wird immer ausgeführt, wenn sich Variablen verändert haben
            int value = VariableManager.Instance.GetVariable(variableName);
        }
    }
}
