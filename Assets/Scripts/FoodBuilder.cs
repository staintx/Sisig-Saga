using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodBuilder : MonoBehaviour
{
    public RecipeBook recipeBook;
    public Transform outputPoint;
    public FoodItem foodPrefab;

    public PlateSlot[] plates;

    public CookStation[] sisigStations;
    public CookStation isawStation;
    public CookStation gulamanStation;

    public Button porkBtn;
    public Button onionBtn;
    public Button eggBtn;
    public Button calamansiBtn;
    public Button chilliBtn;
    public Button soyBtn;

    public GameplayManager gameplayManager;

    List<IngredientType> currentIngredients = new();
    int currentLevel;

    public void Init(int level)
    {
        currentLevel = level;
        HookButtons();

        if (gameplayManager == null)
            gameplayManager = FindAnyObjectByType<GameplayManager>();
    }

    void HookButtons()
    {
        porkBtn.onClick.AddListener(AddPorkBits);
        onionBtn.onClick.AddListener(AddOnion);
        eggBtn.onClick.AddListener(AddEgg);
        calamansiBtn.onClick.AddListener(AddCalamansi);
        chilliBtn.onClick.AddListener(AddChilli);
        soyBtn.onClick.AddListener(AddSoy);
    }

    public void AddPorkBits() => AddIngredient(IngredientType.PorkBits);
    public void AddOnion() => AddIngredient(IngredientType.Onion);
    public void AddEgg() => ApplyEggToNextPlate();

    public void AddCalamansi() => ServeGarnish(IngredientType.Calamansi);
    public void AddChilli() => ServeGarnish(IngredientType.Chilli);
    public void AddSoy() => ServeGarnish(IngredientType.SoySauce);

    public void AddIngredient(IngredientType ing)
    {
        AudioManager.Instance?.PlayTapIngredient();
        currentIngredients.Add(ing);
        TryAutoCookSisig();
    }

    void ApplyEggToNextPlate()
    {
        if (plates == null || recipeBook == null) return;

        Sprite eggSprite = recipeBook.GetRecipe(FoodType.SisigEgg).cookedSprite;
        for (int i = 0; i < plates.Length; i++)
        {
            var plate = plates[i];
            if (plate == null) continue;

            var food = plate.GetComponentInChildren<FoodItem>();
            if (food == null) continue;

            if (food.TryApplyEgg(eggSprite))
                return;
        }
    }

    void TryAutoCookSisig()
    {
        if (!CanCook(FoodType.Sisig, previewOnly: true)) return;

        foreach (var pan in sisigStations)
        {
            if (!pan.IsCooking)
            {
                pan.StartCooking();
                break;
            }
        }
    }

    public bool CanCook(FoodType food, bool previewOnly = false)
    {
        var need = recipeBook.GetPreIngredients(food, currentLevel);

        foreach (var req in need)
        {
            if (!currentIngredients.Contains(req)) return false;
        }

        if (!previewOnly)
        {
            foreach (var req in need) currentIngredients.Remove(req);
        }

        return true;
    }

    public void SpawnFood(FoodType food)
    {
        Transform target = GetFreePlate();
        if (target == null) return;
        SpawnFoodAt(food, target, setActive: true);
    }

    public bool TrySpawnFood(FoodType food)
    {
        Transform target = GetFreePlate();
        if (target == null) return false;
        SpawnFoodAt(food, target, setActive: true);
        return true;
    }

    public FoodItem SpawnFoodAt(FoodType food, Transform target, bool setActive = false)
    {
        var recipe = recipeBook.GetRecipe(food);
        List<IngredientType> garnish = new();

        var item = Instantiate(foodPrefab, target);
        item.transform.localPosition = Vector3.zero;

        var img = item.GetComponent<Image>();
        img.sprite = recipe.cookedSprite;

        // ✅ size override for Isaw
        if (food == FoodType.Isaw)
        {
            var rect = item.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(75.2303f, 104.5509f);
        }
        // ✅ size override for Gulaman
        else if (food == FoodType.Gulaman)
        {
            var rect = item.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(50.637f, 70.2201f);
        }

        item.Init(food, garnish, this);

        AudioManager.Instance?.PlayPlaceOnPlate();

        if (setActive)
            SetActiveFood(item);

        return item;
    }

    Transform GetFreePlate()
    {
        if (plates == null) return null;

        foreach (var p in plates)
        {
            if (p != null && !p.HasFood)
            {
                var anchor = p.transform.Find("FoodAnchor");
                return anchor != null ? anchor : p.transform;
            }
        }
        return null;
    }

    public void SetActiveFood(FoodItem item) { }

    void ServeGarnish(IngredientType ing)
    {
        AudioManager.Instance?.PlayTapIngredient();
        if (gameplayManager != null)
            gameplayManager.TryServeGarnish(ing);
    }
}