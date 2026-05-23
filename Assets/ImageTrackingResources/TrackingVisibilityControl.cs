using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImage))]
public class ControleVisibilidadeAR : MonoBehaviour
{
   
    public GameObject modelo3D;

    private ARTrackedImage imagemRastreada;

    void Awake()
    {
        imagemRastreada = GetComponent<ARTrackedImage>();
    }

    void Update()
    {
        if (imagemRastreada.trackingState == TrackingState.Tracking)
        {
            if (!modelo3D.activeSelf)
            {
                modelo3D.SetActive(true);
            }
        }
        else
        {
            if (modelo3D.activeSelf)
            {
                modelo3D.SetActive(false);
            }
        }
    }
}