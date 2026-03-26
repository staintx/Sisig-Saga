using UnityEngine;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelUI
    {
        public int levelIndex;      // 1,2,3...
        public Button button;       // Level button
        public GameObject lockIcon; // Lock image
        public Image[] stars;       // 3 star images
    }

    public LevelUI[] levels;

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            int index = levels[i].levelIndex;

            bool unlocked = IsLevelUnlocked(index);
            levels[i].button.interactable = unlocked;
            levels[i].lockIcon.SetActive(!unlocked);

            int starCount = GetStars(index);
            for (int s = 0; s < levels[i].stars.Length; s++)
            {
                levels[i].stars[s].enabled = (s < starCount);
            }
        }
    }

    bool IsLevelUnlocked(int level)
    {
        if (level == 1) return true;
        return PlayerPrefs.GetInt("LevelUnlocked_" + level, 0) == 1;
    }

    int GetStars(int level)
    {
        return PlayerPrefs.GetInt("LevelStars_" + level, 0);
    }

    // Call this after finishing a level
    public static void SaveLevelResult(int level, int starsEarned)
    {
        PlayerPrefs.SetInt("LevelUnlocked_" + (level + 1), 1);

        int current = PlayerPrefs.GetInt("LevelStars_" + level, 0);
        if (starsEarned > current)
            PlayerPrefs.SetInt("LevelStars_" + level, starsEarned);

        PlayerPrefs.Save();
    }
}