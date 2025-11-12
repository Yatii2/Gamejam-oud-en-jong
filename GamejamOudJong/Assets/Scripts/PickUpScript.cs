using UnityEngine;

public class PickUpScript : MonoBehaviour
{
    public int score = 0;

    public void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("OnCollisionEnter");
        if (other.gameObject.CompareTag("Pickup"))
        {
            score++;
            Destroy(other.gameObject);
        }
    }
}
