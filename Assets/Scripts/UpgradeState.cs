using UnityEngine;

public static class UpgradeState
{
    const float SisigCookMult = 0.75f;
    const float IsawCookMult = 0.75f;
    const float GulamanCookMult = 0.75f;

    const int SisigPriceBonus = 5;
    const int IsawPriceBonus = 5;

    public static bool IsOwned(string key)
    {
        return PlayerPrefs.GetInt(key, 0) == 1;
    }

    public static float GetCookTimeMultiplier(FoodType food)
    {
        float mult = 1f;

        if (food == FoodType.Sisig && IsOwned(UpgradeKeys.Stove))
            mult *= SisigCookMult;
        else if (food == FoodType.Isaw && IsOwned(UpgradeKeys.Grill))
            mult *= IsawCookMult;
        else if (food == FoodType.Gulaman && IsOwned(UpgradeKeys.Dispenser))
            mult *= GulamanCookMult;

        return mult;
    }

    public static int GetPriceBonus(FoodType food)
    {
        int bonus = 0;

        if ((food == FoodType.Sisig || food == FoodType.SisigEgg) && IsOwned(UpgradeKeys.MeatTray))
            bonus += SisigPriceBonus;
        else if (food == FoodType.Isaw && IsOwned(UpgradeKeys.IsawTray))
            bonus += IsawPriceBonus;

        return bonus;
    }
}
