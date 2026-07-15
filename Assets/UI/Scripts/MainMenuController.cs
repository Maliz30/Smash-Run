using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    void Awake()
    {
        // Força a ativação do Display 2 no PC se ele for detectado
        if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate();
            Debug.Log("[Display] Display 2 ativado com sucesso para o jogador do PC!");
        }
    }
    
    [Header("Configurações de Transição")]
    [Tooltip("Nome exato da cena de Loading no Build Profiles")]
    [SerializeField] private string loadingSceneName = "CenaCarregamento";
    
    [Tooltip("Nome exato da cena dos quadrinhos/história")]
    [SerializeField] private string cenaDestinoName = "CenaHistoria";

    [Header("Referências de Áudio (Segurança)")]
    [Tooltip("Arraste o componente AudioSource do botão aqui")]
    [SerializeField] private AudioSource somBotao; 

    [Tooltip("Arraste o componente AudioSource geral de cliques aqui")]
    [SerializeField] private AudioSource somClique;

    [Tooltip("Arraste o AudioClip do som de transição aqui")]
    [SerializeField] private AudioClip somTransicao;

    /// <summary>
    /// Acionado pelo botão principal "Iniciar".
    /// </summary>
    public void OnPlayButtonClicked()
    {
        Debug.Log("[MainMenuController] Botão Iniciar clicado. Iniciando transição...");
        
        // Toca o som de clique imediatamente se ele estiver configurado
        if (somClique != null)
        {
            somClique.Play();
        }

        // Inicia a rotina de espera e som de transição
        StartCoroutine(EsperarSomECarregar());
    }

    /// <summary>
    /// Acionado pelo botão "Configurações".
    /// </summary>
    public void OnSettingsButtonClicked()
    {
        if (somClique != null) somClique.Play();
        Debug.Log("[MainMenuController] Botão de configurações acionado.");
    }

    /// <summary>
    /// Acionado pelo botão secundário "Sair".
    /// </summary>
    public void OnQuitButtonClicked()
    {
        Debug.Log("[MainMenuController] Botão Sair clicado. Iniciando contagem regressiva para fechar...");
        
        if (somClique != null) somClique.Play();

        // Inicia a rotina de espera para o som poder tocar
        StartCoroutine(EsperarSomESair());
    }

    private IEnumerator EsperarSomECarregar()
    {
        // Toca o efeito de transição se o canal e o clipe existirem
        if (somBotao != null && somTransicao != null)
        {
            somBotao.PlayOneShot(somTransicao);
            // Aguarda o tempo de duração exato do clipe de áudio
            yield return new WaitForSeconds(somTransicao.length);
        }
        else
        {
            // Fallback de segurança se não houver som: aguarda um tempo fixo padrão
            yield return new WaitForSeconds(1.5f);
        }

        Debug.Log("[MainMenuController] Transição concluída. Salvando destino e indo para o Loading...");
        
        // SALVA NA MEMÓRIA: Avisa para a tela de loading qual cena abrir depois
        PlayerPrefs.SetString("CenaParaCarregar", cenaDestinoName);
        PlayerPrefs.Save(); // Força o salvamento físico no registro imediatamente
        
        // Carrega a tela de loading
        SceneManager.LoadScene(loadingSceneName);
    }

    private IEnumerator EsperarSomESair()
    {
        // Aguarda um pequeno momento para o som de clique terminar de tocar
        yield return new WaitForSeconds(0.8f);

        Debug.Log("[MainMenuController] Tempo esgotado. Encerrando aplicação.");

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}