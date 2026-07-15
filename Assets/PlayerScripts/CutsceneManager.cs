using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class CutsceneManager : MonoBehaviour
{
    [Header("Câmeras dos Quadrinhos")]
    [SerializeField] private CinemachineCamera[] camerasDosQuadros; 
    [SerializeField] private CinemachineCamera cameraGeral; 
    [SerializeField] private float tempoEmCadaQuadro = 4.0f; 

    [Header("Configurações de Áudio")]
    [SerializeField] private AudioSource somNarracao; 
    [SerializeField] private AudioClip audioAlegre;  
    [SerializeField] private AudioClip audioShockTransicao; 
    [SerializeField] private AudioClip audioTenso;   

    [Header("Interface de Fim da Cutscene")]
    [SerializeField] private GameObject botaoIniciarJogo; 

    private int quadroAtual = 0;
    private bool cutsceneFinalizada = false;

    void Start()
    {
        // CORREÇÃO: Força o botão a iniciar ATIVO e visível desde o segundo zero
        if (botaoIniciarJogo != null) 
        {
            botaoIniciarJogo.SetActive(true); 
        }
        
        VisualizarQuadro(0);

        if (somNarracao != null && audioAlegre != null)
        {
            somNarracao.clip = audioAlegre;
            somNarracao.loop = true;
            somNarracao.Play();
        }

        StartCoroutine(ExecutarSequenciaHQ());
    }

    IEnumerator ExecutarSequenciaHQ()
    {
        // --- QUADRO 1 ---
        yield return new WaitForSeconds(tempoEmCadaQuadro);
        
        // --- QUADRO 2 ---
        quadroAtual = 1;
        VisualizarQuadro(quadroAtual);
        yield return new WaitForSeconds(tempoEmCadaQuadro);
        
        // --- QUADRO 3 ---
        quadroAtual = 2;
        VisualizarQuadro(quadroAtual);
        yield return new WaitForSeconds(2.0f);

        if (somNarracao != null && audioShockTransicao != null)
        {
            somNarracao.Stop();
            somNarracao.loop = false;
            somNarracao.PlayOneShot(audioShockTransicao);
        }
        
        yield return new WaitForSeconds(2.0f);

        if (somNarracao != null) somNarracao.Stop(); 

        if (somNarracao != null && audioTenso != null)
        {
            somNarracao.clip = audioTenso;
            somNarracao.loop = true;
            somNarracao.Play();
        }
        yield return new WaitForSeconds(1.0f);

        // --- QUADRO 4 ---
        quadroAtual = 3;
        VisualizarQuadro(quadroAtual);
        yield return new WaitForSeconds(tempoEmCadaQuadro);

        // --- EFEITO ZOOM OUT (MOSTRAR TUDO) ---
        Debug.Log("[Cutscene] Tirando o zoom e mostrando todos os quadrinhos.");
        cutsceneFinalizada = true;
        
        DesativarTodasAsCameras();
        if (cameraGeral != null) cameraGeral.Priority = 15;

        yield return new WaitForSeconds(1.2f);

        // O botão permanece ativo aqui também como garantia final
        if (botaoIniciarJogo != null)
        {
            botaoIniciarJogo.SetActive(true);
        }
    }

    void VisualizarQuadro(int indice)
    {
        DesativarTodasAsCameras();
        if (camerasDosQuadros[indice] != null)
        {
            camerasDosQuadros[indice].Priority = 10;
        }
    }

    void DesativarTodasAsCameras()
    {
        for (int i = 0; i < camerasDosQuadros.Length; i++)
        {
            if (camerasDosQuadros[i] != null) camerasDosQuadros[i].Priority = 5;
        }
        if (cameraGeral != null) cameraGeral.Priority = 5;
    }

    public void AvançarParaCenaPrincipal()
    {
        Debug.Log("[Cutscene] Botão clicado! Salvando destino e indo para a tela de carregamento...");

        PlayerPrefs.SetString("CenaParaCarregar", "CenaPrincipal");
        PlayerPrefs.Save();
        SceneManager.LoadScene("CenaCarregamento");
    }
}