using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public class ItemInList
    {
        public string name;
        public ItemData data;
        public Sprite art;
        public int count;
    }

    [HideInInspector] public float hp;
    [HideInInspector] public float maxHp;
    [HideInInspector] public float heartPulse;//  次 / 每分鐘


    [HideInInspector]
    public List<ItemInList> itemList = new List<ItemInList>();

    UIControl ui;

    private void Start()
    {
        ui = FindObjectOfType<UIControl>();
        maxHp = 100;
        hp = maxHp;
    }

    public void ItemAdd(ItemData newItem, int quantity)
    {
        System.Predicate<ItemInList> thatItem = new System.Predicate<ItemInList>(_data => _data.name == newItem.name);
        if (itemList.Find(thatItem) == null || newItem.stackable == false)
        {
            ItemInList i = new ItemInList();
            i.name = newItem.name;
            i.data = newItem;
            i.art = newItem.art;
            i.count = quantity;
            itemList.Add(i);
        }
        else
        {
            itemList.Find(thatItem).count += quantity;
        }
    }
    public void ItemUse(ItemData usedItem)
    {
        FindObjectOfType<ItemFunctions>().SendMessage(usedItem.callFunc);
        System.Predicate<ItemInList> thatItem = new System.Predicate<ItemInList>(_data => _data.name == usedItem.name);
        if (itemList.Find(thatItem).count > 1)
            itemList.Find(thatItem).count -= 1;
        else
            itemList.Remove(itemList.Find(thatItem));

        ui.InventoryUpdate();
    }
    public void ItemRemove()
    {

    }

}
