using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Contagem de Vidas")]
    [SerializeField] private int totalVidas = 3; 
    private int vidasRestantes;

    [Header("Interface (UI)")]
    [SerializeField] private TextMeshProUGUI textoVidas; 
    [Header("Áudio de Dano")]
    [SerializeField] private AudioClip painSound; 
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        vidasRestantes = totalVidas;
        AtualizarTextoInterface();
    }

    public void TakeDamage(int damage)
    {
        if (vidasRestantes <= 0 || GameFlowManager.IsMatchEnding) return;

        vidasRestantes -= 1; 
        AtualizarTextoInterface();

        if (audioSource != null && painSound != null)
        {
            audioSource.PlayOneShot(painSound);
        }

        Debug.Log($"O martelo acertou o player! Vidas restantes: {vidasRestantes}");

        if (vidasRestantes <= 0)
        {
            Die();
        }
    }

    private void AtualizarTextoInterface()
    {
        if (textoVidas != null)
        {
            textoVidas.text = "Vidas: " + vidasRestantes;
        }
    }

    private void Die()
    {
        if (textoVidas != null)
        {
            textoVidas.text = "GAME OVER!";
        }

        Debug.Log("Você Perdeu! Indo para a tela de Game Over...");
        GameFlowManager.RequestGameOver();
    }
}