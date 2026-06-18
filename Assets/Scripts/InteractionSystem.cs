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
            
            // 🔥 ДОБАВЛЯЕМ ЗАПИСЬ В ДНЕВНИК 🔥
            AddToJournal(interactable.data);
        }
    }

    IEnumerator FadeDescription(ItemData data)
    {
        isDescriptionActive = true;
        infoText.text = data.interactionDescription;

        // Плавное появление
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            descriptionGroup.alpha = Mathf.Lerp(0, 1, t);
            yield return null;
        }
        descriptionGroup.alpha = 1f;

        // Ожидание
        yield return new WaitForSeconds(displayDuration);

        // Плавное исчезновение
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

    // 🔥 НОВЫЙ МЕТОД: Добавление записи в дневник 🔥
    void AddToJournal(ItemData data)
    {
        // Проверяем, есть ли данные для дневника
        if (string.IsNullOrEmpty(data.journalEntryId))
        {
            Debug.Log($"[Journal] У предмета '{data.itemName}' нет journalEntryId, пропускаем");
            return;
        }

        if (SaveManager.Instance == null)
        {
            Debug.LogWarning("[Journal] SaveManager не найден!");
            return;
        }

        // Создаём запись
        JournalEntry entry = new JournalEntry(
            id: data.journalEntryId,
            title: !string.IsNullOrEmpty(data.journalTitle) ? data.journalTitle : data.itemName,
            content: !string.IsNullOrEmpty(data.journalContent) ? data.journalContent : data.interactionDescription,
            portrait: data.portraitSprite,
            isNpc: data.isNpcEntry
        );

        // Пытаемся добавить (вернёт false, если уже есть)
        bool wasAdded = SaveManager.Instance.UnlockEntry(entry);

        if (wasAdded)
        {
            Debug.Log($"[Journal] ✅ Новая запись добавлена: {entry.title}");
            
            // Если дневник открыт — обновляем его сразу
            JournalUI jui = FindObjectOfType<JournalUI>();
            if (jui != null && jui.gameObject.activeSelf)
                jui.Refresh();
        }
        else
        {
            Debug.Log($"[Journal] Запись '{entry.title}' уже существует, пропускаем");
        }
    }
}