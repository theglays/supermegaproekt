using UnityEngine;
using TMPro;
using System.Collections;

public class InteractionSystem : MonoBehaviour
{
    [Header("Дистанция")]
    public float interactionDistance = 2f;

    [Header("Ссылки на UI")]
    public GameObject promptE;
    public CanvasGroup descriptionGroup;
    public TextMeshProUGUI infoText;

    [Header("Настройки анимации")]
    public float fadeSpeed = 2f;
    public float displayDuration = 3f;

    private Transform currentInteractable = null;
    private bool isDescriptionActive = false;

    void Update()
    {
        FindNearest();

        if (currentInteractable != null && !isDescriptionActive)
        {
            // 🔥 Проверяем, активен ли объект 🔥
            InteractableObject interactable = currentInteractable.GetComponent<InteractableObject>();
            if (interactable != null && !interactable.isInteractable)
            {
                promptE.SetActive(false);
                return;
            }
            
            promptE.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
        }
        else
        {
            promptE.SetActive(false);
        }
    }

    void FindNearest()
    {
        InteractableObject[] interactables = FindObjectsByType<InteractableObject>(FindObjectsSortMode.None);
        currentInteractable = null;
        float minDist = Mathf.Infinity;

        foreach (InteractableObject obj in interactables)
        {
            float dist = Vector3.Distance(transform.position, obj.transform.position);
            if (dist < minDist && dist < interactionDistance)
            {
                minDist = dist;
                currentInteractable = obj.transform;
            }
        }
    }

   void Interact()
{
    InteractableObject interactable = currentInteractable.GetComponent<InteractableObject>();
    if (interactable != null && interactable.data != null)
    {
        StopAllCoroutines();
        StartCoroutine(FadeDescription(interactable.data));
        
        // 🔥 ВОСПРОИЗВЕДЕНИЕ ЗВУКА 🔥
        PlayInteractionSound(interactable.data);
        
        AddToJournal(interactable.data);
        
        // Уведомляем QuestManager о взаимодействии
        if (QuestManager.Instance != null && !string.IsNullOrEmpty(interactable.data.journalEntryId))
        {
            QuestManager.Instance.OnTargetInteracted(interactable.data.journalEntryId);
        }
    }
}

// 🔥 НОВЫЙ МЕТОД: Воспроизведение звука взаимодействия 🔥
void PlayInteractionSound(ItemData data)
{
    if (AudioManager.Instance == null) return;
    
    // Если указан кастомный звук — используем его
    if (!string.IsNullOrEmpty(data.customSoundName))
    {
        AudioManager.Instance.PlaySFX(data.customSoundName, currentInteractable.position);
    }
    else
    {
        // Иначе используем стандартный звук "Interact"
        AudioManager.Instance.PlaySFX("Interact", currentInteractable.position);
    }
}

    IEnumerator FadeDescription(ItemData data)
    {
        isDescriptionActive = true;
        infoText.text = data.interactionDescription;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            descriptionGroup.alpha = Mathf.Lerp(0, 1, t);
            yield return null;
        }
        descriptionGroup.alpha = 1f;

        yield return new WaitForSeconds(displayDuration);

        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            descriptionGroup.alpha = Mathf.Lerp(1, 0, t);
            yield return null;
        }
        descriptionGroup.alpha = 0f;

        isDescriptionActive = false;
    }

    void AddToJournal(ItemData data)
    {
        if (string.IsNullOrEmpty(data.journalEntryId)) return;
        if (SaveManager.Instance == null) return;

        JournalEntry entry = new JournalEntry(
            id: data.journalEntryId,
            title: !string.IsNullOrEmpty(data.journalTitle) ? data.journalTitle : data.itemName,
            content: !string.IsNullOrEmpty(data.journalContent) ? data.journalContent : data.interactionDescription,
            portrait: data.portraitSprite,
            isNpc: data.isNpcEntry
        );

        bool wasAdded = SaveManager.Instance.UnlockEntry(entry);

        if (wasAdded)
        {
            Debug.Log($"[Journal] ✅ Новая запись: {entry.title}");
            JournalUI jui = FindObjectOfType<JournalUI>();
            if (jui != null && jui.gameObject.activeSelf)
                jui.Refresh();
        }
    }
}