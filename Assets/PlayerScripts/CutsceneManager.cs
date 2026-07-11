using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Obrigatório para controlar o Botão

public class CutsceneManager : MonoBehaviour
{
    [Header("Câmeras dos Quadrinhos")]
    [SerializeField] private CinemachineCamera[] camerasDosQuadros; // Arraste as 4 câmeras individuais (0 a 3)
    [SerializeField] private CinemachineCamera cameraGeral; // Arraste a 5ª câmera (Visão Geral) aqui
    [SerializeField] private float tempoEmCadaQuadro = 4.0f; 

    [Header("Configurações de Áudio")]
    [SerializeField] private AudioSource somNarracao; 
    [SerializeField] private AudioClip audioAlegre;  
    [SerializeField] private AudioClip audioShockTransicao; 
    [SerializeField] private AudioClip audioTenso;   

    [Header("Interface de Fim da Cutscene")]
    [SerializeField] private GameObject botaoIniciarJogo; // Arraste o botão que criamos aqui

    private int quadroAtual = 0;
    private bool cutsceneFinalizada = false;

    void Start()
    {
        if (botaoIniciarJogo != null) botaoIniciarJogo.SetActive(false); // Garante que o botão começa escondido
        
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
        
        // ALTERAÇÃO AQUI: Mudado de 1.0f para 2.0f para o efeito sonoro de choque durar mais
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
        
        // Desativa as prioridades das outras e ativa a câmera geral
        DesativarTodasAsCameras();
        if (cameraGeral != null) cameraGeral.Priority = 15;

        // Espera a câmera terminar de deslizar para trás antes de mostrar o botão (da um efeito lindo!)
        yield return new WaitForSeconds(1.2f);

        // Exibe o botão na tela para os jogadores iniciarem a partida
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

    // Função que será vinculada ao OnClick() do novo botão de Iniciar Jogo
    public void AvançarParaCenaPrincipal()
    {
        Debug.Log("[Cutscene] Botão clicado! Salvando destino e indo para a tela de carregamento...");

        PlayerPrefs.SetString("CenaParaCarregar", "CenaPrincipal");
        SceneManager.LoadScene("cenaCarregamento");
    }
}