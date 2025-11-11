using UnityEngine;
using UnityEngine.SceneManagement;

public class startscene : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Intro Cutscene");
        Destroy(GameController.Instance); // Refresh GameController
    }
    void Start()
    {
        Cursor.visible = true;
    }
}
