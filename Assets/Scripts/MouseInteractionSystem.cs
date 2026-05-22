using UnityEngine;
using TMPro;
using System.Collections;

public class MouseInteractionSystem : MonoBehaviour
{
    [Header("Настройки луча")]
    public Camera mainCamera;
    public LayerMask interactableLayer; // Слой для предметов (например, "Obstacles")
    public float maxDistance = 10f;     // Дальность клика

    [Header("Ссылки на UI")]
    public CanvasGroup descriptionGroup; // Сюда пойдет Text table (через него идет фейд)
    public TextMeshProUGUI infoText;     // Сюда тоже Text table (в него пишется текст)

    [Header("Настройки анимации текста")]
    public float fadeSpeed = 2f;         
    public float displayDuration = 4f;   

    private InteractableObject lastHoveredObject = null;
    private bool isDescriptionActive = false;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        
        // В начале игры прячем текст
        if (descriptionGroup != null) descriptionGroup.alpha = 0f;
    }

    void Update()
    {
        HandleMouseHoverAndClick();
    }

    void HandleMouseHoverAndClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance, interactableLayer))
        {
            InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();

            if (interactable != null)
            {
                // Подсветка при наведении
                if (lastHoveredObject != interactable)
                {
                    ClearLastHover();
                    lastHoveredObject = interactable;
                    HighlightObject(interactable, true);
                }

                // Клик ЛКМ
                if (Input.GetMouseButtonDown(0)) 
                {
                    if (interactable.data != null)
                    {
                        StopAllCoroutines();
                        StartCoroutine(FadeDescription(interactable.data.interactionDescription));
                    }
                }
                return;
            }
        }
        ClearLastHover();
    }

    void HighlightObject(InteractableObject obj, bool enable)
    {
        Renderer r = obj.GetComponent<Renderer>();
        if (r != null)
        {
            if (enable)
                r.material.color = new Color(1.2f, 1.2f, 1.2f); // Подсветка
            else
                r.material.color = Color.white; // Обычный цвет
        }
    }

    void ClearLastHover()
    {
        if (lastHoveredObject != null)
        {
            HighlightObject(lastHoveredObject, false);
            lastHoveredObject = null;
        }
    }

    IEnumerator FadeDescription(string message)
    {
        isDescriptionActive = true;
        infoText.text = message;

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
}