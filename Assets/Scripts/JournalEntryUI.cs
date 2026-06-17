using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JournalEntryUI : MonoBehaviour
{
    [Header("UI Элементы")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI contentText;
    public Image iconImage; // Опционально: иконка предмета

    /// <summary>
    /// Заполнить запись данными
    /// </summary>
    public void Setup(JournalEntry entry)
    {
        if (titleText != null)
            titleText.text = entry.title;
        
        if (contentText != null)
            contentText.text = entry.content;
        
        if (iconImage != null && entry.portrait != null)
            iconImage.sprite = entry.portrait;
    }
}