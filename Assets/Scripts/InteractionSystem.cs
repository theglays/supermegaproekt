using UnityEngine;
using TMPro;
using System.Collections;

public class InteractionSystem : MonoBehaviour
{
    [Header("Дистанция")]
    public float interactionDistance = 2f;

    [Header("Ссылки на UI")]
    public GameObject promptE;
    public CanvasGroup descriptionGroup;
    public TextMeshProUGUI infoText;

    [Header("Настройки анимации")]
    public float fadeSpeed = 2f;
    public float displayDuration = 3f;

    private Transform currentInteractable = null;
    private bool isDescriptionActive = false;

    void Update()
    {
        FindNearest();

        if (currentInteractable != null && !isDescriptionActive)
        {
            // 🔥 Проверяем, активен ли объект 🔥
            InteractableObject interactable = currentInteractable.GetComponent<InteractableObject>();
            if (interactable != null && !interactable.isInteractable)
            {
                promptE.SetActive(false);
                return;
            }
            
            promptE.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
        }
        else
        {
            promptE.SetActive(false);
        }
    }

void FindNearest()
{
    InteractableObject[] interactables = FindObjectsByType<InteractableObject>(FindObjectsSortMode.None);
    currentInteractable = null;
    float minDist = Mathf.Infinity;

    Debug.Log($"[Interaction] Найдено объектов: {interactables.Length}");

    foreach (InteractableObject obj in interactables)
    {
        // Проверяем, активен ли объект
        if (!obj.isInteractable)
        {
            Debug.Log($"[Interaction] {obj.name} неактивен, пропускаем");
            continue;
        }
        
        float dist = Vector3.Distance(transform.position, obj.transform.position);
        Debug.Log($"[Interaction] {obj.name}: дистанция={dist:F2}, interactionDistance={interactionDistance}");
        
        if (dist < minDist && dist < interactionDistance)
        {
            minDist = dist;
            currentInteractable = obj.transform;
            Debug.Log($"[Interaction] ✅ {obj.name} выбран как ближайший (дистанция: {dist:F2})");
        }
    }
    
    if (currentInteractable != null)
    {
        Debug.Log($"[Interaction] Итоговый объект: {currentInteractable.name}");
    }
    else
    {
        Debug.Log("[Interaction] Итоговый объект: НЕТ");
    }
}

