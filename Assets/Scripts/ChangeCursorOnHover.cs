using UnityEngine;

public class ChangeCursorOnHover : MonoBehaviour
{
    public Texture2D defaultCursor;
    public Texture2D hoverCursor;
    public Vector2 hotspot = Vector2.zero; // или new Vector2(16,16) для 32px курсора

    private void Start()
    {
        // Устанавливаем курсор по умолчанию
        Cursor.SetCursor(defaultCursor, hotspot, CursorMode.Auto);
    }

    private void OnMouseEnter()
    {
        Cursor.SetCursor(hoverCursor, hotspot, CursorMode.Auto);
    }

    private void OnMouseExit()
    {
        Cursor.SetCursor(defaultCursor, hotspot, CursorMode.Auto);
    }
}