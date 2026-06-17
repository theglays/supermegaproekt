using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterCard : MonoBehaviour
{
    [Header("UI Элементы")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI bioText;
    public Image portraitImage;

    /// <summary>
    /// Заполнить карточку данными персонажа
    /// </summary>
    public void Setup(JournalEntry entry)
    {
        if (nameText != null)
            nameText.text = entry.title;
        
        if (bioText != null)
            bioText.text = entry.content;
        
        if (portraitImage != null && entry.portrait != null)
            portraitImage.sprite = entry.portrait;
    }
}