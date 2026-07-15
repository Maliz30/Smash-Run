using UnityEngine;
using TMPro;

using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI; // NECESSÁRIO PARA O IMAGE
using System.Collections.Generic; // NECESSÁRIO PARA A LISTA


public class PlayerHealth : MonoBehaviour
{
    [Header("Contagem de Vidas")]
    [SerializeField] private int totalVidas = 3; 
    private int vidasRestantes;

    [Header("Interface (UI)")]
    [SerializeField] private TextMeshProUGUI textoVidas; 
    [SerializeField] private Image imagemStatus; 
    [SerializeField] private List<Sprite> spritesDano; 

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
        AtualizarInterface();
    }

    public void TakeDamage(int damage)
    {
        if (vidasRestantes <= 0 || GameFlowManager.IsMatchEnding) return;

        vidasRestantes -= 1; 
        AtualizarInterface();

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

    private void AtualizarInterface()
    {
        // Atualiza o texto
        if (textoVidas != null) textoVidas.text = $"VIDAS: {vidasRestantes}";
        
        // Atualiza a imagem (se a lista não estiver vazia)
        if (imagemStatus != null && spritesDano.Count > 0)
        {
            // Se vidasRestantes for 3, ele pega o Element 3 da lista. Se for 0, pega o 0.
            if (vidasRestantes < spritesDano.Count)
                imagemStatus.sprite = spritesDano[vidasRestantes];
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
