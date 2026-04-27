using UnityEngine;

public class ClickToMove : MonoBehaviour
{
    public PlayerController player;
    public LayerMask groundLayer; // Опционально, для точности

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ЛКМ
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                // Проверяем, кликнули ли мы по земле
                if (hit.transform.CompareTag("Ground"))
                {
                    player.SetTarget(hit.point);
                }
            }
        }
    }
}