using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [Header("Configurações de Transição")]
    [Tooltip("Nome exato da cena de Loading no Build Profiles")]
    [SerializeField] private string loadingSceneName = "CenaCarregamento"; // Certifique-se de que o nome está igual ao da sua cena
    
    [Tooltip("Nome exato da cena dos quadrinhos/história")]
    [SerializeField] private string cenaDestinoName = "CenaHistoria"; // Adicionamos esse campo para a história

    /// <summary>
    /// Acionado pelo botão principal "Iniciar".
    /// </summary>
    public void OnPlayButtonClicked()
    {
        Debug.Log("[MainMenuController] Botão Iniciar clicado. Aguardando som...");
        
        // Em vez de carregar direto, inicia a rotina de espera
        StartCoroutine(EsperarSomECarregar());

        // Debug.Log("[MainMenuController] Salvando a cena de destino e indo para o Loading...");
        
        // // SALVA NA MEMÓRIA: Avisa para a tela de loading que ela deve carregar a história depois
        // PlayerPrefs.SetString("CenaParaCarregar", "CenaHistoria");
        // SceneManager.LoadScene("cenaCarregamento");
    }

    /// <summary>
    /// Acionado pelo botão "Configurações".
    /// </summary>
    public void OnSettingsButtonClicked()
    {
        // TODO: Implementar abertura do painel de configurações.
        Debug.Log("[MainMenuController] Botão de configurações acionado.");
    }

    /// <summary>
    /// Acionado pelo botão secundário "Sair".
    /// </summary>
    public void OnQuitButtonClicked()
    {
        Debug.Log("[MainMenuController] Botão Sair clicado. Iniciando contagem regressiva para fechar...");
        
        // Inicia a rotina de espera para o som poder tocar
        StartCoroutine(EsperarSomESair());
    }

    private IEnumerator EsperarSomECarregar()
    {
        yield return new WaitForSeconds(1.5f);

        Debug.Log("[MainMenuController] Tempo esgotado. Salvando destino e indo para o Loading...");
        
        // SALVA NA MEMÓRIA: Avisa para a tela de loading que ela deve carregar a história depois
        PlayerPrefs.SetString("CenaParaCarregar", cenaDestinoName);
        
        // Carrega a tela de loading
        SceneManager.LoadScene(loadingSceneName);
    }

    private IEnumerator EsperarSomESair()
    {
        // Aguarda exatamente 2.0 segundos com o jogo rodando
        yield return new WaitForSeconds(1.5f);

        Debug.Log("[MainMenuController] Tempo esgotado. Encerrando aplicação.");

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}