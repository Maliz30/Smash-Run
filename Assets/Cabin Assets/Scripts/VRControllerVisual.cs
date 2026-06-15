using UnityEngine;

public class VRControllerVisual : MonoBehaviour
{
    [SerializeField] private float tipRadius = 0.05f;
    [SerializeField] private Color tipColor = Color.cyan;

    private GameObject tipObject;

    private void Awake()
    {
        // Cria uma esfera simples como ponta do controle
        tipObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        tipObject.name = "ControllerTip";
        tipObject.transform.SetParent(transform);
        tipObject.transform.localPosition = Vector3.forward * 0.1f; // ponta pra frente
        tipObject.transform.localScale = Vector3.one * tipRadius * 2f;

        // Adiciona collider trigger pra interagir com os botões
        SphereCollider col = tipObject.GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = 1f; // escala já controla o tamanho

        // Remove o renderer da esfera e coloca um material colorido
        MeshRenderer renderer = tipObject.GetComponent<MeshRenderer>();
        renderer.material.color = tipColor;
    }
}
