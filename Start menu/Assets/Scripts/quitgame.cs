using UnityEngine;
using UnityEngine.UI;  // For UI components like Button

public class quitgame : MonoBehaviour
{
    public Button quitButton;
    void Update()
    {
        quitButton.onClick.AddListener(Quitgame);

    }
    public void Quitgame()
    {
        Application.Quit();
    }
}
