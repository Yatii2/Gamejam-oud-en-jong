using UnityEngine;
using UnityEngine.InputSystem;

public class MouseCameraFollow : MonoBehaviour
{
    public float followAmount = 50f;   // how far the UI moves
    public float smoothSpeed = 5f;

    private RectTransform rect;
    private Vector2 startPos;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        startPos = rect.anchoredPosition;
    }

    void Update()
    {
        if (Mouse.current == null)
            return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        float x = (mousePos.x / Screen.width - 0.5f) * 2f;
        float y = (mousePos.y / Screen.height - 0.5f) * 2f;

        Vector2 targetPos = startPos + new Vector2(x * followAmount, y * followAmount);

        rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, targetPos, Time.deltaTime * smoothSpeed);
    }
}
