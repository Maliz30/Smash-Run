using UnityEngine;

[RequireComponent(typeof(VuforiaPlanoMultiMarcador))]
[RequireComponent(typeof(VuforiaFisicaPlanoInclinado))]
public class VuforiaInstanciadorModeloPlano : MonoBehaviour
{
    [Header("Prefab fisico")]
    [SerializeField] private GameObject prefabModeloFisico;

    [Header("Posicionamento inicial")]
    [SerializeField] private Vector3 deslocamentoLocal;
    [SerializeField] private Vector3 rotacaoLocalEuler;

    [Header("Ciclo de vida")]
    [SerializeField] private bool instanciarSomenteUmaVez = true;
    [SerializeField] private bool destruirAoPerderTracking;

    private VuforiaPlanoMultiMarcador planoMultiMarcador;
    private VuforiaFisicaPlanoInclinado fisicaPlano;
    private GameObject instanciaAtual;
    private Rigidbody rigidbodyAtual;
    private bool jaInstanciou;

    void Awake()
    {
        planoMultiMarcador = GetComponent<VuforiaPlanoMultiMarcador>();
        fisicaPlano = GetComponent<VuforiaFisicaPlanoInclinado>();
    }

    void Update()
    {
        if (planoMultiMarcador.PossuiPoseConfiavel)
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
            Debug.LogWarning("VuforiaInstanciadorModeloPlano precisa de um prefab fisico atribuido.", this);
            return;
        }

        Vector3 posicaoInicial = planoMultiMarcador.PosicaoPlanoMundo + planoMultiMarcador.RotacaoPlanoMundo * deslocamentoLocal;
        Quaternion rotacaoInicial = planoMultiMarcador.RotacaoPlanoMundo * Quaternion.Euler(rotacaoLocalEuler);

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

        fisicaPlano.DefinirCorpoAlvo(rigidbodyAtual);
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
        fisicaPlano.DefinirCorpoAlvo(null);
    }
}