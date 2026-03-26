using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableFood : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public FoodType foodType;

    Canvas canvas;
    CanvasGroup canvasGroup;
    Transform originalParent;
    bool served;

    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (served) return;
        originalParent = transform.parent;
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(canvas.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (served) return;
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (served) return;
        canvasGroup.blocksRaycasts = true;
        transform.SetParent(originalParent);
        transform.localPosition = Vector3.zero;
    }

    public void MarkServed()
    {
        served = true;
    }
}