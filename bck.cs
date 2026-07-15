using System.Collections;
using UnityEngine;
using Unity.Cinemachine; // Namespace obrigatório para o Cinemachine da Unity 6
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    [Header("Câmeras dos Quadrinhos")]
    [SerializeField] private CinemachineCamera[] camerasDosQuadros; // Arraste as 4 câmeras na ordem (0 a 3)
    [SerializeField] private float tempoEmCadaQuadro = 3.5f; // Quanto tempo a câmera fica parada em cada quadrinho

    [Header("Áudio")]
    [SerializeField] private AudioSource somNarracao;
    [SerializeField] private AudioClip[] trilhaPorQuadro; // Adicione os SFX (nariz de palhaço, martelo, etc) conforme o SGDD

    private int quadroAtual = 0;
    private Coroutine cutsceneCoroutine;

    void Start()
    {
        // Garante que apenas a primeira câmera está ativa no início
        VisualizarQuadro(0);
        cutsceneCoroutine = StartCoroutine(ExecutarSequenciaHQ());
    }

    IEnumerator ExecutarSequenciaHQ()
    {
        while (quadroAtual < camerasDosQuadros.Length)
        {
            // Toca os efeitos sonoros específicos de cada quadro definidos no SGDD
            if (somNarracao != null && trilhaPorQuadro.Length > quadroAtual && trilhaPorQuadro[quadroAtual] != null)
            {
                somNarracao.PlayOneShot(trilhaPorQuadro[quadroAtual]);
            }

            // Espera o tempo determinado pro jogador ler o quadrinho
            yield return new WaitForSeconds(tempoEmCadaQuadro);
            
            quadroAtual++;

            if (quadroAtual < camerasDosQuadros.Length)
            {
                VisualizarQuadro(quadroAtual);
            }
        }

        // Fim da HQ -> Avança para a tela de instruções de controles (Semana 6 do SGDD)
        CarregarProximaCena();
    }

    void VisualizarQuadro(int indice)
    {
        // Desativa todas as câmeras e ativa apenas a do índice atual.
        // O Cinemachine da Unity 6 fará a transição de deslize automaticamente!
        for (int i = 0; i < camerasDosQuadros.Length; i++)
        {
            if (camerasDosQuadros[i] != null)
            {
                camerasDosQuadros[i].gameObject.SetActive(i == indice);
            }
        }
    }

    void Update()
    {
        // Se o jogador apertar Espaço ou clicar com o mouse, pula para o próximo quadro imediatamente
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            PularParaProximo();
        }
    }

    void PularParaProximo()
    {
        if (cutsceneCoroutine != null) StopCoroutine(cutsceneCoroutine);
        
        quadroAtual++;
        
        if (quadroAtual < camerasDosQuadros.Length)
        {
            VisualizarQuadro(quadroAtual);
            cutsceneCoroutine = StartCoroutine(GerenciarEsperaSimples());
        }
        else
        {
            CarregarProximaCena();
        }
    }

    IEnumerator GerenciarEsperaSimples()
    {
        yield return new WaitForSeconds(tempoEmCadaQuadro);
        PularParaProximo();
    }

    void CarregarProximaCena()
    {
        // Coloque aqui o nome exato da sua cena de instruções de controles
        SceneManager.LoadScene("TelaInstrucoes"); 
    }
}