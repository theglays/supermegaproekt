using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    // Ссылка на камеру, на которую нужно смотреть
    private Camera mainCamera;

    void Start()
    {
        // Находим главную камеру на сцене
        mainCamera = Camera.main;
        
        // Если камеры нет, пробуем найти любую активную камеру
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            // 1. Получаем направление от персонажа к камере
            Vector3 directionToCamera = mainCamera.transform.position - transform.position;
            
            // 2. Игнорируем разницу по высоте (Y), чтобы персонаж не заваливался назад/вперёд
            // Он будет крутиться только вокруг своей оси Y
            directionToCamera.y = 0;

            // 3. Если направление не нулевое, поворачиваемся
            if (directionToCamera != Vector3.zero)
            {
                // Создаём вращение, которое смотрит в сторону направления
                Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
                
                // Плавно или мгновенно применяем вращение
                transform.rotation = targetRotation;
            }
        }
    }
}