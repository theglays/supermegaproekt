using UnityEngine;
using UnityEngine.SceneManagement;
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
    public AudioClip introMusic;  // 🔥 Музыка для интро
    public float introMusicVolume = 0.8f;

    [Header("Настройки")]
    public float typingSpeed = 0.05f;
    public float fadeDuration = 1.5f;

    private int currentPhraseIndex = 0;
    private bool isTyping = false;
    private string currentFullText = "";
    private TextMeshProUGUI currentTextObject;
    private Coroutine typingCoroutine;
    private bool isIntroActive = false;
    private AudioSource audioSource;  // 🔥 AudioSource для музыки

    void Awake()
    {
        // Создаём AudioSource для музыки интро
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        
        Debug.Log("[IntroSequence] AudioSource создан");
    }

    /// <summary>
    /// Вызывается кнопкой "Начать игру"
    /// </summary>
    public void StartIntro()
    {
        if (isIntroActive) return;
        isIntroActive = true;

        Debug.Log("[Intro] Начинаем интро...");
        
        // 🔥 ОСТАНАВЛИВАЕМ МУЗЫКУ МЕНЮ
        StopMenuMusic();
        
        // 🔥 ЗАПУСКАЕМ МУЗЫКУ ИНТРО
        PlayIntroMusic();
        
        // Показываем canvas
        if (introCanvas != null)
            introCanvas.SetActive(true);

        // Запускаем корутину
        StartCoroutine(PlayIntroSequence());
    }

    /// <summary>
    /// Остановить музыку меню
    /// </summary>
    void StopMenuMusic()
    {
        // Ищем все AudioSource в сцене Menu
        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        
        foreach (AudioSource source in allAudioSources)
        {
            // Останавливаем всё что играет (кроме нашего audioSource)
            if (source != audioSource && source.isPlaying)
            {
                Debug.Log($"[Intro] Останавливаем: {source.gameObject.name}");
                source.Stop();
            }
        }
        
        Debug.Log("[Intro] Музыка меню остановлена");
    }

    /// <summary>
    /// Играть музыку интро
    /// </summary>
    void PlayIntroMusic()
    {
        if (introMusic != null && audioSource != null)
        {
            audioSource.clip = introMusic;
            audioSource.volume = introMusicVolume;
            audioSource.Play();
            Debug.Log("[Intro] Музыка интро играет");
        }
        else
        {
            Debug.LogWarning("[Intro] introMusic не назначена!");
        }
    }

    /// <summary>
    /// Остановить музыку интро
    /// </summary>
    void StopIntroMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("[Intro] Музыка интро остановлена");
        }
    }

    IEnumerator PlayIntroSequence()
    {
        // 1. Затемнение
        yield return new WaitForSeconds(fadeDuration);

        // 2. Показываем фразы
        currentPhraseIndex = 0;
        ShowNextPhrase();

        // Ждём пока все фразы не будут показаны
        while (currentPhraseIndex < 4)
        {
            yield return null;
        }

        // 3. После 4-й фразы ждём клика
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        // 4. Скрываем текст и загружаем Level1
        Debug.Log("[Intro] Загружаем Level1...");
        
        // 🔥 ОСТАНАВЛИВАЕМ МУЗЫКУ ИНТРО
        StopIntroMusic();
        
        yield return StartCoroutine(FadeOutAndLoadLevel());
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
            Debug.LogError($"[Intro] Phrase{currentPhraseIndex + 1} не назначен!");
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

    IEnumerator FadeOutAndLoadLevel()
    {
        if (phrase1 != null) phrase1.gameObject.SetActive(false);
        if (phrase2 != null) phrase2.gameObject.SetActive(false);
        if (phrase3 != null) phrase3.gameObject.SetActive(false);
        if (phrase4 != null) phrase4.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        // 🔥 Загружаем Level1 (там свой MusicController запустится)
        SceneManager.LoadScene("Level1");
    }
}