 void Interact()
{
    InteractableObject interactable = currentInteractable.GetComponent<InteractableObject>();
    if (interactable != null && interactable.data != null)
    {
        // Если это NPC и он уже завершил диалог — ничего не делаем
        if (interactable.data.isNpcEntry && interactable.HasCompleted())
        {
            Debug.Log($"[Interaction] NPC '{interactable.data.itemName}' уже завершил диалог");
            return;
        }

        StopAllCoroutines();
        StartCoroutine(FadeDescription(interactable.data));
        
        PlayInteractionSound(interactable.data);
        AddToJournal(interactable.data);
        
// Проверяем, является ли это дверью выхода
if (interactable.data.journalEntryId == "door_exit")
{
    Debug.Log("[Interaction] 🚪 Дверь выхода! Начинаем переход...");
    
    // Завершаем квест
    if (QuestManager.Instance != null)
    {
        QuestManager.Instance.OnTargetInteracted(interactable.data.journalEntryId);
    }
    
    // Проверяем LevelTransition
    if (LevelTransition.Instance != null)
    {
        Debug.Log("[Interaction] ✅ LevelTransition.Instance найден");
        
        if (LevelTransition.Instance.fadeImage != null)
        {
            Debug.Log("[Interaction] ✅ fadeImage назначен");
        }
        else
        {
            Debug.LogError("[Interaction] ❌ fadeImage НЕ назначен в Inspector!");
        }
        
        // Запускаем переход через небольшую задержку
        Invoke("TransitionToNextLevel", 1.5f);
    }
    else
    {
        Debug.LogError("[Interaction] ❌ LevelTransition.Instance НЕ найден!");
    }
    
    return; // 🔥 ВАЖНО: выходим из метода, чтобы не выполнялся остальной код
}
        
   // 🔥 Проверяем, является ли это NPC
if (interactable.data.isNpcEntry)
{
    // 🔥 СНАЧАЛА: NPC с несколькими темами
    if (interactable.data.hasMultipleTopics && 
        interactable.data.dialogueTopics != null && 
        interactable.data.dialogueTopics.Length > 0)
    {
        Debug.Log($"[Interaction] 🎭 NPC с темами! Показываем выбор...");
        ShowTopicSelection(interactable);
    }
    // 🔥 ПОТОМ: обычный NPC с одним диалогом
    else if (interactable.data.dialogue != null)
    {
        Debug.Log($"[Interaction] 💬 Обычный NPC, запускаем диалог");
        
        // Запускаем диалог и передаем действие, которое выполнится ПОСЛЕ его окончания
        DialogueSystem.Instance.StartDialogue(interactable.data.dialogue, () => 
        {
            // Помечаем NPC как завершившего диалог
            interactable.MarkAsCompleted();
            
            // Завершаем квест (и запускаем 3-ю задачу) ТОЛЬКО сейчас
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.OnTargetInteracted(interactable.data.journalEntryId);
            }
            
            // АКТИВИРУЕМ БЛОК В ДНЕВНИКЕ через массив прямых ссылок
            if (!string.IsNullOrEmpty(interactable.data.journalBlockName))
            {
                bool blockFound = false;
                
                if (journalBlocks != null)
                {
                    foreach (GameObject block in journalBlocks)
                    {
                        if (block != null && block.name == interactable.data.journalBlockName)
                        {
                            block.SetActive(true);
                            Debug.Log($"[Interaction] ✅ Активирован блок дневника: {interactable.data.journalBlockName}");
                            blockFound = true;
                            
                            JournalUI journalUI = FindObjectOfType<JournalUI>();
                            if (journalUI != null)
                            {
                                journalUI.ShowNotification();
                            }
                            
                            break;
                        }
                    }
                }
                
                if (!blockFound)
                {
                    Debug.LogWarning($"[Interaction] ❌ Блок дневника '{interactable.data.journalBlockName}' не найден в массиве journalBlocks!");
                }
            }
            
            Debug.Log("[Interaction] Диалог окончен, квест обновлен.");
        });
    }
    // 🔥 ИНАЧЕ: NPC без диалога (ошибка)
    else
    {
        Debug.LogError($"[Interaction] ❌ NPC '{interactable.data.itemName}' не имеет ни тем, ни диалога!");
    }
}
else
{
    // Обычное взаимодействие с предметом
    if (QuestManager.Instance != null && !string.IsNullOrEmpty(interactable.data.journalEntryId))
    {
        QuestManager.Instance.OnTargetInteracted(interactable.data.journalEntryId);
    }
}
    }
}

void TransitionToNextLevel()
{
    Debug.Log("[Interaction] 🔄 Вызываем TransitionToNextLevel()...");
    
    if (LevelTransition.Instance != null)
    {
        Debug.Log("[Interaction] ✅ Запускаем TransitionToLevel('Level2')");
        LevelTransition.Instance.TransitionToLevel("Level2");
    }
    else
    {
        Debug.LogError("[Interaction] ❌ LevelTransition.Instance не найден!");
    }
}
void PlayInteractionSound(ItemData data)
{
    if (AudioManager.Instance == null) return;
    
    // Если указан кастомный звук — используем его
    if (!string.IsNullOrEmpty(data.customSoundName))
    {
        AudioManager.Instance.PlaySFX(data.customSoundName, currentInteractable.position);
    }
    else
    {
        // Иначе используем стандартный звук "Interact"
        AudioManager.Instance.PlaySFX("Interact", currentInteractable.position);
    }
}

    IEnumerator FadeDescription(ItemData data)
    {
        isDescriptionActive = true;
        infoText.text = data.interactionDescription;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            descriptionGroup.alpha = Mathf.Lerp(0, 1, t);
            yield return null;
        }
        descriptionGroup.alpha = 1f;

        yield return new WaitForSeconds(displayDuration);

        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            descriptionGroup.alpha = Mathf.Lerp(1, 0, t);
            yield return null;
        }
        descriptionGroup.alpha = 0f;

        isDescriptionActive = false;
    }

  [Header("Ссылки на блоки дневника")]
[Tooltip("Перетащи сюда все Entry_Block_1, Entry_Block_2 и т.д. из Hierarchy")]
public GameObject[] journalBlocks;

