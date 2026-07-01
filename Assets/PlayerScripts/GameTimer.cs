using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("Configuração do Tempo")]
    [SerializeField] private float duracaoPartida = 180f;
    private float tempoRestante;
    private bool partidaEncerrada;

    [Header("Interface (UI)")]
    [SerializeField] private TextMeshProUGUI textoTempoTela;
    [SerializeField] private TextMeshProUGUI textoTempoVR;

    private void Start()
    {
        tempoRestante = duracaoPartida;
        AtualizarTextoInterface();
    }

    private void Update()
    {
        if (partidaEncerrada) return;

        tempoRestante -= Time.deltaTime;

        if (tempoRestante <= 0)
        {
            tempoRestante = 0;
            partidaEncerrada = true;
            Debug.Log("Tempo esgotado!");
        }

        AtualizarTextoInterface();
    }

    private void AtualizarTextoInterface()
    {
        int minutos = Mathf.FloorToInt(tempoRestante / 60);
        int segundos = Mathf.FloorToInt(tempoRestante % 60);
        string texto = string.Format("{0:00}:{1:00}", minutos, segundos);

        if (textoTempoTela != null) textoTempoTela.text = texto;
        if (textoTempoVR != null) textoTempoVR.text = texto;
    }
}
