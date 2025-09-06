using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeUI : MonoBehaviour
{
    public static FadeUI Instance;

    [Header("Fade Settings")]
    public Image fadeImage;          // UI Image for fading
    public float fadeDuration = 0.5f;

    // ✅ Input lock flag
    public bool IsFading { get; private set; } = false;

    private void Awake()
    {
        // Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Start fully transparent
        SetAlpha(0f);
    }

    private void SetAlpha(float alpha)
    {
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = alpha;
            fadeImage.color = c;
        }
    }

    public IEnumerator FadeOutIn(System.Action onFadeComplete)
    {
        IsFading = true;

        // Fade out
        yield return StartCoroutine(Fade(0f, 1f));

        // Teleport after fade-out
        onFadeComplete?.Invoke();

        // Fade in
        yield return StartCoroutine(Fade(1f, 0f));

        IsFading = false;
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        if (fadeImage == null) yield break;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(endAlpha);
    }
}
