using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "ScriptableObjects/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("Настройки предмета")]
    public string itemName;                  // Имя предмета (например, "Карточный стол")
    
    [TextArea(3, 10)]
    public string interactionDescription;    // Атмосферное описание про Петербург или штосс

    [Tooltip("Имя звука для этого предмета (как в AudioManager). Оставь пустым для стандартного 'Interact'")]
    public string customSoundName;

   [Header("Дневник")]
[Tooltip("Добавлять запись в дневник при взаимодействии?")]
public bool addToJournal = false;

[Tooltip("Уникальный ID записи (должен быть уникальным)")]
public string journalEntryId;

[Tooltip("Заголовок в дневнике")]
public string journalTitle;

[Tooltip("Текст записи в дневнике (ОТДЕЛЬНЫЙ от interactionDescription)")]
[TextArea(3, 10)]
public string journalContent;

[Tooltip("Фотография для карточки персонажа (опционально)")]
public Sprite portraitSprite;

[Tooltip("Это запись о персонаже? (true = NPC, false = предмет)")]
public bool isNpcEntry;

[Tooltip("Имя блока в дневнике, который нужно активировать (например, Entry_Block_2)")]
public string journalBlockName; // 🔥 НОВОЕ ПОЛЕ

[Header("Диалог (для NPC)")]
[Tooltip("Файл диалога (только для NPC)")]
public DialogueData dialogue;
}