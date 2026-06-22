using System;
using UnityEngine;

[Serializable]
public class DialogueLine
{
    public string speakerName;
    public Sprite portrait;
    [TextArea(3, 5)]
    public string text;
    
    [Tooltip("Имя звука для этой фразы (как в AudioManager). Оставь пустым для стандартного")]
    public string voiceSoundName; // 🔥 НОВОЕ ПОЛЕ
}

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Game/Dialogue")]
public class DialogueData : ScriptableObject
{
    public DialogueLine[] lines;
}