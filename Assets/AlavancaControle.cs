using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class AlavancaControle : MonoBehaviour, IHammerInputProvider
{
    [Header("Partes da Alavanca")]
    [Tooltip("Ponto de pivô na base da alavanca (onde ela gira).")]
    [SerializeField] private Transform pivo;
    [Tooltip("Visual do cabo que vai girar. Deve ser filho do pivô.")]
    [SerializeField] private Transform cabo;

    [Header("Configurações")]
    [Tooltip("Ângulo máximo de inclinação da alavanca em graus.")]
    [SerializeField] private float anguloMaximo = 45f;
    [Tooltip("Velocidade com que a alavanca retorna ao centro ao soltar.")]
    [SerializeField] private float velocidadeRetorno = 8f;

    private Transform controllerNaZona;
    private bool estaSegurando;
    private Vector2 inputAtual;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    // Detecta o controle entrando na zona da alavanca (configure o layer no Physics Matrix)
    private void OnTriggerEnter(Collider other)
    {
        if (controllerNaZona == null)
            controllerNaZona = other.transform;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == controllerNaZona)
        {
            controllerNaZona = null;
            estaSegurando = false;
        }
    }

    private void Update()
    {
        if (pivo == null) return;

        bool gripApertado = OVRInput.Get(OVRInput.RawButton.LHandTrigger)
                         || OVRInput.Get(OVRInput.RawButton.RHandTrigger);

        if (controllerNaZona != null && gripApertado)
        {
            estaSegurando = true;
            CalcularInput();
        }
        else
        {
            estaSegurando = false;
            inputAtual = Vector2.Lerp(inputAtual, Vector2.zero, Time.deltaTime * velocidadeRetorno);
        }

        AtualizarVisual();
    }

    private void CalcularInput()
    {
        // Direção do pivô até o controle → converte inclinação em input XZ
        Vector3 direcao = (controllerNaZona.position - pivo.position).normalized;
        float angulo = Vector3.Angle(Vector3.up, direcao);
        float magnitude = Mathf.Clamp01(angulo / anguloMaximo);

        Vector3 horizontal = new Vector3(direcao.x, 0f, direcao.z);
        if (horizontal.sqrMagnitude > 0.001f)
        {
            horizontal.Normalize();
            inputAtual = new Vector2(horizontal.x, horizontal.z) * magnitude;
        }
        else
        {
            inputAtual = Vector2.zero;
        }
    }

    private void AtualizarVisual()
    {
        if (cabo == null) return;

        // Rotaciona o cabo no eixo X (frente/trás) e Z (esquerda/direita)
        float rotX = -inputAtual.y * anguloMaximo;
        float rotZ = -inputAtual.x * anguloMaximo;
        cabo.localRotation = Quaternion.Euler(rotX, 0f, rotZ);
    }

    // --- IHammerInputProvider ---

    public Vector3 GetMovementInput() => new Vector3(inputAtual.x, 0f, inputAtual.y);

    public float GetHeightInput() => 0f;

    public bool GetAttackTrigger() => false;
}
