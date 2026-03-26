using UnityEngine;
using TMPro;

public class ShopPanelController : MonoBehaviour
{
    public TMP_Text coinsText;

    void OnEnable()
    {
        RefreshCoins();
        RefreshAllItems();
    }

    public void RefreshCoins()
    {
        if (coinsText != null)
            coinsText.text = CoinBank.Coins.ToString();
    }

    void RefreshAllItems()
    {
        foreach (var item in GetComponentsInChildren<ShopItemUI>(true))
            item.Refresh();
    }
}