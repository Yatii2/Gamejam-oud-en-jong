using System;
using UnityEngine;
using UnityEngine.UI;

public class PickUpScript : MonoBehaviour
{
    // Runtime Fields
    public int score = 0;

    // Serialize Fields
    [SerializeField] public int winningScore;
    [SerializeField] private RectTransform oldRect;
    [SerializeField] private RectTransform childRect;

    public void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("OnCollisionEnter");
        if (other.gameObject.CompareTag("Pickup") && gameObject.CompareTag("Child"))
        {
            score++;
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("Child"))
        {
            Debug.Log("Child dies, old man wins");
            Destroy(other.gameObject);
            oldRect.gameObject.SetActive(true);
        }
        else
        {
            if (score >= winningScore)
            {
                Debug.Log("Child has collected all keys, Child wins!");
                childRect.gameObject.SetActive(true);
            }
        }
    }
}