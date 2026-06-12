using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Движение")]
    public float moveSpeed = 4f;
    private Vector3 targetPosition;
    private bool hasTarget = false;
    private bool isMoving = false;
    private bool wasMoving = false;  // 👈 ДОБАВЛЕНО: для отслеживания начала движения
    private bool isFacingRight = true;
    
    [Header("Raycast настройки (препятствия)")]
    public LayerMask obstacleLayer;
    public float checkRadius = 0.4f;
    public float stopDistance = 0.05f;
    public Vector3 checkOffset = new Vector3(0, -0.5f, 0);

    [Header("Спрайты: Стоя")]
    public Sprite idleLeft;
    public Sprite idleRight;

    [Header("Спрайты: Ходьба")]
    public List<Sprite> walkSpritesLeft;
    public List<Sprite> walkSpritesRight;

    private SpriteRenderer sr;
    private int walkFrame = 0;
    private float animTimer = 0f;
    private int stepCounter = 0;  // 👈 ДОБАВЬ ЭТУ СТРОКУ
    private float animSpeed = 0.15f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        targetPosition = transform.position;
        
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

            float step = moveSpeed * Time.deltaTime;
            Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, step);
            
            // ПРОВЕРКА: не упрётся ли игрок в препятствие
            if (!IsPathBlocked(newPosition))
            {
                transform.position = newPosition;
            }
            else
            {
                if (CanMoveAround(newPosition))
                {
                    transform.position = newPosition;
                }
                else
                {
                    hasTarget = false;
                    isMoving = false;
                }
            }

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

// // 🔥 РИТМИЧНЫЕ ШАГИ: звук каждые 4 кадра анимации 🔥
// if (AudioManager.Instance != null && isMoving)
// {
//     animTimer += Time.deltaTime;
//     if (animTimer >= animSpeed)
//     {
//         animTimer = 0f;
//         walkFrame = (walkFrame + 1) % currentWalk.Count;
//         sr.sprite = currentWalk[walkFrame];
        
//         // Счётчик шагов: проигрываем звук только каждый 4-й кадр
//         stepCounter++;
//         if (stepCounter % 4 == 0)
//         {
//             AudioManager.Instance.PlaySFX("Footstep", transform.position);
//         }
//     }
// }
        // 2. Анимация
        UpdateSprite();
    }

    // Проверка, заблокирован ли путь
    bool IsPathBlocked(Vector3 newPosition)
    {
        Vector3 checkPoint = newPosition + checkOffset;
        Collider[] hitColliders = Physics.OverlapSphere(checkPoint, checkRadius, obstacleLayer);
        
        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject != gameObject)
            {
                return true;
            }
        }
        return false;
    }

    bool CanMoveAround(Vector3 desiredPosition)
    {
        if (Vector3.Distance(transform.position, targetPosition) < stopDistance)
            return false;
            
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
        stepCounter = 0; // Сбрасываем счётчик при остановке
    }
    else
    {
        animTimer += Time.deltaTime;
        if (animTimer >= animSpeed)
        {
            animTimer = 0f;
            walkFrame = (walkFrame + 1) % currentWalk.Count;
            sr.sprite = currentWalk[walkFrame];
            
            // 🔥 Звук каждые 4 кадра 🔥
            if (AudioManager.Instance != null)
            {
                stepCounter++;
                if (stepCounter % 4 == 0)
                {
                    AudioManager.Instance.PlaySFX("Footstep", transform.position);
                }
            }
        }
    }
}

    public void SetTarget(Vector3 point)
    {
        Vector3 newTarget = new Vector3(point.x, transform.position.y, point.z);
        
        if (!IsTargetBehindWall(newTarget))
        {
            targetPosition = newTarget;
            hasTarget = true;
            isMoving = true;
        }
    }
    
    bool IsTargetBehindWall(Vector3 target)
    {
        Vector3 startPoint = transform.position + checkOffset;
        Vector3 targetPoint = target + checkOffset;

        Vector3 direction = targetPoint - startPoint;
        float distance = direction.magnitude;
        
        RaycastHit hit;
        if (Physics.Raycast(startPoint, direction.normalized, out hit, distance, obstacleLayer))
        {
            if (hit.collider.gameObject != gameObject)
            {
                Debug.Log("Цель за препятствием: " + hit.collider.name);
                return true; 
            }
        }
        return false; 
    }

    public Vector3 GetPosition() => transform.position;
    public bool IsMoving() => isMoving;
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + checkOffset, checkRadius);
        
        if (hasTarget)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(targetPosition + checkOffset, 0.2f);
            Gizmos.DrawLine(transform.position + checkOffset, targetPosition + checkOffset);
        }
    }
}