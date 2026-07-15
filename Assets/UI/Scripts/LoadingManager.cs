using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    [Header("Configurações de Cena")]
    [Tooltip("Nome exato da cena principal do jogo a ser carregada")]
    [SerializeField] private string mainSceneName = "CenaPrincipal";

    [Header("Referências de UI")]
    [SerializeField] private Image progressBarFill;
    [SerializeField] private TextMeshProUGUI progressText;

    private void Start()
    {
        // CORREÇÃO ESSENCIAL: 
        // Busca na memória qual cena deve ser carregada. 
        // Se não encontrar nada (por segurança), usa o valor padrão que está no Inspector.
        string cenaParaCarregar = PlayerPrefs.GetString("CenaParaCarregar", mainSceneName);
        
        Debug.Log("[LoadingManager] Iniciando carregamento assíncrono para: " + cenaParaCarregar);

        // Passa o nome dinâmico da cena para a Corrotina
        StartCoroutine(LoadMainSceneAsync(cenaParaCarregar));
    }

    // Adicionamos o parâmetro 'string nomeCena' aqui na assinatura do método
    private IEnumerator LoadMainSceneAsync(string nomeCena)
    {
        // Inicia o carregamento em background usando o nome dinâmico vindo da memória
        AsyncOperation operation = SceneManager.LoadSceneAsync(nomeCena);
        
        // Impede que a cena mude imediatamente para permitir transições visuais
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // O progresso de load do Unity vai de 0 a 0.9. Mapeamos isso para 0 a 1.
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (progressBarFill != null)
            {
                progressBarFill.fillAmount = progress;
            }

            if (progressText != null)
            {
                progressText.text = $"Carregando... {(progress * 100):0}%";
            }

            // Quando atinge 0.9, o carregamento terminou no background, restando apenas a ativação
            // Quando atinge 0.9, o carregamento de arquivos terminou
            if (operation.progress >= 0.9f)
            {
                // Força o texto a mostrar 100% e a barra encher totalmente
                if (progressBarFill != null) progressBarFill.fillAmount = 1f;
                if (progressText != null) progressText.text = "Carregando... 100%";

                // DÁ UM RESPIRO: Espera meio segundo para a Unity estabilizar a memória
                yield return new WaitForSeconds(0.5f);

                // Libera a ativação da cena
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
