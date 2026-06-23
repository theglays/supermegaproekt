using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class IntroSequence : MonoBehaviour
{
    [Header("UI элементы")]
    public GameObject introCanvas;
    public TextMeshProUGUI phrase1;
    public TextMeshProUGUI phrase2;
    public TextMeshProUGUI phrase3;
    public TextMeshProUGUI phrase4;

    [Header("Тексты фраз")]
    [TextArea(2, 3)]
    public string text1 = "Санкт-Петербург, 1880 год...";
    [TextArea(2, 3)]
    public string text2 = "Тайны прошлого не дают покоя...";
    [TextArea(2, 3)]
    public string text3 = "Карты решают всё...";
    [TextArea(2, 3)]
    public string text4 = "Но цена ошибки — жизнь.";

    [Header("Музыка интро")]
    public AudioClip introMusic;
    public float introMusicVolume = 0.8f;

    [Header("Настройки")]
    public float typingSpeed = 0.05f;
    public float fadeDuration = 1.5f;
    public float endFadeDuration = 2f;  // 🔥 Длительность финального затемнения

    private int currentPhraseIndex = 0;
    private bool isTyping = false;
    private string currentFullText = "";
    private TextMeshProUGUI currentTextObject;
    private Coroutine typingCoroutine;
    private bool isIntroActive = false;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    public void StartIntro()
    {
        if (isIntroActive) return;
        isIntroActive = true;

        Debug.Log("[Intro] Начинаем интро...");
        
        StopMenuMusic();
        PlayIntroMusic();
        
        if (introCanvas != null)
            introCanvas.SetActive(true);

        StartCoroutine(PlayIntroSequence());
    }

    void StopMenuMusic()
    {
        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        
        foreach (AudioSource source in allAudioSources)
        {
            if (source != audioSource && source.isPlaying)
            {
                source.Stop();
            }
        }
    }

    void PlayIntroMusic()
    {
        if (introMusic != null && audioSource != null)
        {
            audioSource.clip = introMusic;
            audioSource.volume = introMusicVolume;
            audioSource.Play();
        }
    }

    void StopIntroMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    IEnumerator PlayIntroSequence()
    {
        // 1. Затемнение
        yield return new WaitForSeconds(fadeDuration);

        // 2. Показываем фразы
        currentPhraseIndex = 0;
        ShowNextPhrase();

        while (currentPhraseIndex < 4)
        {
            yield return null;
        }

        // 3. После 4-й фразы ждём клика
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        // 4. 🔥 ПЛАВНОЕ ЗАТЕМНЕНИЕ
        Debug.Log("[Intro] Начинаем плавное затемнение...");
        yield return StartCoroutine(FadeToBlack());

        // 5. Загружаем Level1
        Debug.Log("[Intro] Загружаем Level1...");
        StopIntroMusic();
        
        SceneManager.LoadScene("Level1");
    }

    /// <summary>
    /// Плавное затемнение экрана
    /// </summary>
    IEnumerator FadeToBlack()
    {
        // Создаём чёрную панель если её нет
        Image fadeImage = GetOrCreateFadeImage();
        
        if (fadeImage == null)
        {
            Debug.LogError("[Intro] Не удалось создать FadeImage!");
            yield break;
        }

        // Анимируем прозрачность от 0 до 1
        float elapsed = 0f;
        Color startColor = new Color(0, 0, 0, 0);
        Color endColor = new Color(0, 0, 0, 1);

        while (elapsed < endFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / endFadeDuration;
            fadeImage.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        fadeImage.color = endColor;
        Debug.Log("[Intro] Затемнение завершено");
    }

    /// <summary>
    /// Получает или создаёт FadeImage
    /// </summary>
    Image GetOrCreateFadeImage()
    {
        // Ищем существующий FadeImage
        Image fadeImage = GameObject.Find("FadeImage")?.GetComponent<Image>();
        
        if (fadeImage != null)
            return fadeImage;

        // Создаём новый
        GameObject fadeObj = new GameObject("FadeImage");
        fadeObj.transform.SetParent(introCanvas.transform, false);
        
        RectTransform rect = fadeObj.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        
        Image img = fadeObj.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0);
        img.raycastTarget = false;
        
        return img;
    }

    void ShowNextPhrase()
    {
        if (currentPhraseIndex >= 4) return;

        switch (currentPhraseIndex)
        {
            case 0: currentTextObject = phrase1; break;
            case 1: currentTextObject = phrase2; break;
            case 2: currentTextObject = phrase3; break;
            case 3: currentTextObject = phrase4; break;
        }

        if (currentTextObject == null)
        {
            currentPhraseIndex++;
            return;
        }

        switch (currentPhraseIndex)
        {
            case 0: currentFullText = text1; break;
            case 1: currentFullText = text2; break;
            case 2: currentFullText = text3; break;
            case 3: currentFullText = text4; break;
        }

        currentTextObject.gameObject.SetActive(true);
        currentTextObject.text = "";

        isTyping = true;
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        int charIndex = 0;

        while (charIndex < currentFullText.Length && isTyping)
        {
            currentTextObject.text += currentFullText[charIndex];
            charIndex++;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        currentTextObject.text = currentFullText;
    }

    void Update()
    {
        if (!isIntroActive) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                isTyping = false;
                if (typingCoroutine != null)
                    StopCoroutine(typingCoroutine);
                
                if (currentTextObject != null)
                    currentTextObject.text = currentFullText;
            }
            else
            {
                currentPhraseIndex++;
                
                if (currentPhraseIndex < 4)
                {
                    ShowNextPhrase();
                }
            }
        }
    }
}