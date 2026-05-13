using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropDownSelectionBlock : MonoBehaviour
{
    public int amount;
    public void UpdateValurDrawingForTime()
    {
        VALUR.Data.displayManager.blockUpdater = amount;
    }

    public void SelectionEnds()
    {
        VALUR.Data.displayManager.blockUpdater = 0;
    }
}
