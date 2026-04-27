using UnityEngine;
using TMPro;

public class InteractionSystem : MonoBehaviour
{
    public float interactionDistance = 2.5f;
    public GameObject promptUI; // Объект с текстом "E"
    public TextMeshProUGUI promptText;
    
    private Transform currentInteractable = null;

    void Update()
    {
        // Ищем ближайший объект с тегом Interactable
        GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");
        currentInteractable = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject obj in interactables)
        {
            float dist = Vector3.Distance(transform.position, obj.transform.position);
            if (dist < minDist && dist < interactionDistance)
            {
                minDist = dist;
                currentInteractable = obj.transform;
            }
        }

        // Показываем/скрываем подсказку
        if (currentInteractable != null)
        {
            promptUI.SetActive(true);
            promptText.text = "E";
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Взаимодействие с: " + currentInteractable.name);
                // Сюда можно добавить вызов методов на объекте:
                // currentInteractable.GetComponent<YourInteractionScript>().Interact();
            }
        }
        else
        {
            promptUI.SetActive(false);
        }
    }
}