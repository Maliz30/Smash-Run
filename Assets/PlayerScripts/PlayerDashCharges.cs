using UnityEngine;

public class PlayerDashCharges : MonoBehaviour
{
    [SerializeField] private int maxCharges = 3;
    private int currentCharges;

    private void Awake()
    {
        currentCharges = maxCharges;
    }

    public bool TryConsumeCharge()
    {
        if (currentCharges <= 0)
            return false;

        currentCharges--;
        return true;
    }

    public void AddCharges(int amount)
    {
        currentCharges = Mathf.Min(currentCharges + amount, maxCharges);
    }

    public int CurrentCharges => currentCharges;
    public int MaxCharges => maxCharges;
}
