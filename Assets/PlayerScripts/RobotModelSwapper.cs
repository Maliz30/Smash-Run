using UnityEngine;

public class RobotModelSwapper : MonoBehaviour
{
    [Header("Matrizes de Geometria")]
    [Tooltip("Arraste o FBX estático (Pose T)")]
    [SerializeField] private GameObject malhaEstatica;
    
    [Tooltip("Arraste o FBX animado (Bash)")]
    [SerializeField] private GameObject malhaAnimada;

    [Header("Controles")]
    [Tooltip("Tecla para acionar a troca de malha")]
    [SerializeField] private KeyCode teclaAcionamento = KeyCode.P;

    private Animator _animatorAnimado;
    private static readonly int BashTrigger = Animator.StringToHash("Bash");

    private void Awake()
    {
        // Certifica-se de que a malha estática inicia visível e a animada invisível.
        if (malhaEstatica != null) malhaEstatica.SetActive(true);
        if (malhaAnimada != null) 
        {
            malhaAnimada.SetActive(false);
            _animatorAnimado = malhaAnimada.GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(teclaAcionamento))
        {
            TrocarParaAnimado();
        }
    }

    /// <summary>
    /// Acionado via input de teclado (Update) ou Botão UI.
    /// Realiza o sync matricial (evitando bugs de offset), desativa a estática, 
    /// ativa a animada e dispara a animação instantaneamente.
    /// </summary>
    public void TrocarParaAnimado()
    {
        if (malhaEstatica == null || malhaAnimada == null) return;

        // Equalização de vetores solicitada para evitar deslocamento de eixos.
        malhaAnimada.transform.position = malhaEstatica.transform.position;
        malhaAnimada.transform.rotation = malhaEstatica.transform.rotation;

        malhaEstatica.SetActive(false);
        malhaAnimada.SetActive(true);

        if (_animatorAnimado != null)
        {
            _animatorAnimado.SetTrigger(BashTrigger);
        }
    }
}
