
using UnityEngine;

public class RenderPipelineBootstrap : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Boot()
    {
        RenderPipelineSwitcher.Initialize();
    }
}
