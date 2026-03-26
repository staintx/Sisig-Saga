using UnityEngine;

public class PlateSlot : MonoBehaviour
{
    public bool HasFood
    {
        get
        {
            // If any child has a FoodItem (even under FoodAnchor), plate is occupied
            return GetComponentInChildren<FoodItem>(includeInactive: false) != null;
        }
    }
}