using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class Wallhacks : MonoBehaviour
{
    public float phaseDuration = 1f;
    public float phaseCooldown = 2f;
    public Button wallhackButton;
    public Image cooldownFillImage;
    public TextMeshProUGUI cooldownText;

    Collider2D[] colliders;
    bool[] originalIsTrigger;
    bool isPhasing = false;
    bool isOnCooldown = false;

    void Awake()
    {
        colliders = GetComponentsInChildren<Collider2D>();
        originalIsTrigger = new bool[colliders.Length];
        if (cooldownFillImage != null) cooldownFillImage.fillAmount = 0f;
        if (cooldownText != null) cooldownText.text = "";
    }

    public void OnPhaseButtonPressed()
    {
        if (!isPhasing && !isOnCooldown)
            StartCoroutine(PhaseRoutine());
    }

    IEnumerator PhaseRoutine()
    {
        isPhasing = true;
        for (int i = 0; i < colliders.Length; i++) originalIsTrigger[i] = colliders[i].isTrigger;
        for (int i = 0; i < colliders.Length; i++) colliders[i].isTrigger = true;
        if (wallhackButton != null) wallhackButton.interactable = false;

        float t = 0f;
        while (t < phaseDuration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < colliders.Length; i++) colliders[i].isTrigger = originalIsTrigger[i];
        isPhasing = false;

        isOnCooldown = true;
        t = 0f;
        while (t < phaseCooldown)
        {
            t += Time.deltaTime;
            float remaining = Mathf.Max(0f, phaseCooldown - t);
            if (cooldownFillImage != null) cooldownFillImage.fillAmount = remaining / phaseCooldown;
            if (cooldownText != null) cooldownText.text = remaining > 0.05f ? remaining.ToString("F1") + "s" : "";
            yield return null;
        }

        if (cooldownFillImage != null) cooldownFillImage.fillAmount = 0f;
        if (cooldownText != null) cooldownText.text = "";
        if (wallhackButton != null) wallhackButton.interactable = true;
        isOnCooldown = false;
    }

    public bool IsPhasing => isPhasing;
    public bool IsOnCooldown => isOnCooldown;
}