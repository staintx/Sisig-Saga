using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UiSfxButton : MonoBehaviour
{
    void Awake()
    {
        var button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(Play);
    }

    void Play()
    {
        AudioManager.Instance?.PlayUiClick();
    }
}
