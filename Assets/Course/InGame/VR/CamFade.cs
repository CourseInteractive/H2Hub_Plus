using UnityEngine;

public class CamFade : MonoBehaviour
{
    public static CamFade Instance;

    [Header("References")]
    public MeshRenderer meshRenderer;
    public int materialIndex = 0;

    [Header("Settings")]
    public float fadeInDuration = 1f; // Ausblenden beim Start
    public float fadeOutDuration = 1f; // Einblenden auf Befehl

    private Material mat;

    private float timer;
    private float duration;
    private float startAlpha;
    private float targetAlpha;
    private bool isFading;

    // ───────────────────────────────────────────────────────────
    private void Start()
    {
        Instance = this;
        mat = meshRenderer.materials[materialIndex];
        EnsureFadeMode();
        meshRenderer.enabled = true;
        SetAlpha(1f);
        StartFade(0f, fadeInDuration);
    }

    private void Update()
    {
        if (!isFading) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);
        SetAlpha(Mathf.Lerp(startAlpha, targetAlpha, t));

        if (t >= 1f)
            isFading = false;
    }

    // ── Public API ─────────────────────────────────────────────
    public void FadeIn()
    {
        StartFade(1f, fadeOutDuration);
    }

    // ───────────────────────────────────────────────────────────
    private void StartFade(float target, float fadeDuration)
    {
        startAlpha = mat.color.a;
        targetAlpha = target;
        duration = fadeDuration;
        timer = 0f;
        isFading = true;
    }

    private void SetAlpha(float alpha)
    {
        Color c = mat.color;
        c.a = alpha;
        mat.color = c;
    }

    // ── Material auf Transparent-Mode setzen ───────────────────
    private void EnsureFadeMode()
    {
        mat.SetFloat("_Mode", 2);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 4000;
    }

    private void OnDestroy()
    {
        if (mat != null) Destroy(mat);
    }
}