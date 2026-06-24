using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueTopic", menuName = "Game/Dialogue Topic")]
public class DialogueTopic : ScriptableObject
{
    public string topicName;           // Название темы (для кнопки)
    public string topicDescription;    // Описание (опционально)
    public DialogueData dialogue;      // Сам диалог
    public bool isCompleted = false;   // Пройдена ли тема
}