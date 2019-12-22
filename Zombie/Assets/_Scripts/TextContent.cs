using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextContent : MonoBehaviour
{
    [TextArea]
    public string content;

    UIControl ui;

    void Start()
    {
        ui = FindObjectOfType<UIControl>();
    }

    public void CallWindow()
    {
        ui.textContentWindow.SetActive(true);
        ui.textContentWindow.GetComponentInChildren<TextMeshProUGUI>().text = content;

        Time.timeScale = 0;
        AudioListener.pause = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        FindObjectOfType<Player>().playerControlActive = false;
    }
}
