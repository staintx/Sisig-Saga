using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    const string MusicKey = "MusicOn";
    const string SfxKey = "SfxOn";

    public GameObject musicCheck;
    public GameObject sfxCheck;

    void Start()
    {
        bool musicOn = PlayerPrefs.GetInt(MusicKey, 1) == 1;
        bool sfxOn = PlayerPrefs.GetInt(SfxKey, 1) == 1;

        musicCheck.SetActive(musicOn);
        sfxCheck.SetActive(sfxOn);
    }

    public void ToggleMusic()
    {
        bool musicOn = !musicCheck.activeSelf;
        musicCheck.SetActive(musicOn);
        PlayerPrefs.SetInt(MusicKey, musicOn ? 1 : 0);
        PlayerPrefs.Save();
        AudioManager.Instance?.ApplySettings();
    }

    public void ToggleSfx()
    {
        bool sfxOn = !sfxCheck.activeSelf;
        sfxCheck.SetActive(sfxOn);
        PlayerPrefs.SetInt(SfxKey, sfxOn ? 1 : 0);
        PlayerPrefs.Save();
        AudioManager.Instance?.ApplySettings();
    }
}