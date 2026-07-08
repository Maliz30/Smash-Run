using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Configurações de Transição")]
    [Tooltip("Nome exato da cena de Loading no Build Profiles")]
    [SerializeField] private string loadingSceneName = "TelaLoading"; // Certifique-se de que o nome está igual ao da sua cena
    
    [Tooltip("Nome exato da cena dos quadrinhos/história")]
    [SerializeField] private string cenaDestinoName = "CenaHistoria"; // Adicionamos esse campo para a história

    /// <summary>
    /// Acionado pelo botão principal "Iniciar".
    /// </summary>
    public void OnPlayButtonClicked()
    {
        Debug.Log("[MainMenuController] Salvando a cena de destino e indo para o Loading...");
        
        // SALVA NA MEMÓRIA: Avisa para a tela de loading que ela deve carregar a história depois
        PlayerPrefs.SetString("CenaParaCarregar", cenaDestinoName);
        
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