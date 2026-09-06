using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item_Information> items;

    [SerializeField]
    private Transform slotParent;
    [SerializeField]
    private Slot[] slots;
    public bool Inventory_Full;

#if UNITY_EDITOR
    private void OnValidate()
    {
        slots = slotParent.GetComponentsInChildren<Slot>();
    }
#endif

    void Awake()
    {
        FreshSlot();
    }
    private void Update()
    {
        FreshSlot();
    }
    public void FreshSlot()
    {
        int i = 0;
        for (; i < items.Count && i < slots.Length; i++)
        {
            slots[i].item = items[i];
        }
        for (; i < slots.Length; i++)
        {
            slots[i].item = null;
        }
    }

    public void AddItem(Item_Information _item)
    {
        if (items.Count < slots.Length)
        {
            items.Add(_item);
            FreshSlot();
            Inventory_Full = false;
        }
        if (items.Count == slots.Length)
        {
            Inventory_Full = true;
        }
    }
}