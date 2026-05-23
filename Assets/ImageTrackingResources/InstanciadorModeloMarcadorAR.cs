using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImage))]
[RequireComponent(typeof(FisicaInclinacaoMarcador))]
public class InstanciadorModeloMarcadorAR : MonoBehaviour
{
    [Header("Prefab fisico")]
    [SerializeField] private GameObject prefabModeloFisico;

    [Header("Posicionamento inicial")]
    [SerializeField] private Vector3 deslocamentoLocal;
    [SerializeField] private Vector3 rotacaoLocalEuler;

    [Header("Ciclo de vida")]
    [SerializeField] private bool instanciarSomenteUmaVez = true;
    [SerializeField] private bool destruirAoPerderTracking;

    private ARTrackedImage imagemRastreada;
    private FisicaInclinacaoMarcador fisicaMarcador;
    private GameObject instanciaAtual;
    private Rigidbody rigidbodyAtual;
    private bool jaInstanciou;

    void Awake()
    {
        imagemRastreada = GetComponent<ARTrackedImage>();
        fisicaMarcador = GetComponent<FisicaInclinacaoMarcador>();
    }

    void Update()
    {
        if (imagemRastreada.trackingState == TrackingState.Tracking)
        {
            if (instanciaAtual == null && (!instanciarSomenteUmaVez || !jaInstanciou))
            {
                InstanciarModelo();
            }

            return;
        }

        if (destruirAoPerderTracking)
        {
            RemoverInstanciaAtual();
        }
    }

    private void InstanciarModelo()
    {
        if (prefabModeloFisico == null)
        {
            Debug.LogWarning("InstanciadorModeloMarcadorAR precisa de um prefab fisico atribuido.", this);
            return;
        }

        Vector3 posicaoInicial = transform.TransformPoint(deslocamentoLocal);
        Quaternion rotacaoInicial = transform.rotation * Quaternion.Euler(rotacaoLocalEuler);

        instanciaAtual = Instantiate(prefabModeloFisico, posicaoInicial, rotacaoInicial);
        rigidbodyAtual = instanciaAtual.GetComponent<Rigidbody>();

        if (rigidbodyAtual == null)
        {
            rigidbodyAtual = instanciaAtual.GetComponentInChildren<Rigidbody>();
        }

        if (rigidbodyAtual == null)
        {
            Debug.LogWarning("O prefab fisico precisa ter um Rigidbody na raiz ou em um filho.", this);
            Destroy(instanciaAtual);
            instanciaAtual = null;
            return;
        }

        fisicaMarcador.DefinirCorpoAlvo(rigidbodyAtual);
        jaInstanciou = true;
    }

    private void RemoverInstanciaAtual()
    {
        if (instanciaAtual == null)
        {
            return;
        }

        Destroy(instanciaAtual);
        instanciaAtual = null;
        rigidbodyAtual = null;
        fisicaMarcador.DefinirCorpoAlvo(null);
    }
}