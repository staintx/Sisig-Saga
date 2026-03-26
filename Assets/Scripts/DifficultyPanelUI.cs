using UnityEngine;
using UnityEngine.UI;

public class DifficultyPanelUI : MonoBehaviour
{
    [Header("UI")]
    public Image levelTitleImage;

    [Header("Level Title Sprites (index 0 = Level 1)")]
    public Sprite[] levelTitleSprites;

    public void SetLevel(int level)
    {
        if (levelTitleImage == null || levelTitleSprites == null || levelTitleSprites.Length == 0)
            return;

        int index = Mathf.Clamp(level - 1, 0, levelTitleSprites.Length - 1);
        levelTitleImage.sprite = levelTitleSprites[index];
    }
}