using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    //몬스터 타입
    public enum Type { A, B, C };
    public Type enemyType;

    //채력
    public int maxHealth;
    public int curHealth;

    //쫓고 있는지
    public bool isChase;

    //목표(캐릭터)
    public Transform target;

    //공격 범위 변수화
    public BoxCollider meleeArea;
    //원거리 몬스터(C타입) 미사일을 담을 오브젝트
    public GameObject bullet;
    //공격 중인지
    public bool isAttack;

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
        if (nav.enabled) //nav이 활성화 되어 있을 때만(목표를 찾을 수 있을 때만) 움직임
        {
            //목표를 따라가도록 설정
            nav.SetDestination(target.position);
            //쫓아가지 않을 때 멈춤
            nav.isStopped = !isChase;
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

    void Targeting()
    {
        //타겟팅 : lay 사용, 범위로 감지해야하기 때문에 SphereCastAll 사용
        float targetRadius = 0f;//lay 반지름
        float targetRange = 0f; //lay 범위

        //몬스터 타입 별로 타겟팅 lay 범위 지정해주기
        switch (enemyType)
        {
            case Type.A:
                targetRadius = 1.5f;
                targetRange = 3f;
                break;

            case Type.B:
                targetRadius = 1f;
                targetRange = 12f;
                break;

            case Type.C:
                //정확성을 위하여 반지름 작게
                targetRadius = 0.5f;
                targetRange = 25f;
                break;
        }

        //SphereCastAll(<lay 기준>, <lay 반지름>, <lay 방향>, <lay 범위>, <lay 감지 대상>)
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 
            targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

        //ray가 플레이어를 감지하고 이미 공격중이 아니라면
        if (rayHits.Length > 0 && !isAttack)
        {
            //공격!
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        //공격 시작!
        isChase = false;//추격 종료
        isAttack = true;//공격 시작
        anim.SetBool("isAttack", true);//애니메이션 설정

        //몬스터 유형별로
        switch (enemyType)
        {
            case Type.A:
                //애니메이션에서의 딜레이를 생각하여 후에 공격 활성화
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;//공격 범위 활성화(player에서 이와 부딪치면 반응함)

                //공격후 공격 범위 비활성화
                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;

                //공격 후 잠시 쉼
                yield return new WaitForSeconds(1f);
                break;

            case Type.B:
                //돌격 설정
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                meleeArea.enabled = true;

                //돌격해서 멈춤
                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero; //속도 멈추기
                meleeArea.enabled = false;

                //공격 후 잠시 쉼
                yield return new WaitForSeconds(2f);
                break;

            case Type.C:
                //원거리 공격
                yield return new WaitForSeconds(0.5f);
                //공격을 위한 미사일 객체 생성
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                //미사일의 rigidbody를 가져와서 공격
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;

                //공격 후 잠시 쉼
                yield return new WaitForSeconds(2f);
                break;
        }

        //공격 후 처리
        isChase = true;//추격 시작
        isAttack = false;//공격 종료
        anim.SetBool("isAttack", false);//애니메이션 설정
    }

    private void FixedUpdate()
    {
        //타겟이 공격 범위 내에 들어오면 타겟팅을 함
        Targeting();

        //FixedUpdate: 프레임 단위의 일정간격으로 호출되는 함수
        FreezeVelocity();
    }
}
