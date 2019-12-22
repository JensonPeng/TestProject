using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    #region 定義
    Transform player;
    Collider col;
    CharacterController controller;
    Transform mCamera;
    UIControl ui;
    PlayerData data;
    Animator anim;

    float h;
    float v;
    Vector3 velocity;


    float fireTimer;

    [HideInInspector] public bool playerControlActive = true;
    bool canFire;
    bool haveAmmo;
    [HideInInspector]public bool handGunEquip;
    bool scope;

    Vector3 fireRageOffset;

    public GameObject gun;
    [Header("Physics")]
    public float gravity;
    public Transform groundCheck;
    public float groundDistance = .25f;
    public LayerMask groundMask;
    bool isGrounded()
    {
        if (Physics.CheckSphere(groundCheck.position, groundDistance, groundMask))
            return true;
        else
            return false;
    }
    [Header("Effect")]
    public ParticleSystem muzzle;
    public GameObject impactEffect;
    [Header("Sound")]
    public GameObject impactSound;
    [Header("Status")]
    public float moveSpeed;
    float moveSpeedBase = 4f;
    public float jumpHeight;
    public float hitForce;
    public float fireDamage = 12f;
    public float fireRate;
    #endregion

    void Start()
    {
        player = GetComponent<Transform>();
        col = GetComponent<Collider>();
        controller = GetComponent<CharacterController>();
        mCamera = Camera.main.transform;
        ui = FindObjectOfType<UIControl>();
        data = GetComponent<PlayerData>();
        anim = GetComponent<Animator>();
    }

    #region 註記List內的物件
    System.Predicate<PlayerData.ItemInList> ninemmAmmo = new System.Predicate<PlayerData.ItemInList>(_ammo => _ammo.name == "Ammo(9mm)");
    #endregion

    bool PressE()
    {
        if (Input.GetKeyDown(KeyCode.E) 
            && !ui.IAnim.GetBool("Open") 
            && !ui.textContentWindow.gameObject.activeInHierarchy
            )
            return true;
        else
            return false;
    }
    bool PressTab()
    {
        if (Input.GetKeyDown(KeyCode.Tab)
            && !ui.passwordWindow.gameObject.activeInHierarchy
            && !ui.textContentWindow.gameObject.activeInHierarchy
            )
            return true;
        else
            return false;
    }

    void Update()
    {
        #region 角色控制
        if (playerControlActive)
        {   //移動
            if (isGrounded() && velocity.y < 0)
                velocity.y = 0f;

            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");

            if (v < 0) //如果倒退走 速度減半
            {
                float speedReduce = moveSpeed / 2;
                moveSpeed = speedReduce;
            }

            Vector3 move = transform.right * h + transform.forward * v;

            controller.Move(Vector3.ClampMagnitude(move, 1) * moveSpeed * Time.deltaTime);

            if (Input.GetButtonDown("Jump") && isGrounded())
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            //射擊部分
            fireTimer += Time.deltaTime;
            if (fireTimer >= fireRate)
            {
                canFire = true;
            }

            if (Input.GetMouseButtonDown(0) && canFire && handGunEquip && haveAmmo)
            {
                canFire = false;
                fireTimer = 0;
                muzzle.transform.Rotate(0, 0, Random.Range(0, 180));
                muzzle.Play();

                if (scope)
                    fireRageOffset = Vector3.zero;
                else
                    fireRageOffset = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0);

                //Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2) + fireRageOffset);
                Ray ray = new Ray(mCamera.position, mCamera.TransformDirection(Vector3.forward + fireRageOffset));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000f))
                {
                    GunFireUpdate();

                    GameObject effectClone = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(effectClone, 1f);

                    muzzle.GetComponentInParent<AudioSource>().Play();

                    gun.GetComponent<Animator>().SetTrigger("Fire");

                    if (!hit.transform.GetComponentInParent<ZombieAI>())
                    {
                        GameObject soundClone = Instantiate(impactSound, hit.point, Quaternion.LookRotation(hit.normal));
                        Destroy(soundClone, 1f);
                    }
                    if (hit.rigidbody != null)
                    {
                        ZombieAI z_ai = hit.transform.GetComponentInParent<ZombieAI>();

                        if (z_ai != null)
                        {
                            if (hit.collider.tag == "Head")
                                z_ai.TakeDamage(true, fireDamage);
                            else
                                z_ai.TakeDamage(false, fireDamage);

                            if (z_ai.health <= 0)
                                hit.rigidbody.AddForce(-hit.normal * hitForce);
                        }
                        else
                        {

                            hit.rigidbody.AddForce(-hit.normal * hitForce);
                        }
                    }
                }
            }

            if (Input.GetMouseButton(1))
            {
                scope = true;
                //ui.crosshair.SetActive(true);
                gun.GetComponent<Animator>().SetBool("Scope", true);
            }
            else
            {
                scope = false;
                //ui.crosshair.SetActive(false);
                gun.GetComponent<Animator>().SetBool("Scope", false);
            }
        }
        #endregion

        #region 血量控制
        ui.heartPulseText.GetComponent<Animator>().speed = data.heartPulse / 60;                //依照心律控制動畫速率

        if (data.hp >= data.maxHp)
            data.hp = data.maxHp;// hpmax = 100
        if (data.hp > 70 && data.hp <= 100)//血量介於 70~100
        {
            data.heartPulse = 70 + Random.Range(-3, 3);                                         //脈搏正負3
            ui.heartPulseText.color = Color.Lerp(ui.heartPulseText.color, Color.green, 0.05f);  //顯示綠色
            moveSpeed = 4f;                                                                     //移動速度正常
        }
        if (data.hp > 40 && data.hp <= 70)//血量介於 40~70
        {
            data.heartPulse = 100 + Random.Range(-3, 3);
            ui.heartPulseText.color = Color.Lerp(ui.heartPulseText.color, Color.yellow, 0.05f); //顯示黃色
            moveSpeed = 3f;                                                                     //移動減緩
        }
        if (data.hp > 0 && data.hp <= 40)//血量介於 0~40
        {
            data.heartPulse = 130 + Random.Range(-3, 3);
            ui.heartPulseText.color = Color.Lerp(ui.heartPulseText.color, Color.red, 0.05f);    //顯示紅色
            moveSpeed = 2f;                                                                     //走不動了
        }
        if (data.hp <= 0)
        {
            data.hp = 0;
            Dead();
            print("dead");
        }

        #endregion

        #region 動畫控制
        if (h != 0 || v != 0)
            anim.SetBool("Run", true);
        else
            anim.SetBool("Run", false);

        anim.SetFloat("Speed", moveSpeed / moveSpeedBase);
        #endregion
        if (PressTab())
            ui.InventoryTrigger();

        if (Input.GetKeyDown(KeyCode.R))
            GunReload();

        #region 彈藥顯示
        if (gun.name == "HandGunOnHand")
        {
            if (data.itemList.Find(ninemmAmmo) != null)
                ammoUnload = data.itemList.Find(ninemmAmmo).count - ammoLoaded;
            else
                ammoUnload = 0;
            ui.AmmoTextUpdate(ammoLoaded, ammoUnload);
        }
        #endregion

        #region 互動(按E)
        Ray interactionRay = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit interactionHitInfo;
        if (Physics.Raycast(interactionRay, out interactionHitInfo, 100,  ~LayerMask.GetMask("PlayerHead")))
        {
            ui.ItemNameTextUpdate(Camera.main.WorldToScreenPoint(interactionHitInfo.transform.position));

            #region 道具
            if (interactionHitInfo.collider.tag == "Item" && interactionHitInfo.collider.GetComponent<Item>().searchable())
            {
                ui.itemNameText.text = "Press E to pick '" + interactionHitInfo.collider.GetComponent<Item>().data.name + "'";

                if (PressE())
                {
                    Item pickedItem = interactionHitInfo.collider.GetComponent<Item>();
                    data.ItemAdd(pickedItem.data, pickedItem.count);
                    Destroy(interactionHitInfo.collider.gameObject);
                }
            }
            #endregion

            #region 互動式物件
            if (interactionHitInfo.collider.tag == "interactiveOBJ" && Vector3.Distance(interactionHitInfo.collider.transform.position, player.position) <= 2f)
            {
                // 推倒門
                if (interactionHitInfo.collider.name == "WindowDoor")
                {
                    ui.itemNameText.text = "Press E to open the " + interactionHitInfo.collider.name;

                    if (PressE())
                    {
                        interactionHitInfo.rigidbody.isKinematic = false;
                        interactionHitInfo.rigidbody.AddForce(-interactionHitInfo.normal * 1000);
                        interactionHitInfo.collider.tag = "Untagged";
                        Physics.IgnoreCollision(interactionHitInfo.collider, player.GetComponent<Collider>());
                    }
                }

                // 鐵櫃
                if (interactionHitInfo.collider.GetComponentInParent<Locker>())
                {
                    if (!interactionHitInfo.collider.GetComponentInParent<Animator>().GetBool("Open"))
                        ui.itemNameText.text = "Press E to open the Locker";
                    else
                        ui.itemNameText.text = "Press E to close the Locker";

                    if (PressE())
                    {
                        Locker locker = interactionHitInfo.collider.GetComponentInParent<Locker>();
                        locker.CallWindow();
                    }
                }

                // 閱讀
                if (interactionHitInfo.collider.GetComponent<TextContent>())
                {
                    ui.itemNameText.text = "Press E to read the paper";
                    
                    if (PressE())
                        interactionHitInfo.collider.GetComponent<TextContent>().CallWindow();
                }

                // 電子門禁
                if (interactionHitInfo.collider.GetComponent<PassMachine>())
                {
                    ui.itemNameText.text = "Press E to check";

                    if (PressE())
                        interactionHitInfo.collider.GetComponent<PassMachine>().Open();
                }

            }
            #endregion
        }
        #endregion
    }

    void Dead()
    {
        h = 0;
        v = 0;
        playerControlActive = false;
    }

    #region 彈藥計算
    int ammoLoadLimit;
    int ammoLoaded;
    int ammoUnload;


    void GunFireUpdate() //開槍時檢查彈夾內與總彈藥量
    {
        ammoLoaded -= 1;

        if (ammoLoaded < 1)
            haveAmmo = false;

        if (data.itemList.Find(ninemmAmmo).count > 1)
        {
            data.itemList.Find(ninemmAmmo).count -= 1;
        }
        else
        {
            data.itemList.Remove(data.itemList.Find(ninemmAmmo));
        }
    }

    void GunReload() //換彈夾
    {
        if (ammoUnload > 0 && data.itemList.Find(ninemmAmmo) != null)
        {
            haveAmmo = true;

            if (data.itemList.Find(ninemmAmmo).count >= ammoLoadLimit)
            {
                ammoLoaded = ammoLoadLimit;
                ammoUnload = data.itemList.Find(ninemmAmmo).count - ammoLoaded;
            }
            else
            {
                ammoLoaded = data.itemList.Find(ninemmAmmo).count;
                ammoUnload = 0;
            }
        }
    }

    public void SetAmmoInfo()
    {
        if (gun.name == "HandGunOnHand")
        {
            ammoLoadLimit = 12;
            //GunReload();
        }
    }
    #endregion
}
