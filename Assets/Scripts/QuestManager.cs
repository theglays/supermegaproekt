using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Список задач (порядок важен!)")]
    public List<Quest> quests = new List<Quest>();
    
    private int currentQuestIndex = 0;
    private Quest currentQuest;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (quests.Count > 0)
        {
            currentQuest = quests[0];
            Debug.Log($"[Quest] Начата задача: {currentQuest.description}");
            
            // Уведомляем UI
            QuestUI ui = FindObjectOfType<QuestUI>();
            if (ui != null) ui.UpdateQuestDisplay(currentQuest);
        }
    }

    /// <summary>
    /// Вызывается при взаимодействии с предметом/NPC
    /// </summary>
    public void OnTargetInteracted(string targetId)
    {
        if (currentQuest == null || currentQuest.IsCompleted()) return;

        // Проверяем, есть ли этот ID в списке целей текущей задачи
        if (currentQuest.targetIds.Contains(targetId) && !currentQuest.completedTargets.Contains(targetId))
        {
            currentQuest.completedTargets.Add(targetId);
            Debug.Log($"[Quest] Прогресс: {currentQuest.completedTargets.Count}/{currentQuest.targetIds.Count}");
            
            // Уведомляем UI об обновлении прогресса
            QuestUI ui = FindObjectOfType<QuestUI>();
            if (ui != null) ui.UpdateQuestDisplay(currentQuest);

            // Проверяем, выполнена ли задача
            if (currentQuest.IsCompleted())
            {
                Debug.Log($"[Quest] ✅ Задача выполнена: {currentQuest.description}");
                CompleteCurrentQuest();
            }
        }
    }

    void CompleteCurrentQuest()
    {
        currentQuestIndex++;
        
        if (currentQuestIndex < quests.Count)
        {
            currentQuest = quests[currentQuestIndex];
            Debug.Log($"[Quest] Новая задача: {currentQuest.description}");
            
            // Уведомляем UI
            QuestUI ui = FindObjectOfType<QuestUI>();
            if (ui != null) ui.UpdateQuestDisplay(currentQuest);
            
            // Блокируем/разблокируем объекты в зависимости от типа задачи
            UpdateInteractableStates();
        }
        else
        {
            Debug.Log("[Quest] Все задачи выполнены!");
            currentQuest = null;
            
            QuestUI ui = FindObjectOfType<QuestUI>();
            if (ui != null) ui.HideQuest();
        }
    }

    /// <summary>
    /// Блокирует/разблокирует интерактивные объекты в зависимости от текущей задачи
    /// </summary>
    void UpdateInteractableStates()
    {
        InteractableObject[] allInteractables = FindObjectsByType<InteractableObject>(FindObjectsSortMode.None);
        
        foreach (InteractableObject obj in allInteractables)
        {
            if (obj.data == null) continue;
            
            bool shouldBeActive = false;
            
            // Если текущая задача — взаимодействие с предметами
            if (currentQuest.questType == QuestType.Interact)
            {
                shouldBeActive = currentQuest.targetIds.Contains(obj.data.journalEntryId);
            }
            // Если текущая задача — диалог с NPC
            else if (currentQuest.questType == QuestType.Talk)
            {
                shouldBeActive = currentQuest.targetIds.Contains(obj.data.journalEntryId);
            }
            
            obj.SetInteractable(shouldBeActive);
        }
    }

    public Quest GetCurrentQuest() => currentQuest;
    public bool HasActiveQuest() => currentQuest != null && !currentQuest.IsCompleted();
}