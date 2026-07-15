using UnityEngine;
using TMPro;

public class PlayerDashCharges : MonoBehaviour
{
    [Header("Cargas de Dash")]
    [SerializeField] private int maxCharges = 3;
    private int currentCharges;

    [Header("Interface (UI)")]
    [SerializeField] private TextMeshProUGUI textoDashes;

    private void Awake()
    {
        currentCharges = maxCharges;
        AtualizarInterface();
    }

    public bool TryConsumeCharge()
    {
        if (currentCharges <= 0)
            return false;

        currentCharges--;
        AtualizarInterface();
        return true;
    }

    public void AddCharges(int amount)
    {
        currentCharges = Mathf.Min(currentCharges + amount, maxCharges);
        AtualizarInterface();
    }

    private void AtualizarInterface()
    {
        if (textoDashes != null)
            textoDashes.text = $"DASHES: {currentCharges}";
    }

    public int CurrentCharges => currentCharges;
    public int MaxCharges => maxCharges;
}
