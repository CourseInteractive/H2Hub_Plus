
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(menuName = "Rendering/URP Pipeline Registry", fileName = "URP_PipelineRegistry")]
public class UrpPipelineRegistry : ScriptableObject
{
    [Serializable]
    public class PipelineEntry
    {
        public UniversalRenderPipelineAsset pipelineAsset;

        // Optional renderer list for this pipeline
        public List<ScriptableRendererData> rendererData = new();
    }

    public List<PipelineEntry> entries = new();

    [Tooltip("Current pipeline index")]
    public int currentPipelineIndex = 0;

    [Tooltip("Current renderer index per pipeline")]
    public List<int> currentRendererIndexPerPipeline = new();
}
