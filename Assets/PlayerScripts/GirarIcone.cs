using UnityEngine;

public class GirarIcone : MonoBehaviour
{
    // Velocidade do giro. Negativo gira para o sentido horário
    public float velocidade = -300f; 

    void Update()
    {
        // Faz o objeto girar no próprio eixo Z continuamente
        transform.Rotate(0, 0, velocidade * Time.unscaledDeltaTime);
    }
}