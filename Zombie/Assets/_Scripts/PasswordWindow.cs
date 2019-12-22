using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PasswordWindow : MonoBehaviour
{
    [HideInInspector]public GameObject from;
    public TextMeshProUGUI[] numbersText;
    int[] numbers = new int[4] { 0, 0, 0, 0 };

    void Start()
    {
        for (int i = 0; i < numbersText.Length; i++)
            numbersText[i].text = numbers[i].ToString();
    }

    #region plus
    public void Plus1()
    {
        numbers[0] += 1;
        if (numbers[0] > 9)
            numbers[0] = 0;

        TextUpdate();
    }
    public void Plus2()
    {
        numbers[1] += 1;
        if (numbers[1] > 9)
            numbers[1] = 0;

        TextUpdate();
    }
    public void Plus3()
    {
        numbers[2] += 1;
        if (numbers[2] > 9)
            numbers[2] = 0;

        TextUpdate();
    }
    public void Plus4()
    {
        numbers[3] += 1;
        if (numbers[3] > 9)
            numbers[3] = 0;

        TextUpdate();
    }
    #endregion

    #region minus
    public void Minus1()
    {
        numbers[0] -= 1;
        if (numbers[0] < 0)
            numbers[0] = 9;

        TextUpdate();
    }
    public void Minus2()
    {
        numbers[1] -= 1;
        if (numbers[1] < 0)
            numbers[1] = 9;

        TextUpdate();
    }
    public void Minus3()
    {
        numbers[2] -= 1;
        if (numbers[2] < 0)
            numbers[2] = 9;

        TextUpdate();
    }
    public void Minus4()
    {
        numbers[3] -= 1;
        if (numbers[3] < 0)
            numbers[3] = 9;

        TextUpdate();
    }
    #endregion

    void TextUpdate()
    {
        for (int i = 0; i < numbersText.Length; i++)
            numbersText[i].text = numbers[i].ToString();
    }

    public void Confirm()
    {
        string answer = numbers[0].ToString() + numbers[1].ToString() + numbers[2].ToString() + numbers[3].ToString();
        if (from.GetComponent<Locker>().password == answer)
        {
            from.GetComponent<Locker>().locked = false;
            from.GetComponent<Locker>().AnimationOpen();
            gameObject.SetActive(false);
            TimeGo();
        }
        else
        {
            for (int i = 0; i < numbersText.Length; i++)
            {
                numbers[i] = 0;
                numbersText[i].text = "0";
            }
        }
    }
    public void Cancel()
    {
        gameObject.SetActive(false);
        TimeGo();
    }

    void TimeGo()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        FindObjectOfType<Player>().playerControlActive = true;
    }
}
