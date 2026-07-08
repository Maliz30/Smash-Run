// using System.Collections;
// using UnityEngine;
// using Unity.Cinemachine; // Namespace obrigatório para o Cinemachine da Unity 6
// using UnityEngine.SceneManagement;

// public class CutsceneManager : MonoBehaviour
// {
//     [Header("Câmeras dos Quadrinhos")]
//     [SerializeField] private CinemachineCamera[] camerasDosQuadros; // Arraste as 4 câmeras na ordem (0 a 3)
//     [SerializeField] private float tempoEmCadaQuadro = 3.5f; // Quanto tempo a câmera fica parada em cada quadrinho

//     [Header("Configurações de Áudio")]
//     [SerializeField] private AudioSource somNarracao; // O AudioSource do seu gerenciador
//     [SerializeField] private AudioClip audioAlegre;  // Waltz of the Carnies
//     [SerializeField] private AudioClip audioShockTransicao; // shock-funny-version
//     [SerializeField] private AudioClip audioTenso;   // Circus of Freaks

//     private int quadroAtual = 0;
//     private Coroutine cutsceneCoroutine;

//     void Start()
//     {
//         // Garante que apenas a primeira câmera está ativa no início
//         VisualizarQuadro(0);

//         // Inicia a música alegre em Loop
//         if (somNarracao != null && audioAlegre != null)
//         {
//             somNarracao.clip = audioAlegre;
//             somNarracao.loop = true;
//             somNarracao.Play();
//         }

//         cutsceneCoroutine = StartCoroutine(ExecutarSequenciaHQ());
//     }

//     IEnumerator ExecutarSequenciaHQ()
//     {
//         // --- QUADRO 1 ---
//         yield return new WaitForSeconds(tempoEmCadaQuadro);
        
//         // --- QUADRO 2 ---
//         quadroAtual = 1;
//         VisualizarQuadro(quadroAtual);
//         yield return new WaitForSeconds(tempoEmCadaQuadro);
        
//         // --- QUADRO 3 (A transição acontece aqui - Ajustado para estender a música alegre e reduzir o choque) ---
//         quadroAtual = 2;
//         VisualizarQuadro(quadroAtual);

//         // 1. Deixa a música alegre (Waltz of the Carnies) tocando por mais 2 segundos dentro deste quadro
//         yield return new WaitForSeconds(2.0f);

//         // 2. AGORA SIM: Interrompe a música alegre e toca o efeito de choque cômico
//         if (somNarracao != null && audioShockTransicao != null)
//         {
//             somNarracao.Stop();
//             somNarracao.loop = false;
//             somNarracao.PlayOneShot(audioShockTransicao);
//         }

//         // 3. Espera apenas 1 segundo (os 3 segundos originais menos os 2 que demos para a música alegre)
//         yield return new WaitForSeconds(1.0f);

//         // 4. Entra imediatamente a música tensa do robô (Circus of Freaks) em loop
//         if (somNarracao != null)
//         {
//             somNarracao.Stop(); 
//         }

//         // 5. Espera o 1 segundo restante para fechar os 4 segundos totais do Quadro 3
//         // (2.0s alegre + 1.0s choque + 1.0s tenso = 4.0s)
//         float tempoRestanteQuadro3 = Mathf.Max(0.1f, tempoEmCadaQuadro - 3.0f); 
//         yield return new WaitForSeconds(tempoRestanteQuadro3);

//         // --- QUADRO 4 ---
//         quadroAtual = 3;
//         VisualizarQuadro(quadroAtual);
//         // A música tensa continua tocando de fundo aqui!
//         yield return new WaitForSeconds(tempoEmCadaQuadro);

//         // Fim da HQ -> Avança para a tela de instruções de controles (Semana 6 do SGDD)
//         CarregarProximaCena();
//     }

//     void VisualizarQuadro(int indice)
//     {
//         for (int i = 0; i < camerasDosQuadros.Length; i++)
//         {
//             if (camerasDosQuadros[i] != null)
//             {
//                 camerasDosQuadros[i].gameObject.SetActive(i == indice);
//             }
//         }
//     }

