using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIControl : MonoBehaviour
{
    public Animator IAnim;
    public TextMeshProUGUI itemNameText;
    [Header("Windows")]
    public GameObject inventory;
    public GameObject itemPrefab;
    [Header("UI")]
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI heartPulseText;
    public GameObject crosshair;
    public PasswordWindow passwordWindow;
    public GameObject textContentWindow;

    Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    float heartPulseUpdateTimer;
    [HideInInspector]public bool heartPulseActive;
    private void Update()
    {
        heartPulseText.transform.position = new Vector3(inventory.transform.position.x, heartPulseText.transform.position.y, heartPulseText.transform.position.z);
        heartPulseUpdateTimer += Time.unscaledDeltaTime;
        if (heartPulseActive)
        {
            if (heartPulseUpdateTimer >= 1)
            {
                heartPulseUpdateTimer = 0;
                heartPulseText.text = "Bpm : " + (int)player.GetComponent<PlayerData>().heartPulse;
            }
        }
        else
            heartPulseText.text = "";
    }

    public void InventoryTrigger()
    {
        if (IAnim.GetBool("Open"))
        {//關閉道具欄
            IAnim.SetBool("Open", false);

            Time.timeScale = 1;
            AudioListener.pause = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            player.playerControlActive = true;
            heartPulseActive = false;
        }
        else
        {//開啟道具欄
            IAnim.SetBool("Open", true);

            Time.timeScale = 0;
            AudioListener.pause = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            player.playerControlActive = false;
            heartPulseActive = true;

            InventoryUpdate();
        }
    }
    public void InventoryUpdate()
    {
        foreach (Item a in inventory.GetComponentsInChildren<Item>())
            Destroy(a.gameObject);

        if (player.GetComponent<PlayerData>().itemList != null)
        {
            foreach (PlayerData.ItemInList _data in player.GetComponent<PlayerData>().itemList)
            {
                GameObject clone = Instantiate(itemPrefab, inventory.transform);
                clone.name = _data.name;
                clone.GetComponent<Item>().data = _data.data;
                clone.GetComponent<Image>().sprite = _data.art;
                clone.GetComponentInChildren<TextMeshProUGUI>().text = "x" + _data.count;
            }
        }
    }

    public void ItemNameTextUpdate(Vector3 position)
    {
        itemNameText.transform.position = position;
        itemNameText.text = "";
    }
    public void AmmoTextUpdate(int load, int unload)
    {
        if (player.handGunEquip)
            ammoText.text = load + " / " + unload;
        else
            ammoText.text = "";
    }

    public void ClosePaperWindow()
    {
        textContentWindow.SetActive(false);
        textContentWindow.GetComponentInChildren<TextMeshProUGUI>().text = "";

        Time.timeScale = 1;
        AudioListener.pause = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        FindObjectOfType<Player>().playerControlActive = true;
    }

    public void GoToTitle()
    {
        SceneManager.LoadScene("Title");
    }
}
