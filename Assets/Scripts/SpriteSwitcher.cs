using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSwitcher : MonoBehaviour
{
    [Header("Спрайты")]
    public Sprite spriteA;
    public Sprite spriteB;

    [Header("Таймер переключения")]
    [Tooltip("Базовое время между сменами (в секундах)")]
    public float baseInterval = 3f;
    [Tooltip("Случайная добавка к интервалу (0 = строго по таймеру, >0 = естественная задержка)")]
    public float randomOffset = 2f;

    private SpriteRenderer sr;
    private bool isShowingA = true;
    private float nextSwitchTime;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        
        // Если спрайты не назначены, выводим предупреждение в консоль
        if (spriteA == null || spriteB == null)
        {
            Debug.LogWarning("[" + gameObject.name + "] Не назначены оба спрайта в SpriteSwitcher!");
            return;
        }

        sr.sprite = spriteA;
        ScheduleNextSwitch();
    }

    void Update()
    {
        if (Time.time >= nextSwitchTime)
        {
            // Меняем спрайт
            isShowingA = !isShowingA;
            sr.sprite = isShowingA ? spriteA : spriteB;
            
            // Планируем следующее переключение
            ScheduleNextSwitch();
        }
    }

    void ScheduleNextSwitch()
    {
        // Интервал = база + случайное значение от 0 до randomOffset
        float interval = baseInterval + Random.Range(0f, randomOffset);
        nextSwitchTime = Time.time + interval;
    }
}