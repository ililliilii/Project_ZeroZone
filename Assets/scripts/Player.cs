using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class Player : MonoBehaviour
{
    private Vector3 lastPosition;
    public float Run_Reset;
    public GameObject[] Character;
    public bool NOHA;
    public bool ENHA;
    public bool LUCY;
    [Header("인벤토리")]
    public Inventory inventory;
    public GameObject Inventory_Volume;
    private bool CheckInventory_Full;
    public Image[] Gameover;
    public string[] SetKey;
    [SerializeField]
    private Slider _hpbar, HPBar;
    public GameObject Corpse;
    public Text Hptext;
    // Start is called before the first frame update
    public float Die_time = 1f;
    public float Speed, StartSpeed;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;
    public int hasGrenades;
    public GameObject grenadObj;
    public Camera followCamera;
    Rigidbody rigid;
    public string[] MoveKey;
    public int ammo;
    [Tooltip("Enable to consume reserve ammo when reloading. Disabled preserves the original reload behavior.")]
    public bool useReserveAmmo = false;
    public int coin;
    public int health;
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;
    float hAxis;
    float vAxis;
    bool wDown;
    bool jDown;
    public bool fDown;
    bool gDown;
    bool rDown;
    bool iDown;
    bool sDown1;
    bool sDown2;
    bool sDown3;
    bool sDown4;
    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isReload;
    bool isFireReady = true;
    bool isBorder;
    bool isDamage;
    bool isDie = false;
    public bool Dodge_ON;
    public bool Stop;
    Vector3 moveVec;
    Vector3 dodgeVec;
    Animator anim;
    MeshRenderer[] meshs;
    GameObject nearObject;
    Weapon equipWeapon;
    Weapon reloadWeapon;
    int equipWeaponIndex = -1;
    float fireDelay;
    public KeyCode[] Key_Setting;
    void Awake()
    {
        NOHA = GameData.Instance.Pick_NOHA;
        ENHA = GameData.Instance.Pick_ENHA;
        LUCY = GameData.Instance.Pick_LUCY;
        if (NOHA)
        {
            Character[0].SetActive(true);
        }

        if (ENHA)
        {
            Character[1].SetActive(true);
        }

        if (LUCY)
        {
            Character[2].SetActive(true);
        }

        lastPosition = transform.position;
        _hpbar.maxValue = maxHealth;
        HPBar.maxValue = maxHealth;
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        meshs = GetComponentsInChildren<MeshRenderer>();
    }

    void Start()
    {
        StartSpeed = Speed;
        GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        if (Key_Setting == null || Key_Setting.Length < 4)
        {
            Key_Setting = new KeyCode[4];
        }

        if (GameData.Instance.data.GameManger_KeySet != null && GameData.Instance.data.GameManger_KeySet.Length >= 4)
        {
            Key_Setting[0] = GameData.Instance.data.GameManger_KeySet[0];
            Key_Setting[1] = GameData.Instance.data.GameManger_KeySet[1];
            Key_Setting[2] = GameData.Instance.data.GameManger_KeySet[2];
            Key_Setting[3] = GameData.Instance.data.GameManger_KeySet[3];
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 horizontalVelocity = new Vector3(rigid.velocity.x, 0, rigid.velocity.z);
        CheckInventory_Full = Inventory_Volume.GetComponent<Inventory>().Inventory_Full;
        if (Key_Setting == null || Key_Setting.Length < 4)
        {
            Key_Setting = new KeyCode[4];
        }

        if (GameData.Instance.data.GameManger_KeySet != null && GameData.Instance.data.GameManger_KeySet.Length >= 4)
        {
            Key_Setting[0] = GameData.Instance.data.GameManger_KeySet[0];
            Key_Setting[1] = GameData.Instance.data.GameManger_KeySet[1];
            Key_Setting[2] = GameData.Instance.data.GameManger_KeySet[2];
            Key_Setting[3] = GameData.Instance.data.GameManger_KeySet[3];
        }

        if (!Stop)
        {
            Run_Reset += Time.deltaTime;
            if (Run_Reset >= 3)
            {
                Run_Reset = 3;
            }
        }

        if (Stop)
        {
            Run_Reset = 0;
            Dodge_ON = false;
        }

        if (isDie == true)
        {
            return;
        }
        else if (isDie == false)
        {
            if (horizontalVelocity.magnitude < 0.1f)
            {
                Stop = true;
                Run_Reset = 0;
            }
            else
            {
                Stop = false;
            }

            Run();
            GetInput();
            Move();
            Turn();
            Jump();
            Grenade();
            Raload();
            Attack();
            Dodge();
            Swap();
            Interation();
            Die();
        }

        _hpbar.value = health;
        HPBar.value = health;
        Hptext.text = (health.ToString() + " / " + maxHealth.ToString());
    }

    void GetInput()
    {
        hAxis = 0;
        vAxis = 0;
        fDown = Input.GetButton("Fire1");
        if (fDown)
        {
            hAxis = 0;
            vAxis = 0;
        }

        if (!fDown)
        {
            if (Input.GetKey(Key_Setting[0]))
                vAxis += 1;
            if (Input.GetKey(Key_Setting[1]))
                vAxis -= 1;
            if (Input.GetKey(Key_Setting[2]))
                hAxis -= 1;
            if (Input.GetKey(Key_Setting[3]))
                hAxis += 1;
        }

        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        fDown = Input.GetButton("Fire1");
        gDown = Input.GetButtonDown("Fire2");
        rDown = Input.GetButtonDown("Reload");
        iDown = Input.GetButtonDown("Interation");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
        sDown4 = Input.GetButtonDown("Swap4");
    }

    void Move()
    {
        if (!fDown)
        {
            Vector3 inputVec = new Vector3(hAxis, 0, vAxis).normalized;
            if (!isDodge)
            {
                moveVec = inputVec;
            }

            if (isDodge)
                moveVec = dodgeVec;
            if (isSwap || isReload || !isFireReady)
                moveVec = Vector3.zero;
            if (moveVec != Vector3.zero && !isBorder)
            {
                rigid.velocity = new Vector3(moveVec.x * Speed, rigid.velocity.y, moveVec.z * Speed);
            }
            else
            {
                rigid.velocity = new Vector3(0f, rigid.velocity.y, 0f);
            }

            anim.SetBool("isRun", inputVec != Vector3.zero && moveVec != Vector3.zero);
            anim.SetBool("isWalk", wDown);
        }

        if (fDown)
        {
            moveVec = Vector3.zero;
            rigid.velocity = new Vector3(0f, rigid.velocity.y, 0f);
            anim.SetBool("isRun", false);
        }
    }

    void Die()
    {
        if (health <= 0 && !isDie)
        {
            isDie = true;
            CancelInvoke();
            if (reloadWeapon != null)
                reloadWeapon.CancelReload();
            if (equipWeapon != null)
                equipWeapon.StopUse();
            StartCoroutine(Dead());
        }
    }

    IEnumerator Dead()
    {
        if (isDie == true)
        {
            gameObject.tag = "Untagged";
            rigid.isKinematic = true;
            anim.SetBool("isDie", true);
            Coroutine fade = StartCoroutine(BlackOut());
            yield return new WaitForSeconds(Die_time);
            if (Corpse != null)
                Instantiate(Corpse, transform.position, Quaternion.identity);
            foreach (Renderer mesh in GetComponentsInChildren<Renderer>())
                mesh.enabled = false;
            foreach (Collider area in GetComponentsInChildren<Collider>())
                area.enabled = false;
            yield return fade;
            Destroy(gameObject);
        }
    }

    void Turn()
    {
        if (moveVec != Vector3.zero)
            transform.LookAt(transform.position + moveVec);
        if (fDown)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }
    }

    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap)
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Grenade()
    {
        if (hasGrenades <= 0)
            return;
        if (gDown && !isReload && !isSwap)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 2;
                GameObject instantGrenade = Instantiate(grenadObj, transform.position, transform.rotation);
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);
                hasGrenades--;
                if (hasGrenades < grenades.Length && grenades[hasGrenades] != null)
                    grenades[hasGrenades].SetActive(false);
            }
        }
    }

    void Attack()
    {
        if (equipWeapon == null)
            return;
        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;
        if (fDown && isFireReady && !isDodge && !isSwap && !isReload)
        {
            if (!equipWeapon.Use())
                return;
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : (equipWeapon.type == Weapon.Type.Range ? "doShot" : "doRifle"));
            fireDelay = 0;
        }
    }

    void Raload()
    {
        if (equipWeapon == null)
            return;
        if (equipWeapon.type == Weapon.Type.Melee)
            return;
        if (AvailableReloadAmmo(equipWeapon.maxAmmo) <= 0 || isReload)
            return;
        if ((rDown || equipWeapon.curAmmo <= 0) && !isJump && !isDodge && !isSwap)
        {
            if (!equipWeapon.TryStartReload())
                return;
            reloadWeapon = equipWeapon;
            anim.SetTrigger("doReload");
            isReload = true;
            Invoke("ReloadOut", 1.5f);
        }
    }

    void ReloadOut()
    {
        if (reloadWeapon != null && !isDie)
        {
            int loaded = reloadWeapon.CompleteReload(AvailableReloadAmmo(reloadWeapon.maxAmmo));
            if (useReserveAmmo)
                ammo -= loaded;
        }

        reloadWeapon = null;
        isReload = false;
    }

    int AvailableReloadAmmo(int magazineCapacity)
    {
        return Mathf.Max(0, useReserveAmmo ? ammo : magazineCapacity);
    }

    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap && Dodge_ON && !Stop)
        {
            Run_Reset = 0;
            Dodge_ON = false;
            dodgeVec = moveVec;
            Speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;
            Invoke("DodgeOut", 0.5f);
        }
    }

    void DodgeOut()
    {
        Speed *= 0.5f;
        isDodge = false;
    }

    void Swap()
    {
        if (isReload || isSwap)
            return;
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;
        if (sDown4 && (!hasWeapons[3] || equipWeaponIndex == 3))
            return;
        int weaponIndex = -1;
        if (sDown1)
            weaponIndex = 0;
        if (sDown2)
            weaponIndex = 1;
        if (sDown3)
            weaponIndex = 2;
        if (sDown4)
            weaponIndex = 3;
        if ((sDown1 || sDown2 || sDown3 || sDown4) && !isJump && !isDodge)
        {
            if (equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);
            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);
            anim.SetTrigger("doSwap");
            isSwap = true;
            Invoke("SwapOut", 0.4f);
        }
    }

    void SwapOut()
    {
        isSwap = false;
    }

    void Interation()
    {
        if (iDown && nearObject != null && !isJump && !isDodge && !CheckInventory_Full)
        {
            Object_Item targetItem = nearObject.GetComponent<Object_Item>();
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;
                Item_Information itemData = targetItem.ClickItem();
                if (inventory != null)
                {
                    inventory.AddItem(itemData);
                }

                Destroy(nearObject);
                nearObject = null;
            }
        }
    }

    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }

    void StopToWall()
    {
        Vector3 rayStartOrigin = transform.position + Vector3.up * 0.5f;
        float rayLength = 0.7f;
        Debug.DrawRay(rayStartOrigin, moveVec * rayLength, Color.green);
        isBorder = Physics.Raycast(rayStartOrigin, moveVec, rayLength, LayerMask.GetMask("Wall"));
    }

    void FixedUpdate()
    {
        if (isDie)
            return;
        FreezeRotation();
        StopToWall();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            rigid.constraints &= ~RigidbodyConstraints.FreezePositionY;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDie)
            return;
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Grenade:
                    int capacity = Mathf.Max(0, Mathf.Min(maxHasGrenades, grenades.Length));
                    if (hasGrenades >= capacity || item.value <= 0)
                        return;
                    hasGrenades = Mathf.Clamp(hasGrenades + item.value, 0, capacity);
                    for (int i = 0; i < grenades.Length; i++)
                        if (grenades[i] != null)
                            grenades[i].SetActive(i < hasGrenades);
                    break;
            }

            Destroy(other.gameObject);
        }

        if (other.tag == "Melee")
        {
            Weapon weapan = other.GetComponent<Weapon>();
            health -= weapan.damage;
            StartCoroutine(OnDamage());
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            health -= bullet.damge;
            Destroy(other.gameObject);
            StartCoroutine(OnDamage());
        }
    }

    IEnumerator OnDamage()
    {
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.red;
        }

        yield return new WaitForSeconds(1f);
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
    }

    void Run()
    {
        if (!isDodge && !Dodge_ON && !Stop && Run_Reset == 3)
        {
            Dodge_ON = true;
        }
    }

    IEnumerator BlackOut()
    {
        if (Gameover == null || Gameover.Length < 3 || Gameover[0] == null || Gameover[1] == null || Gameover[2] == null)
            yield break;
        Gameover[0].gameObject.SetActive(true);
        Gameover[1].gameObject.SetActive(true);
        Gameover[2].gameObject.SetActive(true);
        Color color = Gameover[0].color;
        Color color2 = Gameover[1].color;
        Color color3 = Gameover[2].color;
        while (color.a < 0.9f)
        {
            color.a += Time.deltaTime / 2f;
            Gameover[0].color = color;
            yield return null;
        }

        while (color2.a < 1.0f || color3.a < 1.0f)
        {
            color2.a += Time.deltaTime / 3f;
            color3.a += Time.deltaTime / 3f;
            Gameover[1].color = color2;
            Gameover[2].color = color3;
            yield return null;
        }
    }
}
