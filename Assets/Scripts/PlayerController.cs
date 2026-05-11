using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Движение")]
    public float moveSpeed = 4f;
    private Vector3 targetPosition;
    private bool hasTarget = false;
    private bool isMoving = false;
    private bool isFacingRight = true;
    
    [Header("Raycast настройки (препятствия)")]
    public LayerMask obstacleLayer;       // Слой препятствий
    public float checkRadius = 0.4f;      // Радиус проверки столкновений
    public float stopDistance = 0.05f;    // Дистанция остановки от препятствия

    [Header("Спрайты: Стоя")]
    public Sprite idleLeft;
    public Sprite idleRight;

    [Header("Спрайты: Ходьба")]
    public List<Sprite> walkSpritesLeft;
    public List<Sprite> walkSpritesRight;

    private SpriteRenderer sr;
    private int walkFrame = 0;
    private float animTimer = 0f;
    private float animSpeed = 0.15f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        targetPosition = transform.position;
        
        // Если слой не назначен в инспекторе, ищем по имени
        if (obstacleLayer == 0)
        {
            obstacleLayer = LayerMask.GetMask("Obstacles");
        }
        
        UpdateSprite();
    }

    void Update()
    {
        // 1. Движение к точке клика
        if (hasTarget)
        {
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0;

            // Определяем направление взгляда
            if (Mathf.Abs(direction.x) > 0.2f)
            {
                bool newFacing = direction.x > 0;
                if (newFacing != isFacingRight)
                {
                    isFacingRight = newFacing;
                    walkFrame = 0;
                    animTimer = 0f;
                }
            }

            // Рассчитываем следующий шаг
            float step = moveSpeed * Time.deltaTime;
            Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, step);
            
            // ПРОВЕРКА: не упрётся ли игрок в препятствие
            if (!IsPathBlocked(newPosition))
            {
                transform.position = newPosition;
            }
            else
            {
                // Если путь заблокирован, пробуем обойти (опционально)
                if (CanMoveAround(newPosition))
                {
                    transform.position = newPosition;
                }
                else
                {
                    // Останавливаем движение к цели
                    hasTarget = false;
                    isMoving = false;
                }
            }

            // Проверяем, достигли ли цели
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                hasTarget = false;
                isMoving = false;
            }
            else
            {
                isMoving = true;
            }
        }
        else
        {
            isMoving = false;
        }

        // 2. Анимация
        UpdateSprite();
    }

    // Проверка, заблокирован ли путь
    bool IsPathBlocked(Vector3 newPosition)
    {
        // Проверяем коллизию в новой позиции
        Collider[] hitColliders = Physics.OverlapSphere(newPosition, checkRadius, obstacleLayer);
        
        foreach (Collider collider in hitColliders)
        {
            // Игнорируем самого игрока (если есть коллайдер)
            if (collider.gameObject != gameObject)
            {
                return true; // Путь заблокирован
            }
        }
        return false; // Путь свободен
    }

    // Простая попытка "обойти" препятствие (движение по касательной)
    bool CanMoveAround(Vector3 desiredPosition)
    {
        // Если расстояние до цели очень маленькое - не пытаемся обойти
        if (Vector3.Distance(transform.position, targetPosition) < stopDistance)
            return false;
            
        // Пробуем движение вверх/вниз по Z (для 3D)
        Vector3 sidestepPosition = new Vector3(
            desiredPosition.x,
            desiredPosition.y,
            desiredPosition.z + 0.5f
        );
        
        if (!IsPathBlocked(sidestepPosition))
        {
            targetPosition = sidestepPosition;
            return true;
        }
        
        sidestepPosition = new Vector3(
            desiredPosition.x,
            desiredPosition.y,
            desiredPosition.z - 0.5f
        );
        
        if (!IsPathBlocked(sidestepPosition))
        {
            targetPosition = sidestepPosition;
            return true;
        }
        
        return false;
    }

    void UpdateSprite()
    {
        Sprite currentIdle = isFacingRight ? idleRight : idleLeft;
        List<Sprite> currentWalk = isFacingRight ? walkSpritesRight : walkSpritesLeft;

        if (!isMoving)
        {
            if (sr.sprite != currentIdle)
                sr.sprite = currentIdle;
            animTimer = 0f;
            walkFrame = 0;
        }
        else
        {
            animTimer += Time.deltaTime;
            if (animTimer >= animSpeed)
            {
                animTimer = 0f;
                walkFrame = (walkFrame + 1) % currentWalk.Count;
                sr.sprite = currentWalk[walkFrame];
            }
        }
    }

    public void SetTarget(Vector3 point)
    {
        Vector3 newTarget = new Vector3(point.x, transform.position.y, point.z);
        
        // Проверяем, не кликнули ли за препятствием
        if (!IsTargetBehindWall(newTarget))
        {
            targetPosition = newTarget;
            hasTarget = true;
            isMoving = true;
        }
    }
    
    // Проверка, можно ли кликнуть в точку (луч от игрока до цели)
    bool IsTargetBehindWall(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        float distance = direction.magnitude;
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction.normalized, out hit, distance, obstacleLayer))
        {
            // Если луч упёрся в препятствие раньше, чем дошёл до цели
            if (hit.collider.gameObject != gameObject)
            {
                Debug.Log("Цель за препятствием: " + hit.collider.name);
                return true; // Цель за стеной
            }
        }
        return false; // Цель достижима
    }

    public Vector3 GetPosition() => transform.position;
    public bool IsMoving() => isMoving;
    
    // Визуализация радиуса проверки в редакторе (для отладки)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
        
        if (hasTarget)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(targetPosition, 0.2f);
            Gizmos.DrawLine(transform.position, targetPosition);
        }
    }
}