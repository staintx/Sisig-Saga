using UnityEngine;
using UnityEngine.EventSystems;

public class IsawSkewer : MonoBehaviour, IPointerClickHandler
{
    public CookStation grillStation;

    void Awake()
    {
        if (grillStation != null) return;

        foreach (var station in FindObjectsOfType<CookStation>())
        {
            if (station.foodType == FoodType.Isaw)
            {
                grillStation = station;
                break;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (grillStation == null) return;

        AudioManager.Instance?.PlayTapStation();

        if (grillStation.TryQueueCook())
            Destroy(gameObject);
    }
}