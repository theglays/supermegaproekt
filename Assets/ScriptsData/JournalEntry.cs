using System;
using UnityEngine;

[Serializable]
public class JournalEntry
{
    public string entryId;              // Уникальный ID (напр. "npc_maria_001")
    public string title;                // Заголовок (имя персонажа/предмета)
    public string content;              // Текст описания
    public Sprite portrait;             // Фотография (для NPC)
    public bool isNpc;                  // true = персонаж, false = предмет
    public bool isUnlocked;             // Открыта ли запись
    public float unlockTimestamp;       // Время открытия (для сортировки)

    // Конструктор для удобного создания
    public JournalEntry(string id, string title, string content, Sprite portrait, bool isNpc)
    {
        entryId = id;
        this.title = title;
        this.content = content;
        this.portrait = portrait;
        this.isNpc = isNpc;
        isUnlocked = true;
        unlockTimestamp = Time.time;
    }
}