void AddToJournal(ItemData data)
{
    // Проверяем, должен ли этот предмет добавлять запись в дневник
    if (!data.addToJournal)
    {
        return;
    }
    
    Debug.Log($"[Journal] Пытаемся активировать блок для предмета: {data.itemName}, ID: {data.journalEntryId}");
    
    if (journalBlocks == null || journalBlocks.Length == 0)
    {
        Debug.LogError("[Journal] Массив journalBlocks пуст! Перетащи блоки в Inspector у InteractionSystem!");
        return;
    }
    
    // Ищем блок по имени
    string blockName = $"Entry_Block_{data.journalEntryId}";
    bool found = false;
    
    foreach (GameObject block in journalBlocks)
    {
        if (block != null && block.name == blockName)
        {
            block.SetActive(true);
            Debug.Log($"[Journal] ✅ Активирован блок: {blockName}");
            found = true;
            
            // Обновляем текст, если нужно
            TextMeshProUGUI textComp = block.GetComponentInChildren<TextMeshProUGUI>();
            if (textComp != null && !string.IsNullOrEmpty(data.journalContent))
            {
                textComp.text = data.journalContent;
            }
            
            // 🔥 ПОКАЗЫВАЕМ УВЕДОМЛЕНИЕ О НОВОЙ ЗАПИСИ
            JournalUI journalUI = FindObjectOfType<JournalUI>();
            if (journalUI != null)
            {
                journalUI.ShowNotification();
            }
            
            break;
        }
    }
    
    if (!found)
    {
        Debug.LogWarning($"[Journal] ❌ Блок '{blockName}' не найден в массиве journalBlocks!");
    }
}




void HandleNPCInteraction(InteractableObject interactable)
{
    Debug.Log("[Interaction] HandleNPCInteraction() ВЫЗВАН");
    
    if (interactable.data == null)
    {
        Debug.LogError("[Interaction] ❌ data = NULL!");
        return;
    }

    // 🔥 СНАЧАЛА проверяем темы
    if (interactable.data.hasMultipleTopics && interactable.data.dialogueTopics != null && interactable.data.dialogueTopics.Length > 0)
    {
        Debug.Log("[Interaction] ✅ У NPC есть темы!");
        // ... показываем темы
    }
    else if (interactable.data.dialogue != null)
    {
        Debug.Log("[Interaction] Обычный диалог");
        // ... обычный диалог
    }
    else
    {
        Debug.LogError("[Interaction] ❌ Нет ни тем, ни диалога!");
    }
}

void ShowTopicSelection(InteractableObject interactable)
{
    Debug.Log("========================================");
    Debug.Log("[Interaction] ShowTopicSelection() ВЫЗВАН");
    
    if (TopicSelectionUI.Instance == null)
    {
        Debug.LogError("[Interaction] ❌ TopicSelectionUI.Instance = NULL!");
        Debug.LogError("[Interaction] Создай объект с TopicSelectionUI в сцене!");
        return;
    }

    Debug.Log($"[Interaction] ✅ TopicSelectionUI.Instance найден");
    Debug.Log($"[Interaction] Тем: {interactable.data.dialogueTopics?.Length ?? 0}");

    TopicSelectionUI.Instance.Show(interactable.data.dialogueTopics, (topicIndex) =>
    {
        Debug.Log($"[Interaction] Callback: выбрана тема #{topicIndex}");
        StartTopicDialogue(interactable, topicIndex);
    });
    
    Debug.Log("[Interaction] Show() вызван");
    Debug.Log("========================================");
}

void StartTopicDialogue(InteractableObject interactable, int topicIndex)
{
    if (topicIndex < 0 || topicIndex >= interactable.data.dialogueTopics.Length) return;

    DialogueTopic topic = interactable.data.dialogueTopics[topicIndex];
    if (topic == null || topic.dialogue == null)
    {
        Debug.LogError($"[Interaction] Тема {topicIndex} или её диалог = NULL!");
        return;
    }

    Debug.Log($"[Interaction] Начинаем диалог по теме: {topic.topicName}");

    if (DialogueSystem.Instance != null)
    {
        DialogueSystem.Instance.StartDialogue(topic.dialogue, () =>
        {
            // Callback после завершения диалога
            Debug.Log($"[Interaction] Диалог по теме '{topic.topicName}' завершён");
            
            // Помечаем тему как пройденную
            topic.isCompleted = true;
            
            // Проверяем, все ли темы пройдены
            bool allCompleted = true;
            foreach (var t in interactable.data.dialogueTopics)
            {
                if (!t.isCompleted)
                {
                    allCompleted = false;
                    break;
                }
            }

            if (allCompleted)
            {
                Debug.Log("[Interaction] ✅ ВСЕ темы пройдены! Засчитываем задачу");
                interactable.MarkAsCompleted();
                if (QuestManager.Instance != null)
                {
                    QuestManager.Instance.OnTargetInteracted(interactable.data.journalEntryId);
                }
            }
            else
            {
                Debug.Log("[Interaction] ⏳ Есть непройденные темы");
            }
        });
    }
}


}