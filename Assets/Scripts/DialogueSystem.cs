using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance { get; private set; }

    [Header("UI элементы")]
    public GameObject dialoguePanel;          // Вся панель диалога
    public Image portraitImage;               // Картинка портрета
    public TextMeshProUGUI speakerNameText;   // Имя говорящего
    public TextMeshProUGUI dialogueText;      // Текст диалога
    public Button nextButton;                 // Кнопка "Далее"
    private System.Action onDialogueFinished; // 🔥 НОВОЕ

    [Header("Настройки анимации")]
    public float typingSpeed = 0.05f;         // Скорость вывода текста (секунд на символ)

    private DialogueData currentDialogue;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private string fullText = "";
    private Coroutine typingCoroutine;

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

        // Скрываем панель при старте
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        // Скрываем кнопку "Далее" по умолчанию
        if (nextButton != null)
            nextButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// Начать диалог
    /// </summary>
  public void StartDialogue(DialogueData dialogue, System.Action onFinish = null)
{
    if (dialogue == null || dialogue.lines.Length == 0)
    {
        Debug.LogError("[Dialogue] Диалог пуст или не назначен!");
        return;
    }

    currentDialogue = dialogue;
    currentLineIndex = 0;
    onDialogueFinished = onFinish;

    if (dialoguePanel != null)
        dialoguePanel.SetActive(true);

    // 🔥 СКРЫВАЕМ ПЛАШКУ ЗАДАЧ ПРИ НАЧАЛЕ ДИАЛОГА
    QuestUI questUI = FindObjectOfType<QuestUI>();
    if (questUI != null)
    {
        questUI.HideQuest();
        Debug.Log("[Dialogue] Плашка задач скрыта");
    }

    ShowLine();
}

    /// <summary>
    /// Показать текущую строку
    /// </summary>
    void ShowLine()
    {
        if (currentLineIndex >= currentDialogue.lines.Length)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = currentDialogue.lines[currentLineIndex];

        // Устанавливаем портрет
        if (portraitImage != null && line.portrait != null)
        {
            portraitImage.sprite = line.portrait;
            portraitImage.gameObject.SetActive(true);
        }
        else if (portraitImage != null)
        {
            portraitImage.gameObject.SetActive(false);
        }

        // Устанавливаем имя говорящего
        if (speakerNameText != null)
        {
            speakerNameText.text = line.speakerName;
            speakerNameText.gameObject.SetActive(!string.IsNullOrEmpty(line.speakerName));
        }

        // Запускаем анимацию текста
        fullText = line.text;
        if (dialogueText != null)
            dialogueText.text = "";

        // Скрываем кнопку "Далее"
        if (nextButton != null)
            nextButton.gameObject.SetActive(false);

        // Запускаем побуквенный вывод
        isTyping = true;
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText());
    }

    /// <summary>
    /// Анимация побуквенного вывода текста
    /// </summary>
    IEnumerator TypeText()
    {
        if (dialogueText == null) yield break;

        dialogueText.text = "";
        int charIndex = 0;

        while (charIndex < fullText.Length && isTyping)
        {
            dialogueText.text += fullText[charIndex];
            charIndex++;
            yield return new WaitForSeconds(typingSpeed);
        }

        // Текст полностью выведен
        isTyping = false;

        // Показываем кнопку "Далее"
        if (nextButton != null)
            nextButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Пропустить анимацию и показать весь текст сразу
    /// </summary>
    public void SkipTyping()
    {
        if (isTyping)
        {
            isTyping = false;
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            if (dialogueText != null)
                dialogueText.text = fullText;

            if (nextButton != null)
                nextButton.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Перейти к следующей строке (вызывается кнопкой "Далее")
    /// </summary>
    public void NextLine()
    {
        if (isTyping)
        {
            // Если текст ещё печатается — пропускаем анимацию
            SkipTyping();
        }
        else
        {
            // Иначе переходим к следующей строке
            currentLineIndex++;
            ShowLine();
        }
    }

    /// <summary>
    /// Завершить диалог
    /// </summary>
  void EndDialogue()
{
    if (dialoguePanel != null)
        dialoguePanel.SetActive(false);

    currentDialogue = null;
    currentLineIndex = 0;

    // Вызываем действие, если оно было передано
    if (onDialogueFinished != null)
    {
        onDialogueFinished.Invoke();
        onDialogueFinished = null;
    }

    //  ПОКАЗЫВАЕМ ПЛАШКУ ЗАДАЧ ПОСЛЕ ЗАВЕРШЕНИЯ ДИАЛОГА
    QuestUI questUI = FindObjectOfType<QuestUI>();
    if (questUI != null && QuestManager.Instance != null)
    {
        Quest currentQuest = QuestManager.Instance.GetCurrentQuest();
        if (currentQuest != null)
        {
            questUI.UpdateQuestDisplay(currentQuest);
            Debug.Log("[Dialogue] Плашка задач показана с новой задачей");
        }
        else
        {
            questUI.HideQuest();
            Debug.Log("[Dialogue] Все задачи выполнены, плашка скрыта");
        }
    }

    Debug.Log("[Dialogue] Диалог завершён");
}

    void Update()
    {
        // Если диалог активен и текст печатается — клик ЛКМ пропускает анимацию
        if (dialoguePanel != null && dialoguePanel.activeSelf && isTyping)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SkipTyping();
            }
        }
    }
}