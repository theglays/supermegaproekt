using System;
using UnityEngine;

[Serializable]
public class DialogueLine
{
    public string speakerName;        // Имя говорящего (опционально)
    public Sprite portrait;           // Портрет говорящего
    [TextArea(3, 5)]
    public string text;               // Текст реплики
}

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Game/Dialogue")]
public class DialogueData : ScriptableObject
{
    public DialogueLine[] lines;      // Массив реплик
}