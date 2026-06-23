using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelStartFade : MonoBehaviour
{
    [Header("Настройки")]
    public float fadeDuration = 2f;  // Длительность появления

    void Start()
    {
        Debug.Log("[LevelStartFade] Начинаем fade-in...");
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        // Ждём кадр чтобы всё загрузилось
        yield return null;

        // Ищем или создаём FadeImage
        Image fadeImage = GetOrCreateFadeImage();
        
        if (fadeImage == null)
        {
            Debug.LogError("[LevelStartFade] Не удалось создать FadeImage!");
            yield break;
        }

        // Начинаем с полностью чёрного
        fadeImage.color = new Color(0, 0, 0, 1);

        // Анимируем от чёрного к прозрачному
        float elapsed = 0f;
        Color startColor = new Color(0, 0, 0, 1);
        Color endColor = new Color(0, 0, 0, 0);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            fadeImage.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        fadeImage.color = Color.clear;
        
        // Удаляем объект после завершения
        Destroy(fadeImage.gameObject);
        
        Debug.Log("[LevelStartFade] Fade-in завершён");
    }

    Image GetOrCreateFadeImage()
    {
        // Ищем Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("[LevelStartFade] Canvas не найден!");
            return null;
        }

        // Ищем существующий FadeImage
        Image fadeImage = GameObject.Find("FadeImage")?.GetComponent<Image>();
        
        if (fadeImage != null)
            return fadeImage;

        // Создаём новый
        GameObject fadeObj = new GameObject("FadeImage");
        fadeObj.transform.SetParent(canvas.transform, false);
        
        RectTransform rect = fadeObj.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        
        Image img = fadeObj.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 1);
        img.raycastTarget = false;
        
        return img;
    }
}