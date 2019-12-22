using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoSideDoor : MonoBehaviour
{
    Animator anim;
    public bool Locked;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && Locked == false)
        {
            anim.SetBool("Open", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            anim.SetBool("Open", false);
        }
    }
}
