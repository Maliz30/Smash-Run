using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
	[Header("Configurações de Transição")]
	[Tooltip("Nome exato da cena de loading no Build Settings")]
	[SerializeField] private string loadingSceneName = "cenaCarregamento";

	[Tooltip("Nome exato da cena principal do jogo")]
	[SerializeField] private string cenaPrincipalName = "CenaPrincipal";

	[Tooltip("Nome exato da cena do menu principal")]
	[SerializeField] private string menuSceneName = "CenaMenu";

	public void ReiniciarJogo()
	{
		GameFlowManager.ResetMatchState();
		StartCoroutine(CarregarCenaComLoading(cenaPrincipalName));
	}

	public void VoltarAoMenu()
	{
		GameFlowManager.ResetMatchState();
		StartCoroutine(CarregarCenaComLoading(menuSceneName));
	}

	private IEnumerator CarregarCenaComLoading(string nomeCenaDestino)
	{
		PlayerPrefs.SetString("CenaParaCarregar", nomeCenaDestino);
		yield return null;
		SceneManager.LoadScene(loadingSceneName);
	}
}
