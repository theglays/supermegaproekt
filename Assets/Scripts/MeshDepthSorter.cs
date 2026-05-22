using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MeshDepthSorter : MonoBehaviour
{
    [Tooltip("Базовый RenderQueue. Для прозрачных материалов обычно 3000")]
    public int baseRenderQueue = 3000;
    
    [Tooltip("Направление: -1 если камера смотрит в -Z, 1 если в +Z")]
    public int depthDirection = -1;
    
    [Tooltip("Множитель шага сортировки. 100 обычно хватает")]
    public float sortMultiplier = 100f;

    private MeshRenderer mr;
    private Material cachedMat;

    void Awake()
    {
        mr = GetComponent<MeshRenderer>();
        // Клонируем материал, чтобы изменение RenderQueue не влияло на другие объекты
        cachedMat = mr.material;
    }

    void LateUpdate()
    {
        // Вычисляем новый RenderQueue на основе позиции Z
        int newQueue = Mathf.RoundToInt(baseRenderQueue + transform.position.z * sortMultiplier * depthDirection);
        
        // Защита от выхода за границы валидного диапазона
        newQueue = Mathf.Clamp(newQueue, 2500, 4000);
        
        if (cachedMat.renderQueue != newQueue)
            cachedMat.renderQueue = newQueue;
    }
}