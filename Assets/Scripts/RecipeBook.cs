using System.Collections.Generic;
using UnityEngine;

public class RecipeBook : MonoBehaviour
{
    [System.Serializable]
    public class Recipe
    {
        public FoodType food;
        public Sprite orderSprite;
        public Sprite cookedSprite;
        public int baseCoins = 10;
        public float cookTime = 5f;
    }

    public Recipe sisig;
    public Recipe sisigEgg;
    public Recipe isaw;
    public Recipe gulaman;

    public Recipe GetRecipe(FoodType food)
    {
        return food switch
        {
            FoodType.Sisig => sisig,
            FoodType.SisigEgg => sisigEgg,
            FoodType.Isaw => isaw,
            FoodType.Gulaman => gulaman,
            _ => sisig
        };
    }

    public List<IngredientType> GetPreIngredients(FoodType food, int level)
    {
        if (food != FoodType.Sisig) return new List<IngredientType>();

        return new List<IngredientType>
        {
            IngredientType.PorkBits,
            IngredientType.Onion
        };
    }

    public List<IngredientType> GetGarnish(FoodType food, int level)
    {
        if (food != FoodType.Sisig && food != FoodType.SisigEgg) return new List<IngredientType>();

        if (level >= 5) return new List<IngredientType>
        {
            IngredientType.Calamansi, IngredientType.Chilli, IngredientType.SoySauce
        };

        if (level >= 4) return new List<IngredientType>
        {
            IngredientType.Calamansi, IngredientType.Chilli
        };

        return new List<IngredientType>();
    }
}