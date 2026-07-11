using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class VRVisionBlocker : MonoBehaviour
{
    [SerializeField] private Image inkOverlay;

    private Coroutine blockRoutine;

    private void Awake()
    {
        if (inkOverlay != null)
        {
            inkOverlay.enabled = false;
        }
    }

    public void Block(float duration)
    {
        if (inkOverlay == null)
        {
            return;
        }

        if (blockRoutine != null)
        {
            StopCoroutine(blockRoutine);
        }

        blockRoutine = StartCoroutine(BlockRoutine(duration));
    }

    private IEnumerator BlockRoutine(float duration)
    {
        inkOverlay.enabled = true;
        yield return new WaitForSeconds(duration);
        inkOverlay.enabled = false;
        blockRoutine = null;
    }
}
