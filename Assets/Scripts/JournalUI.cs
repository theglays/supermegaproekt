using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class JournalUI : MonoBehaviour
{
    [Header("Ссылки на UI")]
    [Tooltip("Весь объект книги (панель, которая выезжает)")]
    public GameObject journalPanel;
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
    [Tooltip("Позиция книги 'за экраном' (левее видимой области)")]
    public Vector2 hiddenPos = new Vector2(-900, 0);
    [Tooltip("Позиция книги 'на экране'")]
    public Vector2 visiblePos = new Vector2(50, 0);

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

        // Устанавливаем начальную позицию за экраном
        panelRect.anchoredPosition = hiddenPos;
        journalPanel.SetActive(true); // Активируем, но оставляем скрытым визуально
    }

    void Update()
    {
        if (isAnimating && panelRect != null)
        {
            Vector2 target = isOpen ? visiblePos : hiddenPos;
            // Плавное движение к цели
            panelRect.anchoredPosition = Vector2.Lerp(panelRect.anchoredPosition, target, slideSpeed * Time.deltaTime);

            // Если расстояние до цели меньше 1 пикселя → фиксируем позицию
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
            Refresh(); // Обновляем контент при открытии
        }
    }

    /// <summary> Переключить вкладку </summary>
    public void SwitchTab(bool showEntries)
    {
        if (entriesPage) entriesPage.SetActive(showEntries);
        if (charactersPage) charactersPage.SetActive(!showEntries);
    }

    /// <summary> Загрузить записи из сохранения и создать UI-элементы </summary>
    public void Refresh()
    {
        if (entriesContainer == null || charactersContainer == null) return;

        // Очищаем старые элементы
        foreach (Transform child in entriesContainer) Destroy(child.gameObject);
        foreach (Transform child in charactersContainer) Destroy(child.gameObject);

        if (SaveManager.Instance == null)
        {
            Debug.LogWarning("[JournalUI] SaveManager не найден на сцене!");
            return;
        }

        List<JournalEntry> entries = SaveManager.Instance.GetUnlockedEntries();
        // Сортировка: новые сверху
        entries.Sort((a, b) => b.unlockTimestamp.CompareTo(a.unlockTimestamp));

        foreach (var entry in entries)
        {
            if (entry.isNpc)
            {
                if (characterPrefab != null && charactersContainer != null)
                {
                    GameObject go = Instantiate(characterPrefab, charactersContainer);
                    go.GetComponent<CharacterCard>()?.Setup(entry);
                }
            }
            else
            {
                if (entryPrefab != null && entriesContainer != null)
                {
                    GameObject go = Instantiate(entryPrefab, entriesContainer);
                    go.GetComponent<JournalEntryUI>()?.Setup(entry);
                }
            }
        }
    }
}