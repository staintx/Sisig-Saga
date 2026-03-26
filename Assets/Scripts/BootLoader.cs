using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BootLoader : MonoBehaviour
{
    public float loadTime = 2.5f;
    public Image loadingBarFill;

    float timer;

    void Update()
    {
        timer += Time.deltaTime;
        float progress = Mathf.Clamp01(timer / loadTime);

        if (loadingBarFill != null)
            loadingBarFill.fillAmount = progress;

        if (progress >= 1f)
            SceneManager.LoadScene("Menus");
    }
}