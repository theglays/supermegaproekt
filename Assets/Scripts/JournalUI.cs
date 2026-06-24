using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class JournalUI : MonoBehaviour
{
    [Header("Ссылки на UI")]
    public GameObject journalPanel;    // Вся книга (BookPage)
    public GameObject titlePage;       // Титульник
    public GameObject entriesPage;     // Страница записей (Content_Entries)
    public GameObject charactersPage;  // Страница персонажей (Content_Characters)

    [Header("Уведомление о новой записи")]
    public GameObject newEntryNotification;  // Плашка с текстом "Добавлена новая запись"
    public int blinkCount = 5;               // Количество миганий
    public float fadeDuration = 0.3f;        // Скорость одного мигания
    public float pauseBetweenBlinks = 0.2f;  // Пауза между миганиями

    [Header("Анимация выезда")]
    public float slideSpeed = 8f;
    public Vector2 hiddenPos = new Vector2(-6000, 0);
    public Vector2 visiblePos = new Vector2(0, 0);

    // Приватные переменные
    private bool isOpen = false;
    private bool isAnimating = false;
    private RectTransform panelRect;

    private Coroutine notificationCoroutine;
    private CanvasGroup notificationCanvasGroup;

    void Awake()
    {
            // DontDestroyOnLoad(gameObject);
        // Инициализация панели дневника
        panelRect = journalPanel.GetComponent<RectTransform>();
        if (panelRect == null)
        {
            Debug.LogError("[JournalUI] journalPanel должен иметь RectTransform!");
            return;
        }

        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = hiddenPos;
        
        journalPanel.SetActive(true);

        // Инициализация Canvas Group для уведомления
        if (newEntryNotification != null)
        {
            notificationCanvasGroup = newEntryNotification.GetComponent<CanvasGroup>();
            if (notificationCanvasGroup == null)
            {
                notificationCanvasGroup = newEntryNotification.AddComponent<CanvasGroup>();
            }
            
            notificationCanvasGroup.alpha = 0f;
            notificationCanvasGroup.blocksRaycasts = false;
            newEntryNotification.SetActive(false);
        }
    }

    void Start()
    {
        if (panelRect != null)
            panelRect.anchoredPosition = hiddenPos;
    }

    void Update()
    {
        if (isAnimating && panelRect != null)
        {
            Vector2 target = isOpen ? visiblePos : hiddenPos;
            panelRect.anchoredPosition = Vector2.Lerp(panelRect.anchoredPosition, target, slideSpeed * Time.deltaTime);

            if (Vector2.Distance(panelRect.anchoredPosition, target) < 1f)
            {
                panelRect.anchoredPosition = target;
                isAnimating = false;
            }
        }
    }

    // ==================== УПРАВЛЕНИЕ ДНЕВНИКОМ ====================

    /// <summary> Открыть/закрыть дневник </summary>
    public void ToggleJournal()
    {
        if (isAnimating) return;
        
        isOpen = !isOpen;
        isAnimating = true;

        if (isOpen)
        {
            ShowTitlePage();
            HideNotification(); // Скрываем уведомление при открытии дневника
        }
    }

    /// <summary> Показать титульный лист </summary>
    public void ShowTitlePage()
    {
        if (titlePage) titlePage.SetActive(true);
        if (entriesPage) entriesPage.SetActive(false);
        if (charactersPage) charactersPage.SetActive(false);
    }

    /// <summary> Переключить на вкладку "Записи" </summary>
   /// <summary> Переключить на вкладку "Записи" </summary>
public void ShowEntriesTab()
{
    if (titlePage) titlePage.SetActive(false);
    if (entriesPage) entriesPage.SetActive(true);
    if (charactersPage) charactersPage.SetActive(false);
    
    //  ЗВУК ПЕРЕЛИСТЫВАНИЯ СТРАНИЦЫ
    if (AudioManager.Instance != null)
    {
        AudioManager.Instance.PlaySFX("Page_Turn");
    }
}

/// <summary> Переключить на вкладку "Персонажи" </summary>
public void ShowCharactersTab()
{
    if (titlePage) titlePage.SetActive(false);
    if (entriesPage) entriesPage.SetActive(false);
    if (charactersPage) charactersPage.SetActive(true);
    
    //  ЗВУК ПЕРЕЛИСТЫВАНИЯ СТРАНИЦЫ
    if (AudioManager.Instance != null)
    {
        AudioManager.Instance.PlaySFX("Page_Turn");
    }
}

    // ==================== УВЕДОМЛЕНИЕ О НОВОЙ ЗАПИСИ ====================

    /// <summary> Показать мигающее уведомление о новой записи </summary>
    public void ShowNotification()
    {
        if (newEntryNotification == null || notificationCanvasGroup == null) return;

        // Если анимация уже идет — прерываем и начинаем заново
        if (notificationCoroutine != null)
            StopCoroutine(notificationCoroutine);

        newEntryNotification.SetActive(true);
        notificationCoroutine = StartCoroutine(BlinkNotificationCoroutine());
        
        Debug.Log($"[JournalUI] Уведомление запущено ({blinkCount} миганий)");
    }

    /// <summary> Мгновенно скрыть уведомление </summary>
    public void HideNotification()
    {
        if (notificationCoroutine != null)
        {
            StopCoroutine(notificationCoroutine);
            notificationCoroutine = null;
        }

        if (notificationCanvasGroup != null)
        {
            notificationCanvasGroup.alpha = 0f;
            notificationCanvasGroup.blocksRaycasts = false;
        }
        
        if (newEntryNotification != null)
            newEntryNotification.SetActive(false);
    }

/// <summary>
/// Принудительно закрыть дневник (без анимации)
/// </summary>
public void HideJournal()
{
    isOpen = false;
    isAnimating = false;
    
    if (panelRect != null)
    {
        panelRect.anchoredPosition = hiddenPos;
    }

    // Скрываем все страницы
    if (titlePage) titlePage.SetActive(false);
    if (entriesPage) entriesPage.SetActive(false);
    if (charactersPage) charactersPage.SetActive(false);

    Debug.Log("[JournalUI] Дневник закрыт");
}
    /// <summary> Корутина для мигания: Fade In → Пауза → Fade Out (повторяется blinkCount раз) </summary>
    private IEnumerator BlinkNotificationCoroutine()
    {
        notificationCanvasGroup.blocksRaycasts = true;
        
        for (int i = 0; i < blinkCount; i++)
        {
            // 1. Плавное появление (Fade In)
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                notificationCanvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
                yield return null;
            }
            notificationCanvasGroup.alpha = 1f;
            
            // 2. Пауза на полной видимости
            yield return new WaitForSeconds(pauseBetweenBlinks);
            
            // 3. Плавное исчезновение (Fade Out)
            elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                notificationCanvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / fadeDuration);
                yield return null;
            }
            notificationCanvasGroup.alpha = 0f;
            
            // 4. Короткая пауза перед следующим миганием (кроме последнего)
            if (i < blinkCount - 1)
                yield return new WaitForSeconds(0.1f);
        }
        
        // После всех миганий — полностью скрываем
        notificationCanvasGroup.blocksRaycasts = false;
        newEntryNotification.SetActive(false);
        
        Debug.Log("[JournalUI] Уведомление завершено");
    }
}