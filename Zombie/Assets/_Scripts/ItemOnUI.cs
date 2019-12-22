using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemOnUI : MonoBehaviour, IPointerDownHandler
{
    Item item;
    PlayerData pData;
    Player player;

    private void Start()
    {
        item = GetComponent<Item>();
        pData = FindObjectOfType<PlayerData>();
        player = pData.GetComponent<Player>();
    }

    public  void OnPointerDown(PointerEventData eventData)
    {
        if (item.data.useable)
        {
            pData.ItemUse(item.data);
        }
        if (item.data.isWeapon && item.data.name == "Hand Gun")
        {
            player.gun.SetActive(!player.gun.activeInHierarchy);
            player.handGunEquip = !player.handGunEquip;
            player.SetAmmoInfo();
        }
    }
}
