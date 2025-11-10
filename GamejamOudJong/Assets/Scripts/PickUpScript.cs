using UnityEngine;

public class PickUpScript : MonoBehaviour
{
    public int score = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
