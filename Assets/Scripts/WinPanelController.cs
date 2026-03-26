using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class WinPanelController : MonoBehaviour
{
    public TMP_Text coinsText;
    public TMP_Text tipsText;

    public Image star1;
    public Image star2;
    public Image star3;

    public Color starOn = Color.white;
    public Color starOff = new Color(1f, 1f, 1f, 0.2f);

    [Header("Scene Navigation")]
    public string menuSceneName = "Menus";

    public void Show(int coins, int tips, int stars)
    {
        AudioManager.Instance?.PlayVictoryBgm();
        if (coinsText != null) coinsText.text = $"Coins: {coins}";
        if (tipsText != null) tipsText.text = $"Tips: {tips}";

        SetStars(stars);

        gameObject.SetActive(true);
    }

    void SetStars(int count)
    {
        if (star1 != null) star1.color = count >= 1 ? starOn : starOff;
        if (star2 != null) star2.color = count >= 2 ? starOn : starOff;
        if (star3 != null) star3.color = count >= 3 ? starOn : starOff;
    }

    public void Replay()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        PlayerPrefs.SetInt("OpenLevelSelect", 1);
        PlayerPrefs.Save();

        if (!string.IsNullOrEmpty(menuSceneName))
            SceneManager.LoadScene(menuSceneName);
        else
            Debug.LogWarning("NextLevel: Menu scene name not set.");
    }
}