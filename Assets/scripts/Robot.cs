using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Robot : MonoBehaviour
{
// 테스트용 코드 시작점 01
    [SerializeField] 
    private Transform upperBody;
    [SerializeField, Range(-180f, 180f)]
    private float targetAngle;

    [SerializeField] private float rotationSpeed = 180f;


    [SerializeField] private Vector3 localAxis = Vector3.up;

    private Quaternion initialRotation;
    private float currentAngle;
// 테스트 코드 마무리줄 01


    [SerializeField]
    private Slider hpBar;


    private Animator anim;

    public float maxHealth;
    private float curHealth;

    public float damage;

    private bool isFire;
    private bool isMove;
    private bool isDead = false;

    NavMeshAgent nav;

    private void Awake()//테스트 코드 시작 02
    {
        if (upperBody == null)
        {
            enabled = false;
            return;
        }

        initialRotation = upperBody.localRotation;
    }
    private void LateUpdate()
    {
        if (localAxis.sqrMagnitude < 0.0001f)
            return;

        currentAngle = Mathf.MoveTowardsAngle(
            currentAngle,
            targetAngle,
            rotationSpeed * Time.deltaTime
        );

        upperBody.localRotation =
            initialRotation *
            Quaternion.AngleAxis(currentAngle, localAxis.normalized);
    }

    public void SetAngle(float angle)
    {
        targetAngle = angle;
    }
    //여기까지 테스트용 코드 02

    void Start()
    {
        curHealth = maxHealth;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            if (isFire)
            {
                anim.SetBool("isAttack", true);
            }
            else
            {
                anim.SetBool("isAttack", false);
            }

            if (isMove)
            {
                anim.SetBool("isRun", true);
            }
            else
            {
                anim.SetBool("isRun", false);
            }
        }
        if (isDead)
        {
            anim.SetBool("isDie", true);
        }
    }
    void UpperBody_Rotation()
    {

    }
    void Dead()
    {
        if (curHealth <= 0)
        {
            isDead = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Ammo"))
        {
            curHealth -= damage;
        }
    }
}