//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
//         {
//             CarregarProximaCena();
//         }
//     }

//     void CarregarProximaCena()
//     {
//         SceneManager.LoadScene("TelaInstrucoes"); 
//     }
// }

using System.Collections;
using UnityEngine;
using Unity.Cinemachine; // Namespace obrigatório para o Cinemachine da Unity 6
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    [Header("Câmeras dos Quadrinhos")]
    [SerializeField] private CinemachineCamera[] camerasDosQuadros; // Arraste as 4 câmeras na ordem (0 a 3)
    [SerializeField] private float tempoEmCadaQuadro = 3.5f; // Quanto tempo a câmera fica parada em cada quadrinho

    [Header("Configurações de Áudio")]
    [SerializeField] private AudioSource somNarracao; // O AudioSource do seu gerenciador
    [SerializeField] private AudioClip audioAlegre;  // Waltz of the Carnies
    [SerializeField] private AudioClip audioShockTransicao; // shock-funny-version
    [SerializeField] private AudioClip audioTenso;   // Circus of Freaks

    private int quadroAtual = 0;
    private Coroutine cutsceneCoroutine;

    void Start()
    {
        // Garante que apenas a primeira câmera está ativa no início
        VisualizarQuadro(0);

        // Inicia a música alegre em Loop
        if (somNarracao != null && audioAlegre != null)
        {
            somNarracao.clip = audioAlegre;
            somNarracao.loop = true;
            somNarracao.Play();
        }

        cutsceneCoroutine = StartCoroutine(ExecutarSequenciaHQ());
    }

    IEnumerator ExecutarSequenciaHQ()
    {
        // --- QUADRO 1 ---
        yield return new WaitForSeconds(tempoEmCadaQuadro);
        
        // --- QUADRO 2 ---
        quadroAtual = 1;
        VisualizarQuadro(quadroAtual);
        yield return new WaitForSeconds(tempoEmCadaQuadro);
        
        // --- QUADRO 3 (A transição acontece aqui) ---
        quadroAtual = 2;
        VisualizarQuadro(quadroAtual);

        // 1. Deixa a música alegre (Waltz of the Carnies) tocando por mais 2 segundos dentro deste quadro
        yield return new WaitForSeconds(2.0f);

        // 2. Interrompe a música alegre e toca o efeito de choque cômico
        if (somNarracao != null && audioShockTransicao != null)
        {
            somNarracao.Stop();
            somNarracao.loop = false;
            somNarracao.PlayOneShot(audioShockTransicao);
        }

        // 3. Espera exatamente 1 segundo com o choque cômico rodando sozinho
        yield return new WaitForSeconds(1.0f);

        // 4. CORREÇÃO: Garante a interrupção forçada do áudio de choque antes de entrar a música tensa
        if (somNarracao != null)
        {
            somNarracao.Stop(); 
        }

        // 5. Entra imediatamente a música tensa do robô (Circus of Freaks) em loop limpo
        if (somNarracao != null && audioTenso != null)
        {
            somNarracao.clip = audioTenso;
            somNarracao.loop = true;
            somNarracao.Play();
        }

        // 6. Espera o 1 segundo restante para fechar os 4 segundos totais do Quadro 3
        float tempoRestanteQuadro3 = Mathf.Max(0.1f, tempoEmCadaQuadro - 3.0f); 
        yield return new WaitForSeconds(tempoRestanteQuadro3);

        // --- QUADRO 4 ---
        quadroAtual = 3;
        VisualizarQuadro(quadroAtual);
        // Agora a música tensa continua tocando de fundo de forma 100% limpa, sem o choque atrapalhando!
        yield return new WaitForSeconds(tempoEmCadaQuadro);

        // Fim da HQ -> Avança para a tela de instruções de controles
        CarregarProximaCena();
    }

    void VisualizarQuadro(int indice)
    {
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
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            CarregarProximaCena();
        }
    }

    void CarregarProximaCena()
    {
        SceneManager.LoadScene("TelaInstrucoes"); 
    }
}