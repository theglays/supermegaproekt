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
    
    [Tooltip("На сколько сместить сферу проверки вниз относительно центра игрока, чтобы проверять пороги у ног")]
    public float feetOffsetY = 1.0f;     // Изменяй это значение в инспекторе, чтобы опустить красный круг на ноги

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
        
        if (obstacleLayer == 0)
        {
            obstacleLayer = LayerMask.GetMask("Obstacles");
        }
        
        UpdateSprite();
    }

    void Update()
    {
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
            
            // Проверяем преграды
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

        UpdateSprite();
    }

    // ИСПРАВЛЕННЫЙ МЕТОД: теперь проверяет препятствия строго у ног
    bool IsPathBlocked(Vector3 newPosition)
    {
        // Смещаем центр проверочной сферы вниз, к ступням персонажа
        Vector3 feetPosition = newPosition;
        feetPosition.y -= feetOffsetY;

        Collider[] hitColliders = Physics.OverlapSphere(feetPosition, checkRadius, obstacleLayer);
        
        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject != gameObject)
            {
                return true; // Путь заблокирован (задели порог, стул или шкаф ногами)
            }
        }
        return false;
    }

    bool CanMoveAround(Vector3 desiredPosition)
    {
        if (Vector3.Distance(transform.position, targetPosition) < stopDistance)
            return false;
            
        Vector3 sidestepPosition = new Vector3(desiredPosition.x, desiredPosition.y, desiredPosition.z + 0.5f);
        if (!IsPathBlocked(sidestepPosition))
        {
            targetPosition = sidestepPosition;
            return true;
        }
        
        sidestepPosition = new Vector3(desiredPosition.x, desiredPosition.y, desiredPosition.z - 0.5f);
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
        
        if (!IsTargetBehindWall(newTarget))
        {
            targetPosition = newTarget;
            hasTarget = true;
            isMoving = true;
        }
    }
    
    bool IsTargetBehindWall(Vector3 target)
    {
        // Рейкаст тоже пускаем от ног, чтобы он не пролетал поверх порогов
        Vector3 startPos = transform.position;
        startPos.y -= feetOffsetY;

        Vector3 targetPos = target;
        targetPos.y -= feetOffsetY;

        Vector3 direction = targetPos - startPos;
        float distance = direction.magnitude;
        
        RaycastHit hit;
        if (Physics.Raycast(startPos, direction.normalized, out hit, distance, obstacleLayer))
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
    
    // ИСПРАВЛЕННЫЙ МЕТОД: теперь рисует отладочный круг на правильной высоте
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // Опускаем гизмо на ту же высоту, что и в коде проверки
        Vector3 debugFeetPos = transform.position;
        debugFeetPos.y -= feetOffsetY;
        
        Gizmos.DrawWireSphere(debugFeetPos, checkRadius);
        
        if (hasTarget)
        {
            Gizmos.color = Color.green;
            Vector3 debugTargetPos = targetPosition;
            debugTargetPos.y -= feetOffsetY;
            
            Gizmos.DrawWireSphere(debugTargetPos, 0.2f);
            Gizmos.DrawLine(debugFeetPos, debugTargetPos);
        }
    }
}