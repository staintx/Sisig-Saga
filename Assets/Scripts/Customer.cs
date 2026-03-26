using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    public Image customerImage;
    public GameObject orderBubbleRoot;
    public Image orderImage;
    public Image[] orderIconSlots;
    public Image[] garnishIconSlots;
    public Image patienceFill;
    public Sprite servedCheckSprite;
    public Sprite calamansiSprite;
    public Sprite chilliSprite;
    public Sprite soySprite;

    float patienceTime;
    float timeLeft;
    GameplayManager manager;
    CustomerSpawner spawner; // ✅ NEW
    Transform slotTransform;

    bool isArrived;
    bool isLeaving;
    bool entryFromRight;
    float exitMoveSeconds;
    float moveOffsetX;
    float servedEmoteSeconds;

    CustomerLook look;

    class OrderSlot
    {
        public FoodType food;
        public Image icon;
        public bool served;
    }

    class GarnishSlot
    {
        public IngredientType garnish;
        public Image icon;
        public bool served;
    }

    readonly List<OrderSlot> orders = new();
    readonly List<Image> orderIcons = new();
    readonly List<GarnishSlot> garnishes = new();
    readonly List<Image> garnishIcons = new();

    void Awake()
    {
        SetOrderUIVisible(false);
    }

    public void Setup(
        List<FoodType> foods,
        List<IngredientType> garnishOrders,
        float patienceSeconds,
        GameplayManager gm,
        CustomerLook customerLook,
        CustomerSpawner sp, // ✅ NEW
        Transform slot,
        float servedEmote
    )
    {
        look = customerLook;
        if (look != null && look.idle != null)
            customerImage.sprite = look.idle;

        patienceTime = patienceSeconds;
        timeLeft = patienceSeconds;
        manager = gm;
        spawner = sp; // ✅ NEW
        slotTransform = slot;
        servedEmoteSeconds = Mathf.Max(0f, servedEmote);

        BuildOrders(foods, gm);
        BuildGarnishes(garnishOrders);

        SetOrderUIVisible(false);
    }

    public void BeginEntrance(bool fromRight, float offsetX, float delaySeconds, float moveSeconds, float exitSeconds)
    {
        entryFromRight = fromRight;
        exitMoveSeconds = exitSeconds;
        moveOffsetX = Mathf.Abs(offsetX);

        SetOrderUIVisible(false);

        Vector3 targetLocal = GetLocalPosition();
        Vector3 startLocal = targetLocal + new Vector3(fromRight ? moveOffsetX : -moveOffsetX, 0f, 0f);
        SetLocalPosition(startLocal);

        StartCoroutine(EntranceRoutine(targetLocal, delaySeconds, moveSeconds));
    }

    void Update()
    {
        if (!isArrived || isLeaving) return;
        if (timeLeft <= 0f) return;

        timeLeft -= Time.deltaTime;
        patienceFill.fillAmount = Mathf.Clamp01(timeLeft / patienceTime);

        if (timeLeft <= 0f)
        {
            if (look != null && look.angry != null)
                customerImage.sprite = look.angry;

            manager.OnCustomerFailed();
            StartExit();
        }
    }

    public bool TryServe(FoodType food)
    {
        if (!isArrived || isLeaving) return false;

        int index = FindUnservedOrderIndex(food);
        if (index < 0) return false;

        if (look != null && look.happy != null)
            customerImage.sprite = look.happy;

        MarkOrderServed(index);
        manager.OnDishServed(food);

        if (AllOrdersServed())
        {
            AudioManager.Instance?.PlayCustomerServed();
            manager.OnCustomerServed();
            StartCoroutine(ExitAfterDelay(servedEmoteSeconds));
        }
        return true;
    }

    public bool TryServeGarnish(IngredientType garnish)
    {
        if (!isArrived || isLeaving) return false;

        int index = FindUnservedGarnishIndex(garnish);
        if (index < 0) return false;

        MarkGarnishServed(index);

        if (AllOrdersServed())
        {
            manager.OnCustomerServed();
            StartCoroutine(ExitAfterDelay(servedEmoteSeconds));
        }

        return true;
    }

    IEnumerator EntranceRoutine(Vector3 targetLocal, float delaySeconds, float moveSeconds)
    {
        if (delaySeconds > 0f)
            yield return new WaitForSeconds(delaySeconds);

        yield return MoveToLocal(targetLocal, moveSeconds);

        SetOrderUIVisible(true);
        isArrived = true;
    }

    void StartExit()
    {
        if (isLeaving) return;
        isLeaving = true;

        SetOrderUIVisible(false);

        spawner?.OnCustomerLeft(this);
        DetachFromSlot();

        Vector3 exitTarget = GetLocalPosition() + new Vector3(entryFromRight ? moveOffsetX : -moveOffsetX, 0f, 0f);

        StartCoroutine(ExitRoutine(exitTarget));
    }

    IEnumerator ExitRoutine(Vector3 targetLocal)
    {
        yield return MoveToLocal(targetLocal, exitMoveSeconds);
        Destroy(gameObject);
    }

    IEnumerator ExitAfterDelay(float delaySeconds)
    {
        if (delaySeconds > 0f)
            yield return new WaitForSeconds(delaySeconds);
        StartExit();
    }

    IEnumerator MoveToLocal(Vector3 targetLocal, float moveSeconds)
    {
        float duration = Mathf.Max(0.01f, moveSeconds);
        Vector3 start = GetLocalPosition();
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / duration);
            SetLocalPosition(Vector3.Lerp(start, targetLocal, u));
            yield return null;
        }
        SetLocalPosition(targetLocal);
    }

    void SetOrderUIVisible(bool visible)
    {
        if (orderBubbleRoot != null)
            orderBubbleRoot.SetActive(visible);
        if (orderImage != null)
            orderImage.enabled = visible;
        if (patienceFill != null)
            patienceFill.enabled = visible;
    }

    void DetachFromSlot()
    {
        if (slotTransform == null) return;
        Transform newParent = slotTransform.parent != null ? slotTransform.parent : null;
        transform.SetParent(newParent, true);
    }

    Vector3 GetLocalPosition()
    {
        var rect = transform as RectTransform;
        if (rect != null)
            return rect.anchoredPosition3D;
        return transform.localPosition;
    }

    void SetLocalPosition(Vector3 value)
    {
        var rect = transform as RectTransform;
        if (rect != null)
            rect.anchoredPosition3D = value;
        else
            transform.localPosition = value;
    }

    void BuildOrders(List<FoodType> foods, GameplayManager gm)
    {
        orders.Clear();
        ClearExtraOrderIcons();

        if (foods == null || foods.Count == 0)
            return;

        for (int i = 0; i < foods.Count && i < orderIcons.Count; i++)
        {
            Image icon = orderIcons[i];
            if (icon != null && gm != null && gm.recipeBook != null)
                icon.sprite = gm.recipeBook.GetRecipe(foods[i]).orderSprite;
            if (icon != null)
            {
                icon.gameObject.SetActive(true);
                icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 1f);
            }

            orders.Add(new OrderSlot { food = foods[i], icon = icon, served = false });
        }
    }

    int FindUnservedOrderIndex(FoodType food)
    {
        for (int i = 0; i < orders.Count; i++)
        {
            if (!orders[i].served && orders[i].food == food)
                return i;
        }
        return -1;
    }

    void MarkOrderServed(int index)
    {
        if (index < 0 || index >= orders.Count) return;
        orders[index].served = true;

        var icon = orders[index].icon;
        if (icon == null) return;

        if (servedCheckSprite != null)
            icon.sprite = servedCheckSprite;
        else
            icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 0.4f);
    }

    bool AllOrdersServed()
    {
        for (int i = 0; i < orders.Count; i++)
        {
            if (!orders[i].served) return false;
        }
        for (int i = 0; i < garnishes.Count; i++)
        {
            if (!garnishes[i].served) return false;
        }
        return orders.Count > 0;
    }

    void ClearExtraOrderIcons()
    {
        orderIcons.Clear();
        if (orderIconSlots != null && orderIconSlots.Length > 0)
        {
            orderIcons.AddRange(orderIconSlots);
        }
        else if (orderImage != null)
        {
            orderIcons.Add(orderImage);
        }

        for (int i = 0; i < orderIcons.Count; i++)
        {
            if (orderIcons[i] != null)
                orderIcons[i].gameObject.SetActive(false);
        }
    }

    void ClearExtraGarnishIcons()
    {
        garnishIcons.Clear();
        if (garnishIconSlots != null && garnishIconSlots.Length > 0)
            garnishIcons.AddRange(garnishIconSlots);

        for (int i = 0; i < garnishIcons.Count; i++)
        {
            if (garnishIcons[i] != null)
                garnishIcons[i].gameObject.SetActive(false);
        }
    }

    void BuildGarnishes(List<IngredientType> garnishOrders)
    {
        garnishes.Clear();
        ClearExtraGarnishIcons();

        if (garnishOrders == null || garnishOrders.Count == 0)
            return;

        for (int i = 0; i < garnishOrders.Count && i < garnishIcons.Count; i++)
        {
            Image icon = garnishIcons[i];
            if (icon != null)
                icon.sprite = GetGarnishSprite(garnishOrders[i]);
            if (icon != null)
            {
                icon.gameObject.SetActive(true);
                icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 1f);
            }

            garnishes.Add(new GarnishSlot { garnish = garnishOrders[i], icon = icon, served = false });
        }
    }

    int FindUnservedGarnishIndex(IngredientType garnish)
    {
        for (int i = 0; i < garnishes.Count; i++)
        {
            if (!garnishes[i].served && garnishes[i].garnish == garnish)
                return i;
        }
        return -1;
    }

    void MarkGarnishServed(int index)
    {
        if (index < 0 || index >= garnishes.Count) return;
        garnishes[index].served = true;

        var icon = garnishes[index].icon;
        if (icon == null) return;

        if (servedCheckSprite != null)
            icon.sprite = servedCheckSprite;
        else
            icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 0.4f);
    }

    Sprite GetGarnishSprite(IngredientType garnish)
    {
        return garnish switch
        {
            IngredientType.Calamansi => calamansiSprite,
            IngredientType.Chilli => chilliSprite,
            IngredientType.SoySauce => soySprite,
            _ => null
        };
    }
}