using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImage))]
public class FisicaInclinacaoMarcador : MonoBehaviour
{
    [Header("Objeto controlado")]
    [SerializeField] private Rigidbody corpoAlvo;

    [Header("Resposta da inclinacao")]
    [SerializeField] private float multiplicadorForca = 1f;
    [SerializeField] private float inclinacaoMinimaGraus = 4f;
    [SerializeField] private float suavizacaoNormal = 10f;

    [Header("Amortecimento")]
    [SerializeField] private float amortecimentoLinear = 2.5f;
    [SerializeField] private float velocidadeMaxima = 2f;

    private ARTrackedImage imagemRastreada;
    private Vector3 normalSuavizada;
    private bool avisouCorpoAusente;

    void Awake()
    {
        imagemRastreada = GetComponent<ARTrackedImage>();

        if (corpoAlvo == null)
        {
            corpoAlvo = GetComponentInChildren<Rigidbody>();
        }

        if (corpoAlvo == null)
        {
            Debug.LogWarning("FisicaInclinacaoMarcador precisa de um Rigidbody atribuido ou definido em runtime.", this);
            avisouCorpoAusente = true;
        }

        normalSuavizada = transform.up;

        if (corpoAlvo != null)
        {
            ConfigurarCorpoAlvo();
        }
    }

    void FixedUpdate()
    {
        if (corpoAlvo == null)
        {
            return;
        }

        if (imagemRastreada.trackingState != TrackingState.Tracking)
        {
            AplicarAmortecimento();
            return;
        }

        normalSuavizada = Vector3.Slerp(
            normalSuavizada,
            transform.up,
            suavizacaoNormal * Time.fixedDeltaTime);

        float inclinacaoAtual = Vector3.Angle(normalSuavizada, Vector3.up);
        if (inclinacaoAtual < inclinacaoMinimaGraus)
        {
            AplicarAmortecimento();
            return;
        }

        Vector3 forcaPlano = Vector3.ProjectOnPlane(Physics.gravity, normalSuavizada) * corpoAlvo.mass * multiplicadorForca;
        corpoAlvo.AddForce(forcaPlano, ForceMode.Force);

        if (corpoAlvo.linearVelocity.sqrMagnitude > velocidadeMaxima * velocidadeMaxima)
        {
            corpoAlvo.linearVelocity = corpoAlvo.linearVelocity.normalized * velocidadeMaxima;
        }
    }

    private void AplicarAmortecimento()
    {
        Vector3 velocidadeAtual = corpoAlvo.linearVelocity;
        corpoAlvo.linearVelocity = Vector3.Lerp(velocidadeAtual, Vector3.zero, amortecimentoLinear * Time.fixedDeltaTime);
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

    private void ConfigurarCorpoAlvo()
    {
        corpoAlvo.isKinematic = false;
        corpoAlvo.useGravity = false;
    }
}