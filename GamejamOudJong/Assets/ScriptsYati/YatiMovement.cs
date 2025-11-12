using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class YatiMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private PlayerDash dash; 

    Rigidbody2D rb;
    Vector2 input;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        if (dash == null) dash = GetComponent<PlayerDash>();
    }

    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        if (input.sqrMagnitude > 1f) input = input.normalized;
    }

    void FixedUpdate()
    {
        if (dash != null && dash.IsDashing) return;
        rb.linearVelocity = input * moveSpeed;
    }
}