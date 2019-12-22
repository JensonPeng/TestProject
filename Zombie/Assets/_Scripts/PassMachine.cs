using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassMachine : MonoBehaviour
{
    Material[] allMaterials;
    bool locked = true;
    PlayerData data;

    public GameObject attachWith;

    void Start()
    {
        allMaterials = GetComponent<Renderer>().materials;
        data = FindObjectOfType<PlayerData>();
    }

    public void Open()
    {
        System.Predicate<PlayerData.ItemInList> idCard = new System.Predicate<PlayerData.ItemInList>(item => item.name == "IDCard");

        if (locked)
        {
            if (data.itemList.Find(idCard) != null)
            {
                foreach (Material m in allMaterials)
                {
                    if (m.name == "Red (Instance)")
                    {
                        m.color = Color.green;
                        m.SetColor("_EmissionColor", Color.green);
                    }
                }

                AttachTo();
            }
            else
                print("do alert");
        }
    }

    void AttachTo()
    {
        if (attachWith.GetComponent<TwoSideDoor>())
            attachWith.GetComponent<TwoSideDoor>().Locked = false;
    }
}
