using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "ScriptableObjects/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("Настройки предмета")]
    public string itemName;                  // Имя предмета (например, "Карточный стол")
    
    [TextArea(3, 10)]
    public string interactionDescription;    // Атмосферное описание про Петербург или штосс
}
