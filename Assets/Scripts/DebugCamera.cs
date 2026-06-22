using UnityEngine;

public class DebugCamera : MonoBehaviour
{
    void Start()
    {
        Debug.Log($"[Camera] Камера активна: {gameObject.activeSelf}");
        Debug.Log($"[Camera] Компонент Camera: {GetComponent<Camera>() != null}");
        Debug.Log($"[Camera] Позиция: {transform.position}");
        
        Camera cam = GetComponent<Camera>();
        if (cam != null)
        {
            Debug.Log($"[Camera] Camera.enabled: {cam.enabled}");
        }
    }
}