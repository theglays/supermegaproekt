using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TopicSelectionUI : MonoBehaviour
{
    public static TopicSelectionUI Instance { get; private set; }

    [Header("UI элементы")]
    public GameObject panel;
    public TextMeshProUGUI titleText;
    public Button topicButton1;
    public TextMeshProUGUI topicButtonText1;
    public Button topicButton2;
    public TextMeshProUGUI topicButtonText2;
    public Button closeButton;

    private DialogueTopic[] currentTopics;
    private Action<int> onTopicSelected;

    void Awake()
    {
        // 🔥 СОЗДАЁМ INSTANCE!
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[TopicSelectionUI] ✅ Instance создан");
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Проверяем что все поля назначены
        if (panel == null)
        {
            Debug.LogError("[TopicSelectionUI] ❌ Panel не назначен!");
        }
        
        if (topicButton1 == null || topicButton2 == null)
        {
            Debug.LogError("[TopicSelectionUI] ❌ Кнопки не назначены!");
        }

        // Назначаем кнопки
        if (topicButton1 != null)
            topicButton1.onClick.AddListener(() => SelectTopic(0));
        
        if (topicButton2 != null)
            topicButton2.onClick.AddListener(() => SelectTopic(1));
        
        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);

        // Скрываем панель при старте
        if (panel != null)
            panel.SetActive(false);

        Debug.Log("[TopicSelectionUI] Инициализация завершена");
    }

    /// <summary>
    /// Показать выбор тем
    /// </summary>
    public void Show(DialogueTopic[] topics, Action<int> onSelect)
    {
        Debug.Log("[TopicSelectionUI] Show() вызван");
        
        if (topics == null || topics.Length == 0)
        {
            Debug.LogError("[TopicSelectionUI] ❌ Темы не назначены!");
            return;
        }

        currentTopics = topics;
        onTopicSelected = onSelect;

        // Сбрасываем кнопки
        if (topicButton1 != null)
            topicButton1.gameObject.SetActive(false);
        if (topicButton2 != null)
            topicButton2.gameObject.SetActive(false);

        // Находим непройденные темы и показываем кнопки
        int availableIndex = 0;
        for (int i = 0; i < topics.Length; i++)
        {
            if (topics[i] != null && !topics[i].isCompleted)
            {
                Debug.Log($"[TopicSelectionUI] Найдена доступная тема: {topics[i].topicName}");
                
                if (availableIndex == 0 && topicButton1 != null && topicButtonText1 != null)
                {
                    topicButton1.gameObject.SetActive(true);
                    topicButtonText1.text = topics[i].topicName;
                    Debug.Log($"[TopicSelectionUI] Кнопка 1: {topics[i].topicName}");
                }
                else if (availableIndex == 1 && topicButton2 != null && topicButtonText2 != null)
                {
                    topicButton2.gameObject.SetActive(true);
                    topicButtonText2.text = topics[i].topicName;
                    Debug.Log($"[TopicSelectionUI] Кнопка 2: {topics[i].topicName}");
                }
                availableIndex++;
            }
        }

        // Если осталась только одна тема
        if (availableIndex == 1 && topicButton2 != null)
        {
            topicButton2.gameObject.SetActive(false);
        }

        if (panel != null)
        {
            panel.SetActive(true);
            Debug.Log("[TopicSelectionUI] ✅ Панель показана");
        }
        else
        {
            Debug.LogError("[TopicSelectionUI] ❌ Panel = NULL!");
        }
    }

    void SelectTopic(int index)
    {
        Debug.Log($"[TopicSelectionUI] SelectTopic({index}) вызван");
        
        // Находим реальную тему (пропуская пройденные)
        int availableCount = 0;
        for (int i = 0; i < currentTopics.Length; i++)
        {
            if (currentTopics[i] != null && !currentTopics[i].isCompleted)
            {
                if (availableCount == index)
                {
                    Debug.Log($"[TopicSelectionUI] ✅ Выбрана тема: {currentTopics[i].topicName}");
                    Hide();
                    onTopicSelected?.Invoke(i);
                    return;
                }
                availableCount++;
            }
        }
        
        Debug.LogWarning($"[TopicSelectionUI] ⚠️ Тема #{index} не найдена");
    }

    public void Hide()
    {
        Debug.Log("[TopicSelectionUI] Hide() вызван");
        
        if (panel != null)
            panel.SetActive(false);
        
        if (topicButton1 != null)
            topicButton1.gameObject.SetActive(false);
        
        if (topicButton2 != null)
            topicButton2.gameObject.SetActive(false);
    }
}