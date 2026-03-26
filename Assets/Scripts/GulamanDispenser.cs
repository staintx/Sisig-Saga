using UnityEngine;
using UnityEngine.UI;

public class GulamanDispenser : MonoBehaviour
{
    public Button button;
    public CookStation gulamanStation;
    public GameObject extraDispenserRoot;

    void Start()
    {
        button.onClick.AddListener(() =>
        {
            AudioManager.Instance?.PlayTapStation();
            AudioManager.Instance?.PlayGulamanPour();
            gulamanStation.StartCooking();
        });

        if (extraDispenserRoot != null)
            extraDispenserRoot.SetActive(UpgradeState.IsOwned(UpgradeKeys.Dispenser));
    }
}