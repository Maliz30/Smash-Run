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
        StartCoroutine(LoadMainSceneAsync());
    }

    private IEnumerator LoadMainSceneAsync()
    {
        // Inicia o carregamento em background
        AsyncOperation operation = SceneManager.LoadSceneAsync(mainSceneName);
        
        // Impede que a cena mude imediatamente para permitir transições visuais se desejado
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

            // Quando atinge 0.9, o carregamento terminou, restando apenas a ativação
            if (operation.progress >= 0.9f)
            {
                // Pode-se adicionar um tempo extra de espera ou "Pressione qualquer botão"
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
