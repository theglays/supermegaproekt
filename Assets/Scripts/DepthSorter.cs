using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class DepthSorter : MonoBehaviour
{
    [Tooltip("Множитель сортировки. 100 обычно хватает. Если объекты мерцают, увеличь до 200-500.")]
    public float sortMultiplier = 100f;

    [Tooltip("Базовое смещение. Помогает, если нужно поднять/опустить весь объект в очереди отрисовки.")]
    public int baseOffset = 0;

    private Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        // Принудительно ставим все объекты на один слой сортировки.
        // Если у тебя свой слой, замени "Default" на его название.
        rend.sortingLayerName = "Default";
    }

    void LateUpdate()
    {
        // Чем дальше объект по Z (дальше от камеры), тем МЕНЬШЕ его порядок.
        // Минус гарантирует, что ближние объекты всегда рисуются поверх дальних.
        int order = Mathf.RoundToInt(-transform.position.z * sortMultiplier) + baseOffset;
        
        // Защита от переполнения и слипания
        order = Mathf.Clamp(order, -10000, 10000);
        
        rend.sortingOrder = order;
    }
}