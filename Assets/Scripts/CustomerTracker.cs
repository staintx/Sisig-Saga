using UnityEngine;

public class CustomerTracker : MonoBehaviour
{
    CustomerSpawner spawner;

    public void Init(CustomerSpawner sp)
    {
        spawner = sp;
    }

    void OnDestroy()
    {
        if (spawner != null)
            spawner.OnCustomerLeft(GetComponent<Customer>());
    }
}