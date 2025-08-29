using UnityEngine;
using UnityEngine.SceneManagement;

public class startscene : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Main");
    }
    void Start()
    {
        Cursor.visible = true;
    }
}
