using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FoodItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public FoodType foodType;
    public bool IsReady => garnishNeeded.Count == 0;

    Canvas canvas;
    CanvasGroup canvasGroup;
    Transform originalParent;
    bool served;
    bool hasEgg;

    List<IngredientType> garnishNeeded = new();
    FoodBuilder builder;

    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Init(FoodType type, List<IngredientType> garnish, FoodBuilder fb)
    {
        foodType = type;
        garnishNeeded = new List<IngredientType>(garnish);
        builder = fb;
        hasEgg = type == FoodType.SisigEgg;
    }

    public bool TryApplyEgg(Sprite eggSprite)
    {
        if (foodType != FoodType.Sisig || hasEgg) return false;
        hasEgg = true;
        foodType = FoodType.SisigEgg;

        var img = GetComponent<UnityEngine.UI.Image>();
        if (img != null && eggSprite != null)
            img.sprite = eggSprite;

        return true;
    }

    public void AddGarnish(IngredientType ing)
    {
        if (garnishNeeded.Contains(ing))
            garnishNeeded.Remove(ing);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        builder.SetActiveFood(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (served || !IsReady) return;
        originalParent = transform.parent;
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(canvas.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (served || !IsReady) return;
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (served || !IsReady) return;
        canvasGroup.blocksRaycasts = true;
        transform.SetParent(originalParent);
        transform.localPosition = Vector3.zero;
    }

    public void MarkServed() => served = true;
}