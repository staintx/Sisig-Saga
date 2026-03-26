using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CookStation : MonoBehaviour
{
    [System.Serializable]
    public class CookSlot
    {
        public Transform cookedAnchor;
        public Image progressFill;
        public Image cookingVisual;

        [HideInInspector] public bool cooking;
    }

    public FoodType foodType;

    public Image cookingVisual;
    public Image progressFill;

    public Sprite cookingSprite;
    public Sprite cookedSprite;
    public Sprite idleSprite;

    public FoodBuilder builder;
    public RecipeBook recipeBook;

    [Header("Queue Cooking (for Isaw)")]
    public bool useQueue = false;

    [Header("Multi-slot Grill (Isaw)")]
    public bool useSlots = false;
    public CookSlot[] slots;

    [Header("Ignore Ingredients (Isaw/Gulaman)")]
    public bool ignoreIngredients = false;

    [Header("Auto Spawn On Complete (Gulaman)")]
    public bool spawnOnComplete = false;

    [Header("Custom Spawn Anchor (Gulaman)")]
    public Transform spawnAnchor;   // ✅ NEW

    [Header("Audio")]
    public bool playGrillLoop;
    public bool playPanLoop;

    bool cooking;
    bool ready;
    int queued;

    public bool IsCooking => cooking;
    public bool HasFreeSlot
    {
        get
        {
            return GetFreeSlot() != null;
        }
    }

    void Awake()
    {
        InitFill(progressFill);

        if (cookingVisual != null)
        {
            if (idleSprite != null)
                cookingVisual.sprite = idleSprite;

            cookingVisual.enabled = (cookingVisual.sprite != null);
        }

        if (slots != null)
        {
            foreach (var s in slots)
            {
                InitFill(s.progressFill);
                if (s.cookingVisual != null)
                    s.cookingVisual.enabled = false;
            }
        }
    }

    void InitFill(Image fill)
    {
        if (fill == null) return;
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Radial360;
        fill.fillOrigin = 2;
        fill.fillAmount = 0f;
        fill.enabled = false;
    }

    public void Init(FoodBuilder fb, RecipeBook rb)
    {
        builder = fb;
        recipeBook = rb;
    }

    public void QueueCook()
    {
        queued++;
        if (useSlots)
            TryStartNextSlot();
        else if (!cooking && !ready)
            StartCooking();
    }

    public bool TryQueueCook()
    {
        if (useSlots)
        {
            if (GetFreeSlot() == null) return false;
        }

        if (!ignoreIngredients && !builder.CanCook(foodType))
            return false;

        QueueCook();
        return true;
    }

    void Update()
    {
        if (useSlots && queued > 0)
            TryStartNextSlot();
    }

    void TryStartNextSlot()
    {
        if (queued <= 0) return;

        var slot = GetFreeSlot();
        if (slot == null) return;

        if (!ignoreIngredients && !builder.CanCook(foodType))
            return;

        queued--;

        var recipe = recipeBook.GetRecipe(foodType);
        float cookTime = recipe.cookTime * UpgradeState.GetCookTimeMultiplier(foodType);
        StartCoroutine(CookRoutineSlot(slot, cookTime));
    }

    CookSlot GetFreeSlot()
    {
        if (slots == null) return null;

        foreach (var s in slots)
        {
            if (s == null) continue;

            bool hasCookedItem = s.cookedAnchor != null && s.cookedAnchor.childCount > 0;
            if (!s.cooking && !hasCookedItem)
                return s;
        }
        return null;
    }

    public void StartCooking()
    {
        if (cooking || ready) return;
        if (!ignoreIngredients && !builder.CanCook(foodType)) return;

        var recipe = recipeBook.GetRecipe(foodType);
        float cookTime = recipe.cookTime * UpgradeState.GetCookTimeMultiplier(foodType);

        if (cookingVisual != null && cookingSprite != null)
        {
            cookingVisual.sprite = cookingSprite;
            cookingVisual.enabled = true;
        }

        if (playGrillLoop)
            AudioManager.Instance?.StartGrillLoop();
        if (playPanLoop)
            AudioManager.Instance?.StartPanLoop();

        if (progressFill != null)
        {
            progressFill.fillAmount = 0f;
            progressFill.enabled = true;
        }

        StartCoroutine(CookRoutine(cookTime));
    }

    IEnumerator CookRoutine(float cookTime)
    {
        cooking = true;
        float t = 0f;

        while (t < cookTime)
        {
            t += Time.deltaTime;
            if (progressFill != null) progressFill.fillAmount = t / cookTime;
            yield return null;
        }

        cooking = false;

        if (progressFill != null)
            progressFill.enabled = false;

        if (spawnOnComplete && foodType == FoodType.Gulaman)
        {
            if (spawnAnchor != null)
                builder.SpawnFoodAt(foodType, spawnAnchor);
            else
                builder.SpawnFood(foodType);

            AudioManager.Instance?.PlayGulamanDone();

            if (cookingVisual != null && idleSprite != null)
            {
                cookingVisual.sprite = idleSprite;
                cookingVisual.enabled = true;
            }

            yield break;
        }

        ready = true;

        if (playGrillLoop)
            AudioManager.Instance?.StopGrillLoop();
        if (playPanLoop)
            AudioManager.Instance?.StopPanLoop();

        AudioManager.Instance?.PlayCookingDone();

        if (cookingVisual != null && cookedSprite != null)
        {
            cookingVisual.sprite = cookedSprite;
            cookingVisual.enabled = true;
        }
    }

    IEnumerator CookRoutineSlot(CookSlot slot, float cookTime)
    {
        slot.cooking = true;

        if (slot.cookingVisual != null && cookingSprite != null)
        {
            slot.cookingVisual.sprite = cookingSprite;
            slot.cookingVisual.enabled = true;
        }

        if (slot.progressFill != null)
        {
            slot.progressFill.fillAmount = 0f;
            slot.progressFill.enabled = true;
        }

        float t = 0f;
        while (t < cookTime)
        {
            t += Time.deltaTime;
            if (slot.progressFill != null) slot.progressFill.fillAmount = t / cookTime;
            yield return null;
        }

        slot.cooking = false;

        if (slot.progressFill != null)
            slot.progressFill.enabled = false;

        if (slot.cookingVisual != null)
            slot.cookingVisual.enabled = false;

        if (slot.cookedAnchor != null)
            builder.SpawnFoodAt(foodType, slot.cookedAnchor);

        AudioManager.Instance?.PlayCookingDone();
    }

    public void OnStationTapped()
    {
        if (!ready) return;

        AudioManager.Instance?.PlayTapStation();

        if (!builder.TrySpawnFood(foodType))
            return;

        ready = false;

        if (progressFill != null)
            progressFill.fillAmount = 0f;

        if (cookingVisual != null)
            cookingVisual.enabled = false;

        if (useQueue && queued > 0)
            StartCooking();
    }
}