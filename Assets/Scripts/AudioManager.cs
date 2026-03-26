using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    const string MusicKey = "MusicOn";
    const string SfxKey = "SfxOn";

    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioSource grillLoopSource;
    public AudioSource panLoopSource;

    [Header("BGM")]
    public AudioClip bgmMainMenu;
    public AudioClip bgmGameplay;
    public AudioClip bgmVictory;
    public AudioClip bgmLose;

    [Header("SFX")]
    public AudioClip sfxCountdown;
    public AudioClip sfxCountdownGo;
    public AudioClip sfxCookingDone;
    public AudioClip sfxCustomerServed;
    public AudioClip sfxGrilling;
    public AudioClip sfxGulamanDone;
    public AudioClip sfxGulamanPour;
    public AudioClip sfxPlaceOnPlate;
    public AudioClip sfxTapIngredient;
    public AudioClip sfxTapStation;
    public AudioClip sfxUiClick;

    [Header("SFX Loops")]
    public AudioClip sfxSizzleLoopGrill;
    public AudioClip sfxSizzleLoopPan;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        EnsureSources();
        ApplySettings();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void EnsureSources()
    {
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
        }

        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();

        if (grillLoopSource == null)
        {
            grillLoopSource = gameObject.AddComponent<AudioSource>();
            grillLoopSource.loop = true;
        }

        if (panLoopSource == null)
        {
            panLoopSource = gameObject.AddComponent<AudioSource>();
            panLoopSource.loop = true;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Menus")
            PlayBgm(bgmMainMenu);
        else if (scene.name == "Gameplay")
            PlayBgm(bgmGameplay);
    }

    public void ApplySettings()
    {
        bool musicOn = PlayerPrefs.GetInt(MusicKey, 1) == 1;
        bool sfxOn = PlayerPrefs.GetInt(SfxKey, 1) == 1;

        if (bgmSource != null) bgmSource.mute = !musicOn;
        if (sfxSource != null) sfxSource.mute = !sfxOn;
        if (grillLoopSource != null) grillLoopSource.mute = !sfxOn;
        if (panLoopSource != null) panLoopSource.mute = !sfxOn;
    }

    public void PlayBgm(AudioClip clip)
    {
        if (bgmSource == null || clip == null) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void PlayVictoryBgm() => PlayBgm(bgmVictory);
    public void PlayLoseBgm() => PlayBgm(bgmLose);

    public void PlaySfx(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayCountdownTick() => PlaySfx(sfxCountdown);
    public void PlayCountdownGo() => PlaySfx(sfxCountdownGo);
    public void PlayCookingDone() => PlaySfx(sfxCookingDone);
    public void PlayCustomerServed() => PlaySfx(sfxCustomerServed);
    public void PlayGrilling() => PlaySfx(sfxGrilling);
    public void PlayGulamanDone() => PlaySfx(sfxGulamanDone);
    public void PlayGulamanPour() => PlaySfx(sfxGulamanPour);
    public void PlayPlaceOnPlate() => PlaySfx(sfxPlaceOnPlate);
    public void PlayTapIngredient() => PlaySfx(sfxTapIngredient);
    public void PlayTapStation() => PlaySfx(sfxTapStation);
    public void PlayUiClick() => PlaySfx(sfxUiClick);

    public void StartGrillLoop()
    {
        if (grillLoopSource == null || sfxSizzleLoopGrill == null) return;
        if (grillLoopSource.clip != sfxSizzleLoopGrill)
            grillLoopSource.clip = sfxSizzleLoopGrill;
        if (!grillLoopSource.isPlaying)
            grillLoopSource.Play();
    }

    public void StopGrillLoop()
    {
        if (grillLoopSource == null) return;
        if (grillLoopSource.isPlaying)
            grillLoopSource.Stop();
    }

    public void StartPanLoop()
    {
        if (panLoopSource == null || sfxSizzleLoopPan == null) return;
        if (panLoopSource.clip != sfxSizzleLoopPan)
            panLoopSource.clip = sfxSizzleLoopPan;
        if (!panLoopSource.isPlaying)
            panLoopSource.Play();
    }

    public void StopPanLoop()
    {
        if (panLoopSource == null) return;
        if (panLoopSource.isPlaying)
            panLoopSource.Stop();
    }
}
