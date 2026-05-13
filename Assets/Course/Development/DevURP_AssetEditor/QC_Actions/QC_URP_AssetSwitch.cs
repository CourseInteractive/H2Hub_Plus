using UnityEngine;

public class QC_URP_AssetSwitch : QuickConsole_Entry
{
    public enum PipelineAction { Cycle, Set}
    public PipelineAction action;

    public int indexToSet;

    new private void Awake()
    {
        base.Awake();
        type = EntryType.Action;
    }

    public override void ExecuteAction()
    {
        if(action == PipelineAction.Cycle)
        {
            RenderPipelineSwitcher.CycleRenderPipelineAsset();
        }
        else
        {
            RenderPipelineSwitcher.UseRenderPipelineAsset(indexToSet);
        }
       
    }
}
