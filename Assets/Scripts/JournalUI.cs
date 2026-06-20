using UnityEngine;

public class JournalUI : MonoBehaviour
{
    [Header("Ссылки на UI")]
    public GameObject journalPanel;    // Вся книга (BookPage)
    public GameObject titlePage;       // Титульник
    public GameObject entriesPage;     // Страница записей (Content_Entries)
    public GameObject charactersPage;  // Страница персонажей (Content_Characters)

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

    /// <summary> Открыть/закрыть дневник </summary>
    public void ToggleJournal()
    {
        if (isAnimating) return;
        
        isOpen = !isOpen;
        isAnimating = true;

        if (isOpen)
        {
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
    }

    /// <summary> Переключить на вкладку "Персонажи" </summary>
    public void ShowCharactersTab()
    {
        if (titlePage) titlePage.SetActive(false);
        if (entriesPage) entriesPage.SetActive(false);
        if (charactersPage) charactersPage.SetActive(true);
    }
}