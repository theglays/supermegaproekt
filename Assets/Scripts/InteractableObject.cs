using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public ItemData data;
    
    [HideInInspector] public bool isInteractable = true;
    [HideInInspector] public bool hasCompletedInteraction = false; // 🔥 НОВОЕ ПОЛЕ

    public void SetInteractable(bool active)
    {
        isInteractable = active;
    }

    public void MarkAsCompleted()
    {
        hasCompletedInteraction = true;
    }

    public bool HasCompleted() => hasCompletedInteraction;
}