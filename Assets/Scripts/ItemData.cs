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
    [Tooltip("Уникальный ID записи (должен быть уникальным для каждого предмета/NPC)")]
    public string journalEntryId;
    
    [Tooltip("Заголовок в дневнике (если пусто, берётся itemName)")]
    public string journalTitle;
    
    [Tooltip("Полный текст записи в дневнике (если пусто, берётся interactionDescription)")]
    [TextArea(3, 10)]
    public string journalContent;
    
    [Tooltip("Фотография для карточки персонажа (опционально)")]
    public Sprite portraitSprite;
    
    [Tooltip("Это запись о персонаже? (true = NPC, false = предмет)")]
    public bool isNpcEntry;
}