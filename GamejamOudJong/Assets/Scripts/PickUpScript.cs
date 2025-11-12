using System;
using UnityEngine;

public class PickUpScript : MonoBehaviour
{
    public int score = 0;
    [SerializeField] public int winningScore;

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
            //trigger hier de old man wins screen
        }
    }

    private void Update()
    {
        if (score >= winningScore)
        {
            //trigger hier de child wins script
        } 
    }
}
