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
        
        // 🔥 ВЫЗЫВАЕМ С ЗАДЕРЖКОЙ чтобы объекты успели загрузиться
        Invoke("UpdateInteractableStates", 0.5f);
    }
}
    /// Вызывается при взаимодействии с предметом/NPC
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
            
            // Уведомляем UI
            QuestUI ui = FindObjectOfType<QuestUI>();
            if (ui != null) ui.UpdateQuestDisplay(currentQuest);
            
            // Блокируем/разблокируем объекты
            UpdateInteractableStates();

            // 🔥 ВОСПРОИЗВЕДЕНИЕ ЗВУКА ДЛЯ НОВОЙ ЗАДАЧИ
            PlayQuestSound();
        }
        else
        {
            Debug.Log("[Quest] Все задачи выполнены!");
            currentQuest = null;
            
            QuestUI ui = FindObjectOfType<QuestUI>();
            if (ui != null) ui.HideQuest();
        }
    }

    // 🔥 НОВЫЙ МЕТОД: Воспроизведение звука задачи
    void PlayQuestSound()
    {
        if (AudioManager.Instance != null)
        {
            // Замени "Quest_New" на точное имя звука в твоем AudioManager
            AudioManager.Instance.PlaySFX("Quest_New"); 
            Debug.Log("[Quest] Звук новой задачи воспроизведен");
        }
        else
        {
            Debug.LogWarning("[Quest] AudioManager не найден!");
        }
    }
    /// <summary>
    /// Блокирует/разблокирует интерактивные объекты в зависимости от текущей задачи
    /// </summary>
void UpdateInteractableStates()
{
    Debug.Log($"[Quest] UpdateInteractableStates() начала работу");
    Debug.Log($"[Quest] Текущая задача: {currentQuest?.description ?? "Нет активной задачи"}");
    
    InteractableObject[] allInteractables = FindObjectsByType<InteractableObject>(FindObjectsSortMode.None);
    
    foreach (InteractableObject obj in allInteractables)
    {
        if (obj.data == null)
        {
            obj.SetInteractable(false);
            continue;
        }
        
        string objId = obj.data.journalEntryId;
        bool shouldBeActive = false;
        
        // Если это NPC и он уже завершил диалог — всегда деактивируем
        if (obj.data.isNpcEntry && obj.HasCompleted())
        {
            obj.SetInteractable(false);
            Debug.Log($"[Quest] {obj.name} — NPC завершил диалог, деактивирован");
            continue;
        }
        
        if (currentQuest == null)
        {
            obj.SetInteractable(false);
            continue;
        }
        
        //  СПЕЦИАЛЬНАЯ ЛОГИКА ДЛЯ ТРЕТЬЕЙ ЗАДАЧИ "Найдите выход"
        if (currentQuest.questId == "quest_find_exit")
        {
            // Активируем ВСЕ предметы (кроме NPC), но цель только дверь
            if (!obj.data.isNpcEntry)
            {
                shouldBeActive = true;
            }
        }
        else
        {
            // Обычная логика: активируем только объекты из списка целей
            shouldBeActive = currentQuest.targetIds.Contains(objId);
        }
        
        obj.SetInteractable(shouldBeActive);
        // Debug.Log($"[Quest] {obj.name} (ID: {objId}) установлен как interactable: {shouldBeActive}");
    }
    
    Debug.Log("[Quest] UpdateInteractableStates() завершена");
}
    public Quest GetCurrentQuest() => currentQuest;
    public bool HasActiveQuest() => currentQuest != null && !currentQuest.IsCompleted();
}