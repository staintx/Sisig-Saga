using UnityEngine;
using System.Collections.Generic;

public class CustomerSpawner : MonoBehaviour
{
    public Customer customerPrefab;
    public Transform[] slots;

    public CustomerLook[] customerLooks; // size = 5 (each with idle/happy/angry)

    [Header("Entrance/Exit")]
    public float entryDelaySeconds = 0.8f;
    public float entryMoveSeconds = 5f;
    public float exitMoveSeconds = 3f;
    public float moveOffsetX = 800f;
    public float servedEmoteSeconds = 0.8f;

    [Header("Orders")]
    public int minOrders = 1;
    public int maxOrders = 3;

    [Header("Sisig Garnish Orders")]
    public int minSisigGarnish = 0;
    public int maxSisigGarnish = 2;

    public int totalCustomers;

    GameplayManager manager;
    int spawned;
    int active;
    readonly List<Customer> activeCustomers = new();

    public void Init(GameplayManager gm)
    {
        manager = gm;
    }

    void Update()
    {
        if (manager == null || !manager.IsGameRunning) return;

        if (spawned < manager.CustomersTotal && active < slots.Length)
            SpawnCustomer();
    }

    void SpawnCustomer()
    {
        var slot = GetFreeSlot();
        if (slot == null) return;

        int orderCount = Mathf.Clamp(Random.Range(minOrders, maxOrders + 1), 1, 10);
        List<FoodType> foods = new();
        for (int i = 0; i < orderCount; i++)
            foods.Add(manager.GetRandomOrderFood());

        List<IngredientType> garnishOrders = BuildGarnishOrders(foods);

        var customer = Instantiate(customerPrefab, slot);
        customer.transform.localPosition = Vector3.zero;

        var drop = customer.GetComponent<CustomerDropZone>();
        drop.customer = customer;

        CustomerLook look = customerLooks[Random.Range(0, customerLooks.Length)];
        customer.Setup(foods, garnishOrders, manager.PatienceSeconds, manager, look, this, slot, servedEmoteSeconds);

        bool fromRight = Random.value > 0.5f;
        customer.BeginEntrance(fromRight, moveOffsetX, entryDelaySeconds, entryMoveSeconds, exitMoveSeconds);

        spawned++;
        active++;
        activeCustomers.Add(customer);
    }

    Transform GetFreeSlot()
    {
        foreach (var s in slots)
            if (s.childCount == 0) return s;
        return null;
    }

    public void OnCustomerLeft(Customer customer = null)
    {
        active = Mathf.Max(0, active - 1);
        if (customer != null)
            activeCustomers.Remove(customer);
        else
            activeCustomers.RemoveAll(c => c == null);
    }

    public bool TryServeGarnish(IngredientType garnish)
    {
        for (int i = 0; i < activeCustomers.Count; i++)
        {
            var customer = activeCustomers[i];
            if (customer == null) continue;

            if (customer.TryServeGarnish(garnish))
                return true;
        }

        return false;
    }

    List<IngredientType> BuildGarnishOrders(List<FoodType> foods)
    {
        List<IngredientType> garnishOrders = new();
        if (manager == null || foods == null) return garnishOrders;
        if (!foods.Contains(FoodType.Sisig) && !foods.Contains(FoodType.SisigEgg)) return garnishOrders;

        List<IngredientType> options = manager.GetSisigGarnishOptions();
        if (options == null || options.Count == 0) return garnishOrders;

        int count = Mathf.Clamp(Random.Range(minSisigGarnish, maxSisigGarnish + 1), 0, options.Count);
        for (int i = 0; i < count; i++)
        {
            int pick = Random.Range(0, options.Count);
            garnishOrders.Add(options[pick]);
            options.RemoveAt(pick);
        }

        return garnishOrders;
    }
}