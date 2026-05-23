using UnityEngine;

[RequireComponent(typeof(VuforiaPlanoMultiMarcador))]
public class VuforiaFisicaPlanoInclinado : MonoBehaviour
{
    [Header("Objeto controlado")]
    [SerializeField] private Rigidbody corpoAlvo;

    [Header("Resposta da inclinacao")]
    [SerializeField] private bool usarReferenciaCalibrada = true;
    [SerializeField] private float multiplicadorForca = 1f;
    [SerializeField] private float inclinacaoMinimaGraus = 4f;

    [Header("Amortecimento")]
    [SerializeField] private float amortecimentoLinear = 2.5f;
    [SerializeField] private float velocidadeMaxima = 2f;

    private VuforiaPlanoMultiMarcador planoMultiMarcador;
    private bool avisouCorpoAusente;

    void Awake()
    {
        planoMultiMarcador = GetComponent<VuforiaPlanoMultiMarcador>();

        if (corpoAlvo == null)
        {
            corpoAlvo = GetComponentInChildren<Rigidbody>();
        }

        if (corpoAlvo == null)
        {
            Debug.LogWarning("VuforiaFisicaPlanoInclinado precisa de um Rigidbody atribuido ou definido em runtime.", this);
            avisouCorpoAusente = true;
            return;
        }

        ConfigurarCorpoAlvo();
    }

    void FixedUpdate()
    {
        if (corpoAlvo == null)
        {
            if (!avisouCorpoAusente)
            {
                Debug.LogWarning("VuforiaFisicaPlanoInclinado esta sem Rigidbody alvo.", this);
                avisouCorpoAusente = true;
            }

            return;
        }

        if (!planoMultiMarcador.PossuiPoseConfiavel)
        {
            AplicarAmortecimento();
            return;
        }

        Vector3 normalPlano = usarReferenciaCalibrada
            ? planoMultiMarcador.NormalCalibradaMundo
            : planoMultiMarcador.NormalPlanoMundo;

        float inclinacaoAtual = Vector3.Angle(normalPlano, Vector3.up);
        if (inclinacaoAtual < inclinacaoMinimaGraus)
        {
            AplicarAmortecimento();
            return;
        }

        Vector3 forcaPlano = Vector3.ProjectOnPlane(Physics.gravity, normalPlano) * corpoAlvo.mass * multiplicadorForca;
        corpoAlvo.AddForce(forcaPlano, ForceMode.Force);

        float velocidadeMaximaQuadrada = velocidadeMaxima * velocidadeMaxima;
        if (corpoAlvo.linearVelocity.sqrMagnitude > velocidadeMaximaQuadrada)
        {
            corpoAlvo.linearVelocity = corpoAlvo.linearVelocity.normalized * velocidadeMaxima;
        }
    }

    public void DefinirCorpoAlvo(Rigidbody novoCorpo)
    {
        corpoAlvo = novoCorpo;

        if (corpoAlvo == null)
        {
            return;
        }

        ConfigurarCorpoAlvo();
        avisouCorpoAusente = false;
    }

    public void CapturarReferenciaAtual()
    {
        planoMultiMarcador.CapturarReferenciaAtual();
    }

    private void AplicarAmortecimento()
    {
        Vector3 velocidadeAtual = corpoAlvo.linearVelocity;
        corpoAlvo.linearVelocity = Vector3.Lerp(velocidadeAtual, Vector3.zero, amortecimentoLinear * Time.fixedDeltaTime);
    }

    private void ConfigurarCorpoAlvo()
    {
        corpoAlvo.isKinematic = false;
        corpoAlvo.useGravity = false;
    }
}