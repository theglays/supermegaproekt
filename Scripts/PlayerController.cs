using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Движение")]
    public float moveSpeed = 4f;
    private Vector3 targetPosition;
    private bool hasTarget = false;
    private bool isMoving = false;
    private bool isFacingRight = true; // Начальное направление взгляда

    [Header("Спрайты: Стоя")]
    public Sprite idleLeft;
    public Sprite idleRight;

    [Header("Спрайты: Ходьба")]
    public List<Sprite> walkSpritesLeft;
    public List<Sprite> walkSpritesRight;

    private SpriteRenderer sr;
    private int walkFrame = 0;
    private float animTimer = 0f;
    private float animSpeed = 0.15f; // Скорость смены кадров

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        targetPosition = transform.position;
        UpdateSprite();
    }

    void Update()
    {
        // 1. Движение к точке клика
        if (hasTarget)
        {
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0; // Игнорируем высоту

            // Определяем направление по оси X с порогом, чтобы не дёргался
            if (Mathf.Abs(direction.x) > 0.2f)
            {
                bool newFacing = direction.x > 0;
                if (newFacing != isFacingRight)
                {
                    isFacingRight = newFacing;
                    walkFrame = 0; // Сброс анимации при смене направления
                    animTimer = 0f;
                }
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

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

        // 2. Обновление анимации
        UpdateSprite();
    }

    void UpdateSprite()
    {
        // Выбираем нужный набор спрайтов
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
        targetPosition = new Vector3(point.x, transform.position.y, point.z);
        hasTarget = true;
        isMoving = true;
    }

    public Vector3 GetPosition() => transform.position;
    public bool IsMoving() => isMoving;
}