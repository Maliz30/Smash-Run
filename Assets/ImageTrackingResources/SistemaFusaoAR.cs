using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class SistemaFusaoAR : MonoBehaviour
{
    [Header("Modelos 3D")]
    public GameObject prefabVermelho;
    public GameObject prefabAzul;
    public GameObject prefabRoxo;

    [Header("Configuracao")]
    [Tooltip("Distancia em metros para acontecer a fusao (ex: 0.15 = 15cm)")]
    public float distanciaParaJuntar = 0.15f;

    private ARTrackedImageManager manager;
    private GameObject instanciaVermelha;
    private GameObject instanciaAzul;
    private GameObject instanciaRoxa;

    private ARTrackedImage marcadorVermelho;
    private ARTrackedImage marcadorAzul;

    void Awake()
    {
        manager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable() => manager.trackablesChanged.AddListener(AoMudarRastreamento);
    void OnDisable() => manager.trackablesChanged.RemoveListener(AoMudarRastreamento);

    void AoMudarRastreamento(ARTrackablesChangedEventArgs<ARTrackedImage> evento)
    {
        foreach (var imagem in evento.added)
        {
            if (imagem.referenceImage.name == "QR_Vermelho")
            {
                marcadorVermelho = imagem;
                if (instanciaVermelha == null)
                    instanciaVermelha = Instantiate(prefabVermelho, imagem.transform);
            }
            else if (imagem.referenceImage.name == "QR_Azul")
            {
                marcadorAzul = imagem;
                if (instanciaAzul == null)
                    instanciaAzul = Instantiate(prefabAzul, imagem.transform);
            }
        }
    }

    void Update()
    {
        if (marcadorVermelho != null && marcadorAzul != null &&
            marcadorVermelho.trackingState == TrackingState.Tracking &&
            marcadorAzul.trackingState == TrackingState.Tracking)
        {
            float distancia = Vector3.Distance(marcadorVermelho.transform.position, marcadorAzul.transform.position);

            if (distancia <= distanciaParaJuntar)
            {
                if (instanciaRoxa == null)
                {
                    Vector3 meio = (marcadorVermelho.transform.position + marcadorAzul.transform.position) / 2f;
                    instanciaRoxa = Instantiate(prefabRoxo, meio, Quaternion.identity);
                }
                else
                {
                    instanciaRoxa.transform.position = (marcadorVermelho.transform.position + marcadorAzul.transform.position) / 2f;
                    if (!instanciaRoxa.activeSelf) instanciaRoxa.SetActive(true);
                }

                if (instanciaVermelha.activeSelf) instanciaVermelha.SetActive(false);
                if (instanciaAzul.activeSelf) instanciaAzul.SetActive(false);
            }
            else
            {
                if (!instanciaVermelha.activeSelf) instanciaVermelha.SetActive(true);
                if (!instanciaAzul.activeSelf) instanciaAzul.SetActive(true);

                if (instanciaRoxa != null && instanciaRoxa.activeSelf) instanciaRoxa.SetActive(false);
            }
        }
        else
        {
            if (instanciaRoxa != null && instanciaRoxa.activeSelf) instanciaRoxa.SetActive(false);
        }
    }
}
