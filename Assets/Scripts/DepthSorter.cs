using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DepthSorter : MonoBehaviour
{
    private SpriteRenderer sr;
    private Camera cam;

    [Tooltip("Если сортировка работает наоборот, поставь галочку")]
    public bool invertOrder = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        cam = Camera.main ?? FindObjectOfType<Camera>();
    }

    void LateUpdate()
    {
        if (cam == null) return;

        // Вектор от камеры к объекту
        Vector3 toObject = transform.position - cam.transform.position;
        // Проекция на направление взгляда камеры = точная глубина
        float depth = Vector3.Dot(toObject, cam.transform.forward);

        // Чем больше depth (ближе к камере), тем выше sortingOrder
        int order = Mathf.RoundToInt(depth * 100f);
        sr.sortingOrder = invertOrder ? -order : order;
    }
}