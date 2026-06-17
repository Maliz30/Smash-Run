using UnityEngine;
using TMPro; // Necessário para mexer no TextMeshPro da interface

public class PlayerHealth : MonoBehaviour
{
    [Header("Contagem de Vidas")]
    [SerializeField] private int totalVidas = 3; 
    private int vidasRestantes;

    [Header("Interface (UI)")]
    [SerializeField] private TextMeshProUGUI textoVidas; // Arraste o seu texto da tela aqui

    [Header("Áudio de Dano")]
    [SerializeField] private AudioClip painSound; // Arraste o .mp3 do grito aqui
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

    // Essa é a função que o HammerController vai chamar ao detectar o impacto
    public void TakeDamage(int damage)
    {
        if (vidasRestantes <= 0) return;

        vidasRestantes -= 1; // Perde 1 vida por martelada
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
        Debug.Log("O Player do teclado perdeu todas as vidas! Vitória do VR!");
    }
}