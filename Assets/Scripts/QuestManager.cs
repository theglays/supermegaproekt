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
        
        // 🔥 Принудительно обновляем состояние объектов
        UpdateInteractableStates();
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
    Debug.Log($"[Quest] Задача завершена! Текущий индекс: {currentQuestIndex}");
    
    currentQuestIndex++;
    
    if (currentQuestIndex < quests.Count)
    {
        currentQuest = quests[currentQuestIndex];
        Debug.Log($"[Quest] Новая задача: {currentQuest.description}");
        Debug.Log($"[Quest] Тип задачи: {currentQuest.questType}");
        Debug.Log($"[Quest] Целей в задаче: {currentQuest.targetIds.Count}");
        
        // Выводим все targetIds
        for (int i = 0; i < currentQuest.targetIds.Count; i++)
        {
            Debug.Log($"[Quest] Target ID {i}: {currentQuest.targetIds[i]}");
        }
        
        // Уведомляем UI
        QuestUI ui = FindObjectOfType<QuestUI>();
        if (ui != null) ui.UpdateQuestDisplay(currentQuest);
        
        // 🔥 Блокируем/разблокируем объекты
        Debug.Log("[Quest] Вызываем UpdateInteractableStates()...");
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
    Debug.Log("[Quest] UpdateInteractableStates() начала работу");
    Debug.Log($"[Quest] Текущая задача: {currentQuest?.description ?? "Нет активной задачи"}");
    
    InteractableObject[] allInteractables = FindObjectsByType<InteractableObject>(FindObjectsSortMode.None);
    Debug.Log($"[Quest] Найдено интерактивных объектов: {allInteractables.Length}");
    
    foreach (InteractableObject obj in allInteractables)
    {
        if (obj.data == null)
        {
            Debug.LogWarning($"[Quest] У объекта {obj.name} нет ItemData!");
            obj.SetInteractable(false); // Деактивируем объекты без данных
            continue;
        }
        
        string objId = obj.data.journalEntryId;
        
        // 🔥 Если нет активной задачи — деактивируем всё
        if (currentQuest == null)
        {
            obj.SetInteractable(false);
            Debug.Log($"[Quest] {obj.name} (ID: {objId}) деактивирован (нет активной задачи)");
            continue;
        }
        
        bool shouldBeActive = false;
        
        // Проверяем, есть ли этот объект в списке целей текущей задачи
        if (currentQuest.targetIds.Contains(objId))
        {
            shouldBeActive = true;
        }
        
        obj.SetInteractable(shouldBeActive);
        Debug.Log($"[Quest] {obj.name} (ID: {objId}) установлен как interactable: {shouldBeActive}");
    }
    
    Debug.Log("[Quest] UpdateInteractableStates() завершена");
}
    public Quest GetCurrentQuest() => currentQuest;
    public bool HasActiveQuest() => currentQuest != null && !currentQuest.IsCompleted();
}