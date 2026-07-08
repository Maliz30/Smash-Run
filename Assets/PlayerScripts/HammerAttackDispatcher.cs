using UnityEngine;

/// <summary>
/// Decide qual martelo ataca com base em qual lado do mapa a AreaAtaque está.
/// Área à esquerda do centro -> martelo esquerdo ataca.
/// Área à direita (ou exatamente no centro) -> martelo direito ataca.
/// </summary>
[DisallowMultipleComponent]
public class HammerAttackDispatcher : MonoBehaviour
{
    [SerializeField] private HammerController leftHammer;
    [SerializeField] private HammerController rightHammer;
    [SerializeField] private Transform attackArea;
    [SerializeField] private float centerX = 0f;

    public bool TriggerAttack()
    {
        if (attackArea == null)
        {
            return false;
        }

        HammerController hammer = attackArea.position.x < centerX ? leftHammer : rightHammer;

        return hammer != null && hammer.TryStartAttack(attackArea.position);
    }
}
