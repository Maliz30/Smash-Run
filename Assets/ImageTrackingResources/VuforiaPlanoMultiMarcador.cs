using System;
using UnityEngine;
using Vuforia;

public class VuforiaPlanoMultiMarcador : MonoBehaviour
{
    public enum EixoNormalPlano
    {
        Up,
        Forward,
        Right
    }

    public enum FiltroTracking
    {
        TrackedOnly,
        TrackedAndExtendedTracked
    }

    [Serializable]
    public struct MarcadorPlano
    {
        public string nome;
        public ObserverBehaviour observer;
        public Vector3 posicaoLocalNoPlano;
        public Vector3 rotacaoLocalNoPlanoEuler;
        [Min(0.01f)] public float peso;
    }

    [Header("Marcadores")]
    [SerializeField] private MarcadorPlano[] marcadores;
    [SerializeField] private FiltroTracking filtroTracking = FiltroTracking.TrackedAndExtendedTracked;
    [SerializeField, Min(1)] private int minimoMarcadoresVisiveis = 1;

    [Header("Pose do plano")]
    [SerializeField] private bool atualizarTransformDoObjeto = true;
    [SerializeField, Min(0f)] private float suavizacaoPosicao = 12f;
    [SerializeField, Min(0f)] private float suavizacaoRotacao = 12f;
    [SerializeField] private EixoNormalPlano eixoNormalPlano = EixoNormalPlano.Up;

    [Header("Calibracao")]
    [SerializeField] private bool calibrarAutomaticamenteNoPrimeiroTracking = true;

    private Pose poseAtual;
    private bool poseInicializada;
    private Quaternion rotacaoReferencia = Quaternion.identity;
    private bool referenciaCapturada;
    private int marcadoresRastreados;

    public bool PossuiPoseConfiavel => poseInicializada && marcadoresRastreados >= minimoMarcadoresVisiveis;
    public bool PossuiReferencia => referenciaCapturada;
    public int MarcadoresRastreados => marcadoresRastreados;
    public Vector3 PosicaoPlanoMundo => poseAtual.position;
    public Quaternion RotacaoPlanoMundo => poseAtual.rotation;
    public Vector3 NormalPlanoMundo => ObterEixoNormal(poseAtual.rotation);
    public Vector3 NormalCalibradaMundo => referenciaCapturada ? ObterEixoNormal(poseAtual.rotation * Quaternion.Inverse(rotacaoReferencia)) : NormalPlanoMundo;

    void Update()
    {
        AtualizarPosePlano();

        if (calibrarAutomaticamenteNoPrimeiroTracking && PossuiPoseConfiavel && !referenciaCapturada)
        {
            CapturarReferenciaAtual();
        }
    }

    public void CapturarReferenciaAtual()
    {
        if (!poseInicializada)
        {
            return;
        }

        rotacaoReferencia = poseAtual.rotation;
        referenciaCapturada = true;
    }

    public void LimparReferencia()
    {
        referenciaCapturada = false;
        rotacaoReferencia = Quaternion.identity;
    }

    public Vector3 ObterGravidadeProjetada(bool usarReferenciaCalibrada = true)
    {
        Vector3 normal = usarReferenciaCalibrada ? NormalCalibradaMundo : NormalPlanoMundo;
        return Vector3.ProjectOnPlane(Physics.gravity, normal);
    }

    public float ObterInclinacaoGraus(bool usarReferenciaCalibrada = true)
    {
        Vector3 normal = usarReferenciaCalibrada ? NormalCalibradaMundo : NormalPlanoMundo;
        return Vector3.Angle(normal, Vector3.up);
    }

