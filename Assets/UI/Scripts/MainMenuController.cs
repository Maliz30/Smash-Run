using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Configurações de Transição")]
    [Tooltip("Nome exato da cena de Loading")]
    [SerializeField] private string loadingSceneName = "LoadingScene";

    /// <summary>
    /// Acionado pelo botão principal "Iniciar".
    /// </summary>
    public void OnPlayButtonClicked()
    {
        Debug.Log("[MainMenuController] Iniciando transição para LoadingScene...");
        SceneManager.LoadScene(loadingSceneName);
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
        Debug.Log("[MainMenuController] Encerrando aplicação.");
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
