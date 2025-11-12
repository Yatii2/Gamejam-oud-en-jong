using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonReset : MonoBehaviour
{
    
    public void Reset()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }
}