using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFunctions : MonoBehaviour
{
    PlayerData playerData;


    private void Start()
    {
        playerData = FindObjectOfType<PlayerData>();
    }

    void FirstAid()
    {
        playerData.hp += 100;
        print("+100");
    }
}
