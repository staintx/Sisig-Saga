using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LosePanelController : MonoBehaviour
{
    public TMP_Text customersLostText;
    public TMP_Text timeRemainingText;
    public TMP_Text coinsEarnedText;

    public void Show(int customersLost, float timeRemaining, int coins)
    {
        AudioManager.Instance?.PlayLoseBgm();
        if (customersLostText != null)
            customersLostText.text = $"Customers Lost: {customersLost}";

        if (timeRemainingText != null)
            timeRemainingText.text = $"Time Remaining: {FormatTime(timeRemaining)}";

        if (coinsEarnedText != null)
            coinsEarnedText.text = $"Coins Earned: {coins}";

        gameObject.SetActive(true);
    }

    string FormatTime(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60f);
        int s = Mathf.FloorToInt(seconds % 60f);
        return $"{m:00}:{s:00}";
    }

    public void Replay()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Exit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menus"); // change if needed
    }
}