using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Interaction/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName; // Название объекта
    [TextArea(3, 10)]
    public string interactionDescription; // Текст, который появится на Canvas
}