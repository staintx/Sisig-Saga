using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject howToPlayPanel;
    public GameObject levelSelectPanel;
    public GameObject difficultyPanel;
    public GameObject settingsPanel;
    public GameObject shopPanel; // optional

    [Header("Difficulty UI")]
    public DifficultyPanelUI difficultyUI;

    int selectedLevel = 1;

    void Start()
    {
        if (PlayerPrefs.GetInt("OpenLevelSelect", 0) == 1)
        {
            PlayerPrefs.SetInt("OpenLevelSelect", 0);
            PlayerPrefs.Save();
            ShowLevelSelect();
            return;
        }

        ShowMain();
    }

    public void ShowMain()
    {
        SetActive(mainMenuPanel, true);
        SetActive(howToPlayPanel, false);
        SetActive(levelSelectPanel, false);
        SetActive(difficultyPanel, false);
        SetActive(settingsPanel, false);
        SetActive(shopPanel, false);
    }

    public void ShowHowToPlay()
    {
        SetActive(mainMenuPanel, false);
        SetActive(howToPlayPanel, true);
    }

    public void ShowLevelSelect()
    {
        SetActive(mainMenuPanel, false);
        SetActive(levelSelectPanel, true);
    }

    public void ShowShop()
    {
        SetActive(mainMenuPanel, false);
        SetActive(shopPanel, true);
    }

    public void ShowSettings()
    {
        SetActive(mainMenuPanel, false);
        SetActive(settingsPanel, true);
    }

    public void StartGame()
    {
        ShowLevelSelect();
    }

    // Called by Level buttons
    public void ShowDifficulty(int level)
    {
        selectedLevel = level;
        SetActive(mainMenuPanel, false);
        SetActive(levelSelectPanel, false);
        SetActive(difficultyPanel, true);

        if (difficultyUI != null)
            difficultyUI.SetLevel(level);
    }

    public void BackToLevelSelect()
    {
        SetActive(difficultyPanel, false);
        SetActive(levelSelectPanel, true);
    }

    // Difficulty buttons
    public void StartEasy()
    {
        StartGameplayWithDifficulty(0);
    }

    public void StartMedium()
    {
        StartGameplayWithDifficulty(1);
    }

    public void StartHard()
    {
        StartGameplayWithDifficulty(2);
    }

    void StartGameplayWithDifficulty(int difficulty)
    {
        PlayerPrefs.SetInt("SelectedLevel", selectedLevel);
        PlayerPrefs.SetInt("Difficulty", difficulty);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Gameplay");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void SetActive(GameObject obj, bool value)
    {
        if (obj != null)
            obj.SetActive(value);
    }
}