using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToNextScene : MonoBehaviour
{
    [Header("If = -1, go to the next scene in sequence")]
    [SerializeField] public int nextSceneNumber = -1;
    private void OnTriggerEnter(Collider other) 
    { 
        if (other.CompareTag("Player")) 
        { 
            NextScene();
        }
    }

    public void NextScene()
    {
        if (nextSceneNumber > -1)
        {
            SceneManager.LoadScene(nextSceneNumber);
            return;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}