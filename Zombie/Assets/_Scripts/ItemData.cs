using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class ItemData : ScriptableObject
{
    public string name;
    public Sprite art;
    public bool stackable;
    public bool useable;
    public bool isWeapon;

    public string callFunc;
}