    private void AtualizarPosePlano()
    {
        Vector3 somaPosicoes = Vector3.zero;
        Quaternion primeiraRotacao = Quaternion.identity;
        Vector4 acumuladorRotacao = Vector4.zero;
        float pesoTotal = 0f;

        marcadoresRastreados = 0;

        if (marcadores == null || marcadores.Length == 0)
        {
            return;
        }

        for (int indice = 0; indice < marcadores.Length; indice++)
        {
            MarcadorPlano marcador = marcadores[indice];
            if (marcador.observer == null || !EstaRastreado(marcador.observer.TargetStatus))
            {
                continue;
            }

            float peso = Mathf.Max(0.01f, marcador.peso <= 0f ? 1f : marcador.peso);
            Pose poseCandidata = CalcularPosePlanoAPartirDoMarcador(marcador);

            somaPosicoes += poseCandidata.position * peso;
            AcumularRotacao(poseCandidata.rotation, peso, ref primeiraRotacao, ref acumuladorRotacao, pesoTotal <= 0f);
            pesoTotal += peso;
            marcadoresRastreados++;
        }

        if (marcadoresRastreados < minimoMarcadoresVisiveis || pesoTotal <= 0f)
        {
            return;
        }

        Vector3 posicaoMedia = somaPosicoes / pesoTotal;
        Quaternion rotacaoMedia = NormalizarRotacao(acumuladorRotacao, primeiraRotacao);

        if (!poseInicializada)
        {
            poseAtual = new Pose(posicaoMedia, rotacaoMedia);
            poseInicializada = true;
        }
        else
        {
            float tPosicao = FatorInterpolacao(suavizacaoPosicao, Time.deltaTime);
            float tRotacao = FatorInterpolacao(suavizacaoRotacao, Time.deltaTime);

            poseAtual.position = Vector3.Lerp(poseAtual.position, posicaoMedia, tPosicao);
            poseAtual.rotation = Quaternion.Slerp(poseAtual.rotation, rotacaoMedia, tRotacao);
        }

        if (atualizarTransformDoObjeto)
        {
            transform.SetPositionAndRotation(poseAtual.position, poseAtual.rotation);
        }
    }

    private Pose CalcularPosePlanoAPartirDoMarcador(MarcadorPlano marcador)
    {
        Matrix4x4 mundoParaMarcador = Matrix4x4.TRS(
            marcador.observer.transform.position,
            marcador.observer.transform.rotation,
            Vector3.one);

        Matrix4x4 planoParaMarcador = Matrix4x4.TRS(
            marcador.posicaoLocalNoPlano,
            Quaternion.Euler(marcador.rotacaoLocalNoPlanoEuler),
            Vector3.one);

        Matrix4x4 mundoParaPlano = mundoParaMarcador * planoParaMarcador.inverse;
        return new Pose(mundoParaPlano.MultiplyPoint3x4(Vector3.zero), mundoParaPlano.rotation);
    }

    private Vector3 ObterEixoNormal(Quaternion rotacaoBase)
    {
        return eixoNormalPlano switch
        {
            EixoNormalPlano.Forward => rotacaoBase * Vector3.forward,
            EixoNormalPlano.Right => rotacaoBase * Vector3.right,
            _ => rotacaoBase * Vector3.up
        };
    }

    private bool EstaRastreado(TargetStatus status)
    {
        if (status.Status == Status.TRACKED)
        {
            return true;
        }

        return filtroTracking == FiltroTracking.TrackedAndExtendedTracked && status.Status == Status.EXTENDED_TRACKED;
    }

    private static float FatorInterpolacao(float suavizacao, float deltaTime)
    {
        if (suavizacao <= 0f)
        {
            return 1f;
        }

        return 1f - Mathf.Exp(-suavizacao * deltaTime);
    }

    private static void AcumularRotacao(Quaternion rotacao, float peso, ref Quaternion primeiraRotacao, ref Vector4 acumulador, bool definirReferencia)
    {
        if (definirReferencia)
        {
            primeiraRotacao = rotacao;
        }

        float alinhamento = Quaternion.Dot(primeiraRotacao, rotacao) >= 0f ? 1f : -1f;
        acumulador.x += rotacao.x * alinhamento * peso;
        acumulador.y += rotacao.y * alinhamento * peso;
        acumulador.z += rotacao.z * alinhamento * peso;
        acumulador.w += rotacao.w * alinhamento * peso;
    }

    private static Quaternion NormalizarRotacao(Vector4 acumulador, Quaternion fallback)
    {
        float magnitude = Mathf.Sqrt(
            acumulador.x * acumulador.x +
            acumulador.y * acumulador.y +
            acumulador.z * acumulador.z +
            acumulador.w * acumulador.w);

        if (magnitude <= Mathf.Epsilon)
        {
            return fallback;
        }

        return new Quaternion(
            acumulador.x / magnitude,
            acumulador.y / magnitude,
            acumulador.z / magnitude,
            acumulador.w / magnitude);
    }
}