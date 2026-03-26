using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public string upgradeKey;
    public int price;

    public Button buyButton;
    public Image buyButtonImage;
    public GameObject ownedImage;

    public Sprite buySprite;
    public Sprite ownedSprite; // check_btn

    void Awake()
    {
        EnsureBindings();
    }

    void OnEnable()
    {
        EnsureBindings();
        Refresh();
    }

    void EnsureBindings()
    {
        if (buyButton == null)
            buyButton = GetComponentInChildren<Button>(true);

        if (buyButtonImage == null && buyButton != null)
            buyButtonImage = buyButton.GetComponent<Image>();

        if (buyButton != null)
        {
            buyButton.onClick.RemoveListener(Buy);
            buyButton.onClick.AddListener(Buy);
        }
    }

    public void Refresh()
    {
        bool owned = PlayerPrefs.GetInt(upgradeKey, 0) == 1;
        if (buyButton != null)
        {
            buyButton.interactable = !owned;
            buyButton.gameObject.SetActive(!owned);
        }

        if (buyButtonImage != null && (buySprite != null || ownedSprite != null))
            buyButtonImage.sprite = owned ? ownedSprite : buySprite;

        if (ownedImage != null)
            ownedImage.SetActive(owned);
    }

    public void Buy()
    {
        if (PlayerPrefs.GetInt(upgradeKey, 0) == 1)
        {
            Refresh();
            return;
        }

        if (CoinBank.Spend(price))
        {
            PlayerPrefs.SetInt(upgradeKey, 1);
            PlayerPrefs.Save();
            Refresh();
            FindAnyObjectByType<ShopPanelController>()?.RefreshCoins();
        }
    }
}