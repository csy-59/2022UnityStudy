using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    //채력
    public int maxHealth;
    public int curHealth;

    //쫓고 있는지
    public bool isChase;

    //목표(캐릭터)
    public Transform target;

    //물리 관련
    Rigidbody rigid;
    BoxCollider boxCollider;
    //피격 효과
    Material mat;

    //유니티에서 제공하는 AI
    NavMeshAgent nav;
    //NavMesh: NavMeshAgent가 경로를 그리기 위한 바탕
    //NavMesh는 유니티의 Window > AI > Navigation에서 bake 탭에서 bake를 눌러야한다.
    //애니메이션
    Animator anim;

    private void Awake()
    {
        //초기화
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        //material은 MeshRenderer에서만 가져올 수 있음
        mat = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        //생성되고 2초 뒤에 쫓아가기 시작
        Invoke("ChaseStart", 2);
    }

    void ChaseStart()
    {
        //쫓아가기 시작
        isChase = true;
        //애니메이션 설정
        anim.SetBool("isWalk", true);
    }

    private void Update()
    {
        if (isChase) //쫓아가는 중에만
        {
            //목표를 따라가도록 설정
            nav.SetDestination(target.position);
        }
        
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        //수류탄 피격처리
        curHealth -= 100;
        //피격 받은 상대적 방향 구하기
        Vector3 reactVec = transform.position - explosionPos;
        //구한 방향을 기반으로 
        StartCoroutine(OnDamage(reactVec, true));
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee")
        {
            //근접 공격
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            //넉백에 관한 방향(공격 받은 방향)
            Vector3 reactVec = transform.position - other.transform.position;
            //피격 코루틴 시작
            StartCoroutine(OnDamage(reactVec, false));
        }
        else if(other.tag == "Bullet")
        {
            //원거리 공격
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            //넉백에 관한 방향(공격 받은 방향)
            Vector3 reactVec = transform.position - other.transform.position;
            //총알은 삭제해서 통과하지 못하도록
            Destroy(other.gameObject);
            //피격 코루틴 시작
            StartCoroutine(OnDamage(reactVec, false));
        }
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        //피격시 빨강색으로
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if(curHealth > 0)
        {
            //아직 죽지 않았다면 다시 원래 색으로
            mat.color = Color.white;
        } else
        {
            //죽으면 회색으로
            mat.color = Color.gray;
            //죽으면 더이상 피격 받지 않도록
            gameObject.layer = 12;
            //쫓기 금지
            isChase = false;
            //넉백 효과를 유지하기 위하여 NavAgent 비활성화
            nav.enabled = false;
            //애니메이션 효과
            anim.SetTrigger("doDie");


            if (isGrenade)
            {
                //방향 통일
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3; //위로도 넉백 주기

                //수류탄으로 죽으면 더 회전하는 이펙트를 주기 위해 freeze 수정
                rigid.freezeRotation = false;
                //넉백 주기
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);
            } else
            {
                //방향 통일
                reactVec = reactVec.normalized;
                reactVec += Vector3.up; //위로도 넉백 주기
                //넉백 주기
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
            }
            

            Destroy(gameObject, 4);
        }
    }

    void FreezeVelocity()
    {
        //캐릭터가 다른 rigidBody와 충돌했을 때 일어나는 회전력과 반동 0으로 초기화
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
        
    }

    private void FixedUpdate()
    {
        //FixedUpdate: 프레임 단위의 일정간격으로 호출되는 함수
        FreezeVelocity();
    }
}
