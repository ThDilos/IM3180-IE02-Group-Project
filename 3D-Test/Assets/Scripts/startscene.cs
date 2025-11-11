using UnityEngine;
using UnityEngine.SceneManagement;

public class startscene : MonoBehaviour
{
    public void StartGame()
    {
        if (GameController.Instance != null)
            Destroy(GameController.Instance.gameObject); // Refresh GameController
        SceneManager.LoadScene("Intro Cutscene");
    }
    void Start()
    {
        Cursor.visible = true;
    }
}
