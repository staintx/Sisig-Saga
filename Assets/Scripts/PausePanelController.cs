using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePanelController : MonoBehaviour
{
    public void Resume()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menus"); // change to your main menu scene name
    }
}