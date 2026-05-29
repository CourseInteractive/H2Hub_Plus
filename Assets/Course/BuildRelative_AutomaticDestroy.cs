using UnityEngine;
using UnityEngine.XR.Management;
public class BuildRelative_AutomaticDestroy : MonoBehaviour
{
    public enum DestroyOn { Editor, VR, PC}
    public DestroyOn destroyOnScenario;

    public bool vrActive;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        vrActive = XRGeneralSettings.Instance != null &&
    XRGeneralSettings.Instance.Manager != null &&
    XRGeneralSettings.Instance.Manager.activeLoader != null;

        if (destroyOnScenario == DestroyOn.Editor)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor && !vrActive)
                DestroyImmediate(gameObject);
        }
        else if (destroyOnScenario == DestroyOn.VR)
        {
            if (Application.platform != RuntimePlatform.WindowsEditor || vrActive)
                DestroyImmediate(gameObject);
            
        }
    }

}
