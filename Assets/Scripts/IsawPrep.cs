using UnityEngine;
using UnityEngine.UI;

public class IsawPrep : MonoBehaviour
{
    public Button rawIsawButton;
    public Transform[] containerSlots;
    public IsawSkewer skewerPrefab;

    void Start()
    {
        rawIsawButton.onClick.AddListener(SpawnSkewer);
    }

    void SpawnSkewer()
    {
        foreach (var slot in containerSlots)
        {
            if (slot.childCount == 0)
            {
                Instantiate(skewerPrefab, slot).transform.localPosition = Vector3.zero;
                break;
            }
        }
    }
}