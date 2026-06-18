using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class JournalUI : MonoBehaviour
{
    [Header("Ссылки на UI")]
    [Tooltip("Весь объект книги (панель, которая выезжает)")]
    public GameObject journalPanel;
    
    [Tooltip("Титульный лист (показывается при открытии)")]
    public GameObject titlePage;
    
    [Tooltip("Страница с записями о предметах")]
    public GameObject entriesPage;
    
    [Tooltip("Страница с карточками персонажей")]
    public GameObject charactersPage;
    
    [Tooltip("Родительский объект внутри ScrollView для записей")]
    public Transform entriesContainer;
    
    [Tooltip("Родительский объект внутри ScrollView для персонажей")]
    public Transform charactersContainer;

    [Header("Префабы")]
    [Tooltip("Префаб одной записи (должен иметь компонент JournalEntryUI)")]
    public GameObject entryPrefab;
    
    [Tooltip("Префаб карточки персонажа (должен иметь компонент CharacterCard)")]
    public GameObject characterPrefab;

    [Header("Анимация выезда")]
    public float slideSpeed = 8f;
    public Vector2 hiddenPos = new Vector2(-6000, 0);
    public Vector2 visiblePos = new Vector2(0, 0);

    private bool isOpen = false;
    private bool isAnimating = false;
    private RectTransform panelRect;

    void Awake()
    {
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
    }

    void Start()
    {
        if (panelRect != null)
        {
            panelRect.anchoredPosition = hiddenPos;
        }
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

    /// <summary> Открыть/закрыть дневник </summary>
    public void ToggleJournal()
    {
        if (isAnimating) return;
        
        isOpen = !isOpen;
        isAnimating = true;

        if (isOpen)
        {
            // При открытии показываем титульный лист
            ShowTitlePage();
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
    public void ShowEntriesTab()
    {
        if (titlePage) titlePage.SetActive(false);
        if (entriesPage) entriesPage.SetActive(true);
        if (charactersPage) charactersPage.SetActive(false);
        
        Refresh(); // Загружаем записи
    }

    /// <summary> Переключить на вкладку "Персонажи" </summary>
    public void ShowCharactersTab()
    {
        if (titlePage) titlePage.SetActive(false);
        if (entriesPage) entriesPage.SetActive(false);
        if (charactersPage) charactersPage.SetActive(true);
        
        Refresh(); // Загружаем персонажей
    }

    /// <summary> Загрузить записи из сохранения и создать UI-элементы </summary>
 public void Refresh()
{
    Debug.Log("[JournalUI] Refresh() вызван");
    
    if (entriesContainer == null)
    {
        Debug.LogError("[JournalUI] entriesContainer НЕ назначен в Inspector!");
        return;
    }
    
    if (entryPrefab == null)
    {
        Debug.LogError("[JournalUI] entryPrefab НЕ назначен в Inspector!");
        return;
    }
    
    if (SaveManager.Instance == null)
    {
        Debug.LogError("[JournalUI] SaveManager.Instance = null!");
        return;
    }

    Debug.Log($"[JournalUI] entriesContainer: {entriesContainer.name}");
    Debug.Log($"[JournalUI] entryPrefab: {entryPrefab.name}");

    // Очищаем старые элементы
    foreach (Transform child in entriesContainer)
    {
        Debug.Log($"[JournalUI] Удаляем старый объект: {child.name}");
        Destroy(child.gameObject);
    }

    List<JournalEntry> entries = SaveManager.Instance.GetUnlockedEntries();
    Debug.Log($"[JournalUI] Получено записей из SaveManager: {entries.Count}");

    if (entries.Count == 0)
    {
        Debug.LogWarning("[JournalUI] Список записей пуст!");
        return;
    }

    entries.Sort((a, b) => b.unlockTimestamp.CompareTo(a.unlockTimestamp));

    int createdCount = 0;
    foreach (var entry in entries)
    {
        Debug.Log($"[JournalUI] Обработка записи: {entry.title}, isNpc: {entry.isNpc}");
        
        if (entry.isNpc)
        {
            Debug.Log("[JournalUI] Пропускаем NPC (это персонаж)");
            continue; // Пропускаем персонажей на вкладке записей
        }
        
        if (entryPrefab != null && entriesContainer != null)
        {
            GameObject go = Instantiate(entryPrefab, entriesContainer);
            go.GetComponent<JournalEntryUI>()?.Setup(entry);
            createdCount++;
            Debug.Log($"[JournalUI] ✅ Создан UI-элемент для: {entry.title}");
        }
    }
    
    Debug.Log($"[JournalUI] Всего создано элементов: {createdCount}");
}
}