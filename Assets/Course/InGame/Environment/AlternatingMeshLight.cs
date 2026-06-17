using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class AlternatingMeshLight : MonoBehaviour
{
    [Header("Colors")]
    public Color color1 = Color.red;
    public Color color2 = Color.blue;

    [Header("Animation")]
    [Tooltip("Geschwindigkeit der Farbänderung")]
    public float speed = 1f;
    [Tooltip("Übergang von Material #1 zu Material #2 (0-1 Zeit, 0-1 Wert)")]
    public AnimationCurve curveTo2 = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [Tooltip("Übergang von Material #2 zurück zu Material #1 (0-1 Zeit, 0-1 Wert)")]
    public AnimationCurve curveTo1 = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    [Header("Optional Light")]
    [Tooltip("Optional: Licht, dessen Intensität sich an Material #2 anpasst")]
    public Light targetLight;

    private Renderer _renderer;
    private Material _material;
    private float _time;
    private float _baseLightIntensity;

    private static readonly int ColorID = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorID2 = Shader.PropertyToID("_EmissionColor");

 //   public string colorProperty;
  //  public string emProperty;
    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        // Instanz des Materials holen, um es direkt zu verändern
        _material = _renderer.material;

        if (targetLight != null)
            _baseLightIntensity = targetLight.intensity;
        SetOff();
    }

    void Update()
    {
        if (!active)
            return;
        _time += Time.deltaTime * speed;

        // Voller Zyklus 0..2: erste Hälfte 1->2, zweite Hälfte 2->1
        float cycle = Mathf.Repeat(_time, 2f);
        float t;
        if (cycle < 1f)
            t = curveTo2.Evaluate(cycle);
        else
            t = curveTo1.Evaluate(cycle - 1f);

        SetTo(t);
    }

    void SetTo(float value)
    {
        // Farbe direkt am Material setzen
        _material.SetColor(ColorID, Color.Lerp(color1, color2, value));
        _material.SetColor(ColorID2, Color.Lerp(color1, color2, value));
        // Lichtintensität an "Aktivierung" von Material #2 koppeln
        if (targetLight != null)
            targetLight.intensity = _baseLightIntensity * value;
    }

    void OnDestroy()
    {
        // Aufräumen der Material-Instanz, die durch .material erzeugt wurde
        if (_material != null)
            Destroy(_material);
    }
    bool active;
    public void SetOff()
    {
        _time = 0;
        SetTo(0f);
        active = false;
    }

    public void Activate()
    {
        active = true;
    }
}
