using UnityEngine;
using UnityEngine.EventSystems;

public class CustomerDropZone : MonoBehaviour, IDropHandler
{
    public Customer customer;

    public void OnDrop(PointerEventData eventData)
    {
        if (customer == null) return;

        // ✅ 1) Try FoodItem (sisig/isaw/gulaman)
        var foodItem = eventData.pointerDrag?.GetComponent<FoodItem>();
        if (foodItem != null)
        {
            if (!foodItem.IsReady) return;

            if (customer.TryServe(foodItem.foodType))
            {
                foodItem.MarkServed();
                Destroy(foodItem.gameObject);
            }
            return;
        }

        // ✅ 2) Fallback: DraggableFood (if you still use it anywhere)
        var food = eventData.pointerDrag?.GetComponent<DraggableFood>();
        if (food == null) return;

        if (customer.TryServe(food.foodType))
        {
            food.MarkServed();
            Destroy(food.gameObject);
        }
    }
}