using UnityEngine;
using UnityEngine.UI;

public class VerticalReveal : MonoBehaviour
{
    public float duration = 1f;

    RectTransform rect;
    float timer = 0f;

    void Start()
    {
        rect = GetComponent<RectTransform>();

        // Start fully collapsed vertically
        rect.localScale = new Vector3(1, 0, 1);
    }

    void Update()
    {
        if (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);

            // Scale Y from 0 → 1
            rect.localScale = new Vector3(1, t, 1);
        }
    }
}
