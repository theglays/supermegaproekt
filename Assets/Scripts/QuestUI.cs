using UnityEngine;
using TMPro;

public class QuestUI : MonoBehaviour
{
    [Header("UI элементы")]
    public GameObject questPanel;
    public TextMeshProUGUI questText;
    public TextMeshProUGUI progressText;

    void Start()
    {
            DontDestroyOnLoad(gameObject);
        if (questPanel != null)
            questPanel.SetActive(false);
    }

    public void UpdateQuestDisplay(Quest quest)
    {
        if (quest == null)
        {
            HideQuest();
            return;
        }

        if (questPanel != null)
            questPanel.SetActive(true);

        if (questText != null)
            questText.text = quest.description;

        if (progressText != null)
        {
            if (quest.targetIds.Count > 0)
                progressText.text = $"{quest.completedTargets.Count}/{quest.targetIds.Count}";
            else
                progressText.text = "";
        }
    }

    public void HideQuest()
    {
        if (questPanel != null)
            questPanel.SetActive(false);
    }
}