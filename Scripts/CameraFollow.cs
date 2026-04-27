using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    private Vector3 offset;

    void Start()
    {
        // Запоминаем изначальное смещение камеры относительно игрока
        offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPos = target.position + offset;
        // Плавное следование
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
        // Вращение НЕ меняем! Оно зафиксировано в редакторе.
    }
}