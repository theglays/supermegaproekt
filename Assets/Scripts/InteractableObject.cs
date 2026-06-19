using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public ItemData data;
    
    [HideInInspector] public bool isInteractable = true;

    /// <summary>
    /// Активировать/деактивировать взаимодействие
    /// </summary>
    public void SetInteractable(bool active)
    {
        isInteractable = active;
    }
}