using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Quest
{
    public string questId;                    // Уникальный ID задачи
    public string description;                // Текст задачи (например, "Изучите комнату")
    
    [Tooltip("Список ID предметов/NPC, которые нужно активировать для выполнения")]
    public List<string> targetIds = new List<string>();
    
    [Tooltip("Тип задачи: 'interact' = взаимодействие с предметами, 'talk' = диалог с NPC")]
    public QuestType questType;
    
    [HideInInspector] public List<string> completedTargets = new List<string>();
    
    public bool IsCompleted()
    {
        return completedTargets.Count >= targetIds.Count;
    }
    
    public float GetProgress()
    {
        if (targetIds.Count == 0) return 0f;
        return (float)completedTargets.Count / targetIds.Count;
    }
}

public enum QuestType
{
    Interact,  // Взаимодействие с предметами
    Talk       // Диалог с NPC
}