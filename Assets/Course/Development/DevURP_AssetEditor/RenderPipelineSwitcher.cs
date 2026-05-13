
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public static class RenderPipelineSwitcher
{
    private const string ResourcesPath = "URP_PipelineRegistry";

    private static UrpPipelineRegistry _registry;
    private static bool _initialized;

    public static event Action<int, UniversalRenderPipelineAsset> OnPipelineChanged;
    public static event Action<int, ScriptableRendererData> OnRendererChanged;



    public static void Initialize()
    {
        if (_initialized) return;

        _registry = Resources.Load<UrpPipelineRegistry>(ResourcesPath);
        if (_registry == null)
        {
            Debug.LogError($"RenderPipelineSwitcher: Registry not found at Resources/{ResourcesPath}.asset");
            _initialized = true;
            return;
        }

        NormalizeState();
        ApplyPipeline(_registry.currentPipelineIndex, true);

        _initialized = true;
    }

    private static void NormalizeState()
    {
        if (_registry.entries == null || _registry.entries.Count == 0) return;

        _registry.currentPipelineIndex = Mathf.Clamp(_registry.currentPipelineIndex, 0, _registry.entries.Count - 1);

        while (_registry.currentRendererIndexPerPipeline.Count < _registry.entries.Count)
            _registry.currentRendererIndexPerPipeline.Add(0);

        for (int i = 0; i < _registry.entries.Count; i++)
        {
            var entry = _registry.entries[i];
            int max = (entry.rendererData != null && entry.rendererData.Count > 0) ? entry.rendererData.Count - 1 : 0;
            _registry.currentRendererIndexPerPipeline[i] = Mathf.Clamp(_registry.currentRendererIndexPerPipeline[i], 0, max);
        }
    }

    public static void CycleRenderPipelineAsset()
    {
        EnsureInit();
        if (!IsValid()) return;

        int next = (_registry.currentPipelineIndex + 1) % _registry.entries.Count;
        UseRenderPipelineAsset(next);
    }

    public static void UseRenderPipelineAsset(int index)
    {
        EnsureInit();
        if (!IsValid()) return;

        index = Mathf.Clamp(index, 0, _registry.entries.Count - 1);
        _registry.currentPipelineIndex = index;

        ApplyPipeline(index, true);
    }

    public static void CycleRenderer()
    {
        EnsureInit();
        if (!IsValid()) return;

        var entry = _registry.entries[_registry.currentPipelineIndex];
        if (entry.rendererData == null || entry.rendererData.Count == 0)
        {
            Debug.LogWarning("No renderer data defined for this pipeline.");
            return;
        }

        int p = _registry.currentPipelineIndex;
        int cur = _registry.currentRendererIndexPerPipeline[p];
        int next = (cur + 1) % entry.rendererData.Count;
        UseRenderer(next);
    }

    public static void UseRenderer(int rendererIndex)
    {
        EnsureInit();
        if (!IsValid()) return;

        int p = _registry.currentPipelineIndex;
        var entry = _registry.entries[p];

        if (entry.rendererData == null || entry.rendererData.Count == 0)
        {
            Debug.LogWarning("No renderer data defined for this pipeline.");
            return;
        }

        rendererIndex = Mathf.Clamp(rendererIndex, 0, entry.rendererData.Count - 1);
        _registry.currentRendererIndexPerPipeline[p] = rendererIndex;

        ApplyPipeline(p, true);
    }

    private static void ApplyPipeline(int pipelineIndex, bool applyRendererToo)
    {
        var entry = _registry.entries[pipelineIndex];

        if (entry.pipelineAsset == null)
        {
            Debug.LogError($"Pipeline asset NULL at index {pipelineIndex}");
            return;
        }

        QualitySettings.renderPipeline = entry.pipelineAsset;
        GraphicsSettings.defaultRenderPipeline = entry.pipelineAsset;

        OnPipelineChanged?.Invoke(pipelineIndex, entry.pipelineAsset);

        if (applyRendererToo)
            ApplyRendererForCurrentPipeline(entry, pipelineIndex);
    }

    private static void ApplyRendererForCurrentPipeline(UrpPipelineRegistry.PipelineEntry entry, int pipelineIndex)
    {
        if (entry.rendererData == null || entry.rendererData.Count == 0) return;

        int rIndex = _registry.currentRendererIndexPerPipeline[pipelineIndex];
        rIndex = Mathf.Clamp(rIndex, 0, entry.rendererData.Count - 1);

        var chosen = entry.rendererData[rIndex];

        OnRendererChanged?.Invoke(rIndex, chosen);

        Debug.Log($"Renderer selected: {chosen.name}. Note: safest method is one URP asset per renderer.");
    }

    private static void EnsureInit()
    {
        if (!_initialized) Initialize();
    }

    private static bool IsValid()
    {
        return _registry != null && _registry.entries != null && _registry.entries.Count > 0;
    }

    public static string GetCurrentRPAssetName()
    {
       return _registry.entries[_registry.currentPipelineIndex].pipelineAsset.name;
    }
}
