using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDash : MonoBehaviour
{
    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1.0f;
    public Button dashButton;
    public Image cooldownFillImage;
    public TextMeshProUGUI cooldownText;
    public bool useRbVelocityForDirection = true;
    public float minVelocityForDirection = 0.1f;

    Rigidbody2D rb;
    bool isDashing = false;
    bool isOnCooldown = false;

    public bool IsDashing => isDashing;
    public bool IsOnCooldown => isOnCooldown;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (cooldownFillImage != null) cooldownFillImage.fillAmount = 0f;
        if (cooldownText != null) cooldownText.text = "";
        if (rb != null && rb.bodyType == RigidbodyType2D.Kinematic)
            Debug.LogWarning("Rigidbody2D is Kinematic. Set bodyType to Dynamic for dash to work correctly.");
    }

    public void OnDashButtonPressed()
    {
        if (isDashing || isOnCooldown) return;
        Vector2 dir = DetermineDashDirection();
        if (dir.sqrMagnitude <= 0.0001f) dir = transform.localScale.x >= 0 ? Vector2.right : Vector2.left;
        StartCoroutine(DashRoutine(dir));
        StartCoroutine(CooldownRoutine());
    }

    Vector2 DetermineDashDirection()
    {
        if (useRbVelocityForDirection && rb != null)
        {
            Vector2 v = rb.linearVelocity;
            if (v.magnitude >= minVelocityForDirection) return v.normalized;
        }
        float face = transform.localScale.x >= 0 ? 1f : -1f;
        return new Vector2(face, 0f);
    }

    IEnumerator DashRoutine(Vector2 direction)
    {
        isDashing = true;
        float t = 0f;
        while (t < dashDuration)
        {
            if (rb != null)
            {
                Vector2 next = rb.position + direction.normalized * dashSpeed * Time.fixedDeltaTime;
                rb.MovePosition(next);
            }
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        isDashing = false;
    }

    IEnumerator CooldownRoutine()
    {
        isOnCooldown = true;
        if (dashButton != null) dashButton.interactable = false;
        float t = 0f;
        while (t < dashCooldown)
        {
            t += Time.deltaTime;
            float remaining = Mathf.Max(0f, dashCooldown - t);
            if (cooldownFillImage != null) cooldownFillImage.fillAmount = remaining / dashCooldown;
            if (cooldownText != null) cooldownText.text = remaining > 0.05f ? remaining.ToString("F1") + "s" : "";
            yield return null;
        }
        if (cooldownFillImage != null) cooldownFillImage.fillAmount = 0f;
        if (cooldownText != null) cooldownText.text = "";
        if (dashButton != null) dashButton.interactable = true;
        isOnCooldown = false;
    }
}