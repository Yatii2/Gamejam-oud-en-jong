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
    public bool useLastMoveIfAvailable = true;
    public bool maintainVelocityInFixedUpdate = true;
    public MonoBehaviour[] disableDuringDash;

    Rigidbody2D rb;
    bool isDashing = false;
    bool isOnCooldown = false;
    Vector2 lastMoveDirection = Vector2.right;

    public bool IsDashing => isDashing;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (cooldownFillImage != null) cooldownFillImage.fillAmount = 0f;
        if (cooldownText != null) cooldownText.text = "";
        if (rb.bodyType == RigidbodyType2D.Kinematic) Debug.LogWarning("Rigidbody2D is Kinematic. Set bodyType to Dynamic for velocity-based dash to work.");
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 input = new Vector2(h, v);
        if (input.sqrMagnitude > 0.01f) lastMoveDirection = input.normalized;
    }

    public void OnDashButtonPressed()
    {
        if (!isDashing && !isOnCooldown)
        {
            Vector2 dashDir = DetermineDashDirection();
            StartCoroutine(DashRoutine(dashDir));
            StartCoroutine(CooldownRoutine());
        }
    }

    public void SetLastMoveDirection(Vector2 dir)
    {
        if (dir.sqrMagnitude > 0.01f) lastMoveDirection = dir.normalized;
    }

    Vector2 DetermineDashDirection()
    {
        if (useLastMoveIfAvailable && lastMoveDirection.sqrMagnitude > 0.01f) return lastMoveDirection.normalized;
        float face = transform.localScale.x >= 0 ? 1f : -1f;
        return new Vector2(face, 0f);
    }

    IEnumerator DashRoutine(Vector2 direction)
    {
        isDashing = true;
        if (disableDuringDash != null)
        {
            for (int i = 0; i < disableDuringDash.Length; i++) if (disableDuringDash[i] != null) disableDuringDash[i].enabled = false;
        }

        if (maintainVelocityInFixedUpdate)
        {
            float t = 0f;
            while (t < dashDuration)
            {
                rb.linearVelocity = direction.normalized * dashSpeed;
                t += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            rb.linearVelocity = direction.normalized * dashSpeed;
            yield return new WaitForSeconds(dashDuration);
        }

        rb.linearVelocity = Vector2.zero;

        if (disableDuringDash != null)
        {
            for (int i = 0; i < disableDuringDash.Length; i++) if (disableDuringDash[i] != null) disableDuringDash[i].enabled = true;
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