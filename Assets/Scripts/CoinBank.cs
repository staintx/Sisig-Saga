using UnityEngine;

public static class CoinBank
{
    const string CoinsKey = "Coins";

    public static int Coins
    {
        get => PlayerPrefs.GetInt(CoinsKey, 0);
        set
        {
            PlayerPrefs.SetInt(CoinsKey, Mathf.Max(0, value));
            PlayerPrefs.Save();
        }
    }

    public static bool Spend(int amount)
    {
        if (Coins < amount) return false;
        Coins -= amount;
        return true;
    }

    public static void Add(int amount)
    {
        Coins += amount;
    }
}