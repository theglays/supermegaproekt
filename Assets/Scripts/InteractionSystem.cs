using UnityEngine;
using TMPro;
using System.Collections;

public class InteractionSystem : MonoBehaviour
{
    [Header("Дистанция")]
    public float interactionDistance = 2f;

    [Header("Ссылки на UI")]
    public GameObject promptE;           // Объект с буквой "E"
    public CanvasGroup descriptionGroup; // Canvas Group текста описания
    public TextMeshProUGUI infoText;     // Сам текст

    [Header("Настройки анимации")]
    public float fadeSpeed = 2f;         // Скорость появления/исчезновения
    public float displayDuration = 3f;   // Сколько текст висит полностью видимым

    private Transform currentInteractable = null;
    private bool isDescriptionActive = false;

    void Update()
    {
        FindNearest();

        // Буква E видна только если мы рядом И описание не проигрывается
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
            // Останавливаем текущую анимацию, если она идет, чтобы начать новую
            StopAllCoroutines();
            StartCoroutine(FadeDescription(interactable.data.interactionDescription));
        }
    }

    IEnumerator FadeDescription(string message)
    {
        isDescriptionActive = true;
        infoText.text = message;

        // 1. Плавное появление (Alpha 0 -> 1)
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            descriptionGroup.alpha = Mathf.Lerp(0, 1, t);
            yield return null;
        }
        descriptionGroup.alpha = 1f;

        // 2. Ожидание
        yield return new WaitForSeconds(displayDuration);

        // 3. Плавное исчезновение (Alpha 1 -> 0)
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
}