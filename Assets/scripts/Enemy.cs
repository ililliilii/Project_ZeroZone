using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.XR;

public class Enemy : MonoBehaviour
{
    public int ammo;
    public int maxAmmo;
    public int DieTime = 5;
    public bool ishammer;
    public bool ispistol;
    public bool isrifle;
    public bool isgrenade;
    private bool Loading;
    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isReload;
    bool isFireReady = true;
    bool isBorder;
    bool isDamage;
    bool isDie = false;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    GameObject nearObject;
    Weapon equipWeapon;
    int equipWeaponIndex = -1;
    float fireDelay;
    [Header("Targeting Settings")]
    public float targetRadius = 1.5f;
    public float targetRange = 3f;
    public float detectionRange = 10f;
    public float wanderRadius = 5f;
    [SerializeField]
    private Slider _Hpbar;
    public GameObject Corpse;
    public int maxHealth;
    public int curHealth;
    public Transform target;
    public BoxCollider meleeArea;
    public bool isChase;
    public bool isAttack;
    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;
    MeshRenderer[] meshs;
    NavMeshAgent nav;
    Animator anim;
    void Awake()
    {
        curHealth = maxHealth;
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        StartCoroutine(LoadingTime());
    }

    void Start()
    {
        _Hpbar.maxValue = maxHealth;
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isRun", true);
    }

    void Update()
    {
        _Hpbar.value = curHealth;
        if (!isDie && !Loading)
        {
            SearchTarget();
            Swap();
            AttackRange();
            Attack();
            if (target == null)
            {
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null)
                    target = player.transform;
            }

            if (nav.enabled && nav.isOnNavMesh)
            {
                CheckTargetDistance();
                nav.isStopped = !isChase;
            //nav.isStopped = true;
            // nav.velocity = Vector3.zero;
            }

            if (isAttack && nav.enabled && nav.isOnNavMesh)
            {
                RotateToTarget();
                nav.isStopped = true;
                nav.velocity = Vector3.zero;
            }

            if (!isAttack && nav.enabled && nav.isOnNavMesh)
            {
                nav.isStopped = false;
            //nav.velocity = Vector3.zero;
            }
        }

        if (isDie)
        {
            rigid.isKinematic = true;
            anim.SetBool("isDie", true);
        }
    }

    void CheckTargetDistance()
    {
        if (target != null && target != transform)
        {
            float dist = Vector3.Distance(transform.position, target.position);
            if (dist <= detectionRange)
            {
                nav.SetDestination(target.position);
            }
            else
            {
                if (!nav.pathPending && nav.remainingDistance <= nav.stoppingDistance)
                {
                    Wander();
                }
            }
        }
        else
        {
            if (!nav.pathPending && nav.remainingDistance <= nav.stoppingDistance)
            {
                Wander();
            }
        }
    }

    void Wander()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1))
        {
            nav.SetDestination(hit.position);
        }
    }

    void FreezeVelocity()
    {
        if (isChase && !isAttack)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    void Targerting()
    {
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));
        if (!isAttack && isChase)
        {
            foreach (RaycastHit hit in rayHits)
            {
                if (hit.collider.gameObject.tag == "Player" && hit.collider.gameObject != gameObject)
                {
                    StartCoroutine(AttackBool());
                    break;
                }
            }
        }
    }

    IEnumerator AttackBool()
    {
        isChase = false;
        isAttack = true;
        yield return new WaitForSeconds(1f);
        meleeArea.enabled = true;
        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;
        isChase = true;
        isAttack = false;
    }

    void FixedUpdate()
    {
        if (isDie || Loading)
            return;
        Targerting();
        FreezeVelocity();
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDie)
            return;
        if (other.tag == "Melee")
        {
            Weapon weapan = other.GetComponent<Weapon>();
            if (weapan != null)
                TakeDamage(weapan.damage);
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet == null)
                return;
            TakeDamage(bullet.damge);
            Destroy(other.gameObject);
        }
    }

    public void HitByGreade(Vector3 explosionpos)
    {
        TakeDamage(100);
    }

    void TakeDamage(int damage)
    {
        if (isDie)
            return;
        curHealth -= damage;
        if (curHealth <= 0)
        {
            isDie = true;
            isChase = false;
            isAttack = false;
            StopAllCoroutines();
            CancelInvoke();
            if (equipWeapon != null)
                equipWeapon.StopUse();
            if (meleeArea != null)
                meleeArea.enabled = false;
            nav.enabled = false;
            rigid.isKinematic = true;
            anim.SetBool("isDie", true);
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray;
            gameObject.layer = 14;
            if (Corpse != null)
                Instantiate(Corpse, transform.position, Quaternion.identity);
            Destroy(gameObject, DieTime);
            return;
        }

        StopCoroutine(nameof(OnDamage));
        StartCoroutine(nameof(OnDamage));
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * targetRange, targetRadius);
    }

    void RotateToTarget()
    {
        if (target == null)
            return;
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    void Swap()
    {
        if (isReload || isSwap)
            return;
        /*if (ishammer && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;*/
        if (ispistol && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (isrifle && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        int weaponIndex = -1;
        //if (ishammer) weaponIndex = 0;
        if (ispistol)
            weaponIndex = 1;
        if (isrifle)
            weaponIndex = 0;
        if ((ispistol || isrifle) && !isJump && !isDodge)
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

    void Attack()
    {
        if (equipWeapon == null)
            return;
        if (equipWeapon.type != Weapon.Type.Melee && equipWeapon.curAmmo <= 0 && !isReload)
            StartCoroutine(Reload());
        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;
        if (isAttack && isFireReady && !isDodge && !isSwap && !isReload)
        {
            if (!equipWeapon.Use())
                return;
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : (equipWeapon.type == Weapon.Type.Rifle ? "doRifle" : "doShot"));
            fireDelay = 0;
        }
    }

    /*void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = reAmmo;
        ammo -= reAmmo;
        isReload = false;
    }*/
    IEnumerator Reload()
    {
        Weapon weapon = equipWeapon;
        if (weapon == null || !weapon.TryStartReload())
            yield break;
        isReload = true;
        yield return new WaitForSeconds(1.5f);
        // Preserve the enemy's existing unlimited reserve ammunition.
        if (weapon != null && !isDie)
            weapon.CompleteReload(weapon.maxAmmo);
        isReload = false;
    }

    void SwapOut()
    {
        isSwap = false;
    }

    void AttackRange()
    {
        if (equipWeapon == null)
            return;
        if (equipWeapon.type == Weapon.Type.Melee)
        {
            targetRadius = 3;
        }

        if (equipWeapon.type == Weapon.Type.Range || equipWeapon.type == Weapon.Type.Rifle)
        {
            targetRadius = 23;
        }
    }

    void SearchTarget()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, detectionRange, LayerMask.GetMask("Player"));
        Transform nearestTarget = null;
        float minDistance = Mathf.Infinity;
        foreach (Collider col in cols)
        {
            if (col.gameObject == gameObject)
                continue;
            float distance = Vector3.Distance(transform.position, col.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTarget = col.transform;
            }
        }

        if (nearestTarget != null)
        {
            target = nearestTarget;
        }
        else
        {
            target = null;
        }
    }

    IEnumerator LoadingTime()
    {
        Loading = true;
        yield return new WaitForSeconds(5.0f);
        Loading = false;
        if (!Loading)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                RotateToTarget();
                target = player.transform;
            }

            ChaseStart();
        }
    }
}
