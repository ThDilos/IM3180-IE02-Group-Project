using UnityEngine;
using UnityEngine.SceneManagement;

public class startscene : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Intro Cutscene");
    }
    void Start()
    {
        Cursor.visible = true;
    }
}
