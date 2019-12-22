using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locker : MonoBehaviour
{
    public bool locked;
    Animator anim;
    UIControl ui;
    [Header("四位數")]
    public string password;

    private void Start()
    {
        ui = FindObjectOfType<UIControl>();
        anim = GetComponentInParent<Animator>();
    }

    public void CallWindow()
    {
        if (locked)
        {
            ui.passwordWindow.gameObject.SetActive(true);
            ui.passwordWindow.from = gameObject;

            Time.timeScale = 0;
            AudioListener.pause = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            FindObjectOfType<Player>().playerControlActive = false;
        }
        else
        {
            anim.SetBool("Open", !anim.GetBool("Open"));
        }
    }

    public void AnimationOpen()
    {
        anim.SetBool("Open", true);
    }
}
