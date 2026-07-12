using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RobotAnimationController : MonoBehaviour
{
    private Animator _animator;
    private static readonly int BashTrigger = Animator.StringToHash("Bash");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Aciona o estado de animação "Bash" no Animator acoplado.
    /// Chamado externamente via eventos de botões da interface de usuário.
    /// </summary>
    public void TriggerBashAnimation()
    {
        if (_animator != null)
        {
            _animator.SetTrigger(BashTrigger);
        }
    }
}
