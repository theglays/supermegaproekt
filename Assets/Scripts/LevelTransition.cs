using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelTransition : MonoBehaviour
{
    public static LevelTransition Instance { get; private set; }

    [Header("UI элементы")]
    public Image fadeImage; // Чёрная панель для затемнения

    [Header("Настройки")]
    public float fadeDuration = 1.5f; // Длительность затемнения

    void Awake()
    {
        // if (Instance == null)
        // {
        //     Instance = this;
        // }
        // else
        // {
        //     Destroy(gameObject);
        // }

        // Настраиваем fadeImage
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 0);
        }
    }

    /// Плавное затемнение и переход на следующий уровень
public void TransitionToLevel(string levelName)
{
    Debug.Log($"[Transition] 🎬 TransitionToLevel('{levelName}') вызван");
    
    if (fadeImage == null)
    {
        Debug.LogError("[Transition] ❌ fadeImage не назначен в Inspector!");
        return;
    }
    
    Debug.Log($"[Transition] ✅ fadeImage назначен, запускаем корутину FadeAndLoad('{levelName}')");
    StartCoroutine(FadeAndLoad(levelName));
}

   IEnumerator FadeAndLoad(string levelName)
{
    Debug.Log("[Transition] 🎨 Корутина FadeAndLoad началась");
    
    if (fadeImage == null)
    {
        Debug.LogError("[Transition] ❌ fadeImage = null в корутине!");
        yield break;
    }

    // Сохраняем прогресс
    if (SaveManager.Instance != null)
    {
        Debug.Log("[Transition] 💾 Сохраняем игру...");
        SaveManager.Instance.SaveGame();
    }

    // Затемнение
    Debug.Log("[Transition] 🌑 Начинаем затемнение...");
    float elapsed = 0f;
    Color startColor = fadeImage.color;
    Color endColor = new Color(0, 0, 0, 1);

    while (elapsed < fadeDuration)
    {
        elapsed += Time.deltaTime;
        fadeImage.color = Color.Lerp(startColor, endColor, elapsed / fadeDuration);
        Debug.Log($"[Transition] Прогресс затемнения: {elapsed:F2} / {fadeDuration:F2}");
        yield return null;
    }

    fadeImage.color = endColor;
    Debug.Log($"[Transition] ✅ Затемнение завершено. Загрузка сцены: {levelName}");

    // Загружаем следующую сцену
    SceneManager.LoadScene(levelName);
}

    /// Плавное появление после загрузки уровн
    public void FadeIn()
    {
        if (fadeImage != null)
        {
            StartCoroutine(FadeInCoroutine());
        }
    }

    IEnumerator FadeInCoroutine()
    {
        float elapsed = 0f;
        Color startColor = new Color(0, 0, 0, 1);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeImage.color = Color.Lerp(startColor, Color.clear, elapsed / fadeDuration);
            yield return null;
        }

        fadeImage.color = Color.clear;
    }
}