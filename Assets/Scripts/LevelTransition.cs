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
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Настраиваем fadeImage
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 0);
        }
    }

    /// <summary>
    /// Плавное затемнение и переход на следующий уровень
    /// </summary>
    public void TransitionToLevel(string levelName)
    {
        StartCoroutine(FadeAndLoad(levelName));
    }

    IEnumerator FadeAndLoad(string levelName)
    {
        if (fadeImage == null)
        {
            Debug.LogError("[Transition] fadeImage не назначен!");
            yield break;
        }

        // Сохраняем прогресс перед переходом
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame();
        }

        // Затемнение
        float elapsed = 0f;
        Color startColor = fadeImage.color;
        Color endColor = new Color(0, 0, 0, 1);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeImage.color = Color.Lerp(startColor, endColor, elapsed / fadeDuration);
            yield return null;
        }

        fadeImage.color = endColor;

        // Загружаем следующую сцену
        Debug.Log($"[Transition] Загрузка уровня: {levelName}");
        SceneManager.LoadScene(levelName);
    }

    /// <summary>
    /// Плавное появление после загрузки уровня
    /// </summary>
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