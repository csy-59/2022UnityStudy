using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    //미사일 관련
    public GameObject missile; //미사일 프리펩을 받을 것
    public Transform missilePortA; //미사일이 나갈 포트 1
    public Transform missilePortB; //미사일이 나갈 포트 2

    //돌은 Enemy의 bullet 사용
    //보스가 사용자의 움직임을 예측하게 하기 위한 벡터
    Vector3 lookVec;
    Vector3 tauntVec; //찍기 공격 시 찍을 공간

    public bool isLook; //플레이어를 바라보고 있는지

    private void Awake()
    {
        /*상속 시 Awake는 자식 스크립트에 있는 것으로만 사용됨
         즉, 자식스크립트에 모든 것을 재정의 해줘야함*/
        //초기화
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        //material은 MeshRenderer에서만 가져올 수 있음
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        //쫓아가지 않기
        nav.isStopped = true;

        StartCoroutine(Think());
    }

    private void Update()
    {
        //죽으며
        if (isDead)
        {
            //모든 행동 멈춤
            StopAllCoroutines();
            return;
        }

        if (isLook)
        {
            //플레이어 입력값을 감지하여 예측 벡터 생성
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5f; //예측 백터 생성하기
            //예측 백터를 사용하여 플레이어 예측 방향으로 바라보기
            transform.LookAt(target.position + lookVec); //target은 Enemy의 target
        } else
        {
            nav.SetDestination(tauntVec);
        }
    }

    IEnumerator Think()
    {
        //행동 패컨 결정해주기 
        //약간 시간을 주고 시작
        yield return new WaitForSeconds(0.1f);

        //어떤 패턴을 사용할지 가져오는 랜덤 변수(0에서 4까지)
        int ranAction = Random.Range(0, 5);
        switch (ranAction)
        {
            case 0:
            case 1:
                //미사일 공격
                StartCoroutine(MissileShot());
                break;
            case 2:
            case 3:
                //돌 공격
                StartCoroutine(RockShot());
                break;
            case 4:
                //찍기 공격
                StartCoroutine(Taunt());
                break;
        }
    }

    IEnumerator MissileShot()
    {
        //미사일 발사(하나는 0.2초 뒤에, 하나는 0.5초후에)
        anim.SetTrigger("doShot"); //애니메이션

        yield return new WaitForSeconds(0.2f); //에니메이션을 맞추기 위해
        //미사일 가져옴
        GameObject instantMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation);
        //미사일의 스크립트를 통해 목표물 설정
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
        bossMissileA.target = target;

        yield return new WaitForSeconds(0.3f); //에니메이션을 맞추기 위해
        //미사일 가져옴
        GameObject instantMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation);
        //미사일의 스크립트를 통해 목표물 설정
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        bossMissileB.target = target;

        yield return new WaitForSeconds(2f); //에니메이션을 맞추기 위해

        StartCoroutine(Think());//다시 추격 시작
    }

    IEnumerator RockShot()
    {
        //돌을 굴리기 위한 기모으기를 할때 바라보기 중지
        isLook = false;
        anim.SetTrigger("doBigShot");//애니메이션
        //돌 생성(생성 후에는 돌 스크립트가 함)
        Instantiate(bullet, transform.position, transform.rotation);
        yield return new WaitForSeconds(3f);//에니메이션을 맞추기 위해
        isLook = true;
        StartCoroutine(Think());//다시 추격 시작
    }

    IEnumerator Taunt()
    {
        //내려찍을 위치 설정
        tauntVec = target.position + lookVec;

        isLook = false;
        nav.isStopped = false; //쫓아가기 시작
        //찍는 동안 박스 콜라이더가 플레이어를 밀지 않도록
        boxCollider.enabled = false;
        anim.SetTrigger("doTaunt");//애니메이션

        yield return new WaitForSeconds(1.5f); // 공격 범위 활성화를 위한 것
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f); // 내려 찍은 후
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);//에니메이션을 맞추기 위해

        //초기화
        isLook = true;
        nav.isStopped = true; //쫓아가기 금지
        boxCollider.enabled = true;
        StartCoroutine(Think());//다시 추격 시작
    }
}

