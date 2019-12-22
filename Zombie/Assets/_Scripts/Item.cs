using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData data;
    public int count;

    [HideInInspector]
    public bool searchable()
    {
        if (Vector3.Distance(FindObjectOfType<Player>().transform.position, transform.position) <= 2f)
            return true;
        else
            return false;
    }
}
