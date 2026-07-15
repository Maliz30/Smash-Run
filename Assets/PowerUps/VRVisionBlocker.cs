using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class VRVisionBlocker : MonoBehaviour
{
    [SerializeField] private Image inkOverlay;
    [SerializeField] private float growDuration = 0.4f;
    [SerializeField, Range(0.1f, 1f)] private float cornerReach = 0.5f;

    private RectTransform overlayRect;
    private RectTransform canvasRect;
    private Coroutine blockRoutine;

    private void Awake()
    {
        if (inkOverlay != null)
        {
            overlayRect = inkOverlay.rectTransform;
            canvasRect = overlayRect.parent as RectTransform;
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
        PositionAtRandomCorner();
        overlayRect.localScale = Vector3.zero;
        inkOverlay.enabled = true;

        float elapsed = 0f;
        while (elapsed < growDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / growDuration);
            overlayRect.localScale = Vector3.one * t;
            yield return null;
        }

        overlayRect.localScale = Vector3.one;

        float remaining = duration - growDuration;
        if (remaining > 0f)
        {
            yield return new WaitForSeconds(remaining);
        }

        inkOverlay.enabled = false;
        blockRoutine = null;
    }

    private void PositionAtRandomCorner()
    {
        if (canvasRect == null)
        {
            return;
        }

        float halfW = canvasRect.rect.width * 0.5f;
        float halfH = canvasRect.rect.height * 0.5f;
        float overlayHalfW = overlayRect.rect.width * 0.5f;
        float overlayHalfH = overlayRect.rect.height * 0.5f;

        float x = (Random.value < 0.5f ? -1f : 1f) * Mathf.Max(0f, halfW - overlayHalfW) * cornerReach;
        float y = (Random.value < 0.5f ? -1f : 1f) * Mathf.Max(0f, halfH - overlayHalfH) * cornerReach;

        overlayRect.anchoredPosition = new Vector2(x, y);
    }
}
