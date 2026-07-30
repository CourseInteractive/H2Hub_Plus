using UnityEngine;
using TMPro;

/// <summary>
/// Displays a <see cref="Commission"/> in the UI using TextMeshPro elements.
/// Assign the TMP fields in the Inspector, then call <see cref="SetCommission"/> at runtime.
/// </summary>
public class CommissionUI : MonoBehaviour
{
    // ── Runtime State ───────────────────────────────────────────────────────

    public Commission commission;

    // ── Inspector References ────────────────────────────────────────────────

    [Header("TMP Text Fields")]
    [SerializeField] private TMP_Text receiverText;

    [Tooltip("Displays the reward value.")]
    [SerializeField] private TMP_Text rewardText;

    [Tooltip("Displays the required amount.")]
    [SerializeField] private TMP_Text amountText;

    [Tooltip("Displays the resource type ID (or a mapped name).")]
    [SerializeField] private TMP_Text resourceTypeText;

    // ── Formatting ──────────────────────────────────────────────────────────

    [Header("Label Prefixes (optional)")]
    [SerializeField] private string rewardPrefix       = "Belohnung: ";
    [SerializeField] private string amountPrefix       = "Menge: ";
    [SerializeField] private string resourceTypePrefix = "Ressourcentyp: ";

    [Header("Reward Format")]
    [Tooltip("C# format string for the reward float, e.g. \"0\" or \"0.##\"")]
    [SerializeField] private string rewardFormat = "0";

   

    // ── Public API ──────────────────────────────────────────────────────────

    private void Start()
    {
        Refresh();
    }

    /// <summary>
    /// Binds a <see cref="Commission"/> to the UI and refreshes all text fields immediately.
    /// </summary>
    public void SetCommission(Commission commission)
    {
        commission = commission;
        Refresh();
    }

    /// <summary>
    /// Re-reads <see cref="currentCommission"/> and updates all TMP fields.
    /// Call this whenever the commission data changes at runtime.
    /// </summary>
    public void Refresh()
    {
        if (commission == null)
        {
            ClearUI();
            return;
        }
        if (receiverText != null)
            receiverText.text = commission.receiver;

        if (rewardText != null)
            rewardText.text = rewardPrefix + commission.reward.ToString(rewardFormat);

        if (amountText != null)
            amountText.text = amountPrefix + commission.amount.ToString();

        if (resourceTypeText != null)
            resourceTypeText.text = resourceTypePrefix + commission.resourceType.ToString();
    }

    /// <summary>Clears all text fields (e.g. when no commission is active).</summary>
    public void ClearUI()
    {
        if (rewardText      != null) rewardText.text      = rewardPrefix      + "–";
        if (amountText      != null) amountText.text      = amountPrefix      + "–";
        if (resourceTypeText != null) resourceTypeText.text = resourceTypePrefix + "–";
    }

    public void ButtonClick()
    {
        if (commission.CanBePerformed())
            commission.PerformAndComplete();
        else
            PerformFailed();
    }

    public void PerformFailed()
    {
        GameEventManager.Instance.ReportGameEvent("CommissionFailed");
    }
}
