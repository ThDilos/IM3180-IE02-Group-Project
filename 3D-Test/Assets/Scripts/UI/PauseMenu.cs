using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
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
