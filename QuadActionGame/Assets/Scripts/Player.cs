using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    //무기 관련
    public GameObject[] weapons; //무기 종류
    public bool[] hasWeapons; //갖고 있는지
    public GameObject[] grenades; //수류탄
    public int hasGrendes; //수류탄 갯수
    public GameObject grenadeObject; //수류탄 프리펩

    //카메라 변수
    public Camera followCamera;

    //아이템 관련
    public int ammo; //총알
    public int coin; //코인
    public int health; //채력
    public int score; //점수

    public int maxAmmo; //최대 총알
    public int maxCoin; //최대 코인
    public int maxHealth; //최대 채력
    public int maxHasGrendes; //최대 수류탄

    float hAxis;
    float vAxis;
    bool wDown;
    bool jDown;
    //공격하기
    bool fDown;
    //수류탄 던지기
    bool gDown;
    //장전
    bool rDown;
    //무기 바꾸기
    bool sDown1; //1번
    bool sDown2; //2번
    bool sDown3; //3번
    //아이템 상호작용
    bool iDown;

    //점프 중인지
    bool isJump;
    //회피 중인지
    bool isDodge;
    //무기 변경 중인지
    bool isSwap;
    //공격 가능한지
    bool isFireReady = true;
    //재장전 중인지
    bool isReload;
    //벽인지
    bool isBorder;
    //공격받는 중인지
    bool isDamage;
    //쇼핑 중인지
    bool isShop;

    //벡터
    Vector3 moveVec; //이동시 벡터
    Vector3 dodgeVec; //회피시 백터

    //물리 효과를 위한 변수
    Rigidbody rigid;
    //애니메이션 관련
    Animator anim;
    //플레이어의 Mesh를 받을 배열
    MeshRenderer[] meshs;

    //아이템 감지
    GameObject nearObject;
    //장착중인 무기
    public Weapon equipWeapon;
    int equipWeaponIndex = -1;
    //공격 딜레이
    float fireDelay; 

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        meshs = GetComponentsInChildren<MeshRenderer>();

        Debug.Log(PlayerPrefs.GetInt("MaxScore"));
        //유니티에서 제공하는 간단한 저장 기능
        //PlayerPrefs.SetInt("MaxScore", 112500);
    }

    // Update is called once per frame
    void Update()
    {
        //값 받아오기
        GetInput();
        //이동
        Move();
        //회전
        Turn();
        //점프
        Jump();
        //수류탄
        Grenade();
        //공격
        Attack();
        //장전
        Reload();
        //회피
        Dodge();
        //상호작용
        Interation();
        //무기 바꾸기
        Swap();
    }

    void GetInput()
    {
        //이동관련 값 받아오기
        hAxis = Input.GetAxisRaw("Horizontal"); //좌우
        vAxis = Input.GetAxisRaw("Vertical"); //앞뒤
        wDown = Input.GetButton("Walk"); //걷기
        jDown = Input.GetButtonDown("Jump"); //점프

        iDown = Input.GetButtonDown("Interation"); //상호작용(아이템)
        
        //무기 변경 관련 값 받아오기
        sDown1 = Input.GetButtonDown("Swap1"); //무기 바꾸기:1번
        sDown2 = Input.GetButtonDown("Swap2"); //무기 바꾸기:2번
        sDown3 = Input.GetButtonDown("Swap3"); //무기 바꾸기:3번

        //공격 버튼
        fDown = Input.GetButtonDown("Fire1");
        //수류탄 버튼
        gDown = Input.GetButtonDown("Fire2");
        //재장전
        rDown = Input.GetButtonDown("Reload");

    }

    void Move()
    {
        //대각선으로 이동시 약간 길이가 긴 것을 1로 바꿔주는 것: normalized
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        //회피를 하고 있을 경우 회피 방향으로만 이동할 수 있도록
        if (isDodge)
            moveVec = dodgeVec;


        //무기 변경 중이라면, 공격중이 아니라면 이동하지 않음
        if (isSwap || !isFireReady || isReload)
            moveVec = Vector3.zero;

        //경계에 있을 경우 이동하지 않음
        if (!isBorder)
        {
            //이동관련 걷기 할때는 속도 늦추기(0.3배로)
            transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;
        }




        //애니메이션 관련
        anim.SetBool("isRun", moveVec != Vector3.zero); //달리기(이동 값이 0이 아닐 때)
        anim.SetBool("isWalk", wDown); //걷기(left shift가 눌렸을 때)
    }

    void Turn()
    {
        //키보드에 의해 회전하기: 바라보는 방향으로 몸 회전
        transform.LookAt(transform.position + moveVec);

        //마우스에 의해 회전
        if (fDown)//마우스가 클릭되었을 때만 바라보게 함
        {
            //카메라에서 마우스가 눌린 방향으로 레이를 쏨
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            //out: 레이에서 맞은 대상이 있다면 rayHit로 전송
            if(Physics.Raycast(ray, out rayHit, 100))
            {
                //마우스 클릭 상대 위치
                Vector3 nextVec = rayHit.point - transform.position;
                //높이는 무시하도록
                nextVec.y = 0;
                //해당 방향으로 돌림
                transform.LookAt(transform.position + nextVec);
            }
        }
        
    }
    void Jump()
    {
        //점프키가 눌리고 점프중/회피중이 아니고, 움직이지 않을 경우 점프
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap && !isShop)
        {
            //벡터의 위쪽 방향으로 15만큼의 힘을 순간적(Impulse)으로 줌
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            //애니메이션 효과
            anim.SetBool("isJump", true);//점프중 true로(land 애니메이션을 위해)
            anim.SetTrigger("doJump");//점프 애니메이션 트리거로 불러오기
            isJump = true;
        }
    }

    void Attack()
    {
        //공격하기
        //장착된 무기가 없을 경우
        if (equipWeapon == null)
            return;
 
        //공격 대기 시간추가
        fireDelay += Time.deltaTime;
        //공격 가능 여부: 공속기 대기 시간보다 클 경우
        isFireReady = equipWeapon.rate < fireDelay; 
 
        //공격키를 누르고, 공격 가능하고, 회피나 무기를 교체중이 아니라면 공격함
        if(fDown && isFireReady && !isDodge && !isSwap && !isShop)
        {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");//애니메이션 처리
            fireDelay = 0;//공격 대기 시간 초기화
        }
    }

    void Grenade()
    {
        //수류탄이 없다면 반환
        if (hasGrendes == 0)
            return;

        //수류탄 버튼이 눌리고 재장전과 무기변경 중이 아니라면
        if(gDown && !isReload && !isSwap && !isShop)
        {
            //마우스가 클릭한 곳에 수류탄 던지기
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            //out: 레이에서 맞은 대상이 있다면 rayHit로 전송
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                //마우스 클릭 상대 위치
                Vector3 nextVec = rayHit.point - transform.position;
                //약간 높이가 있게 던짐
                nextVec.y = 15;

                //프리펩된 수류탄 생성
                GameObject instantGrenade = Instantiate(grenadeObject, transform.position, transform.rotation);
                //수류탄 던지기
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                //수류탄에 회전 주기
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                hasGrendes--;
                grenades[hasGrendes].SetActive(false);
            }
        }
    }

    void Reload()
    {
        //재장전
        //무기가 없으면 나감
        if (equipWeapon == null)
            return;
        //무기가 근거리 무기면 나감
        if (equipWeapon.type == Weapon.Type.Melee)
            return;
        //남은 총알이 없으면 나감
        if (ammo == 0)
            return;

        //Debug.Log(rDown + ": 재장전" + !isJump + ": 재장전" + !isDodge + ":회피중" + !isSwap + "무기 전환중"+ !isFireReady + "무기 전환중");
        //재장전 키가 눌리고, 점프, 회피, 무기 변경 중이 아니고, 격발중이 아닐 경우
        if (rDown && !isJump && !isDodge && !isSwap && isFireReady && !isShop)
        {
            anim.SetTrigger("doReload"); //애니메이션 설정
            isReload = true;//재장전 중 표시
 
            //재장전 중 표시 해제(재장전 시간)
            Invoke("ReloadOut", 3f);
        }
    }
    void ReloadOut()
    {
        //남아있는 탄창 갯수가 max 보다 많으면 max개, 아니면 남은 탄창 수 만큼 넣어줌
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = reAmmo;
        //플래이어의 탄창 수 감소
        ammo -= reAmmo;
        isReload = false;
    }

    void Dodge()
    {
        //점프키가 눌리고 점프중이 아니고, 움직이는 중일 때 회피
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap && !isShop)
        {
            dodgeVec = moveVec;
            //회피중일 때만 이동속도 2배
            speed *= 2;
            anim.SetTrigger("doDodge");//회피 애니메이션 트리거로 불러오기
            isDodge = true;

            //시간차를 두고 회피 종료
            Invoke("DodgeOut", 0.6f);
        }
    }
    void DodgeOut()
    {
        //회피 종료
        speed /= 2;
        isDodge = false;
    }   
    
    void Swap()
    {
        //무기 변경
        //변경하려는 무기가 없거나 이미 장착 중일 경우 스왑하지 않음
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;

        //변경한 무기 번호 받아오기
        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;

        //무기가 눌리고 점프나 회피 중이 아닐 경우
        if((sDown1 || sDown2 || sDown3) && !isJump && !isDodge && !isSwap && !isShop)
        {
            //이미 장착중인 무기가 없으면 setActive 줄 사용하지 않음
            if (equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);
            equipWeaponIndex = weaponIndex;//변경한 무기 인덱스 변경
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>(); //변경한 무기 장착시키기
            weapons[weaponIndex].gameObject.SetActive(true); //변경한 무기 활성화

            //바꾸기 애니메이션 불러오기
            anim.SetTrigger("doSwap");

            isSwap = true;

            Invoke("SwapOut", 0.04f);
        }
    }
    void SwapOut()
    {
        isSwap = false;
    }

    void Interation()
    {
        //상호작용
        //상효작용 키 눌림, 근처에 오브젝트가 있음, 점프나 회피 중이 아니면 상호작용
        if(iDown && nearObject != null && !isJump && !isDodge && !isShop)
        {
            //무기일 경우
            if(nearObject.tag == "Weapon")
            {
                //주변의 감지된 무기를 받아옴
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value; //해당 무기 번호
                hasWeapons[weaponIndex] = true;

                //무기는 획득한 후 사라짐
                Destroy(nearObject);
            }
            //상점일 경우
            else if(nearObject.tag == "Shop")
            {
                //상정 스크립트 가져오기
                Shop shop = nearObject.GetComponent<Shop>();
                //상점 입장 처리
                isShop = true;
                shop.Enter(this);
            }
        }
    }

    void FreezeRotation()
    {
        //캐릭터가 다른 rigidBody와 충돌했을 때 일어나는 회전력 0으로 초기화
        rigid.angularVelocity = Vector3.zero;
    }

    void StopToWall()
    {
        //벽에서 더이상 이동하지 않기 위해 만든 레이
        //레이의 시작점, 레이의 방향과 길이, 색
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        //레이어마스크가 Wall인 물체와 레이가 닿으면 isBorder은 true
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    }

    private void FixedUpdate()
    {
        //FixedUpdate: 프레임 단위의 일정간격으로 호출되는 함수
        FreezeRotation();
        StopToWall();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //케릭터가 바닥과 닿아있다면 '점프중'을 false로
        if(collision.gameObject.tag == "Floor")
        {
            //애니메이션 효과(land 애니메이션 가져오기)
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //아이템 감지
        if (other.tag == "Weapon" || other.tag == "Shop")
        {
            nearObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //아이템 감지 종료
        if (other.tag == "Weapon") //무기
            nearObject = null;
        //상점 감지 종료
        else if(other.tag == "Shop")
        {
            //상점 스크립트 가져오기
            Shop shop = nearObject.GetComponent<Shop>();
            //나가기 처리
            if(shop != null)
            {
                shop.Exit();
            }
            
            isShop = false;
            nearObject = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //아이템 획득
        if(other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo://총알 획득
                    ammo += item.value;
                    if (ammo > maxAmmo) //최대 값을 넘지 못하도록
                        ammo = maxAmmo;
                    break;
                case Item.Type.Coin://코인 획득
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;
                case Item.Type.Heart://채력 획득
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Grenade://수류탄 획득
                    //수류탄 활성화
                    grenades[hasGrendes].SetActive(true);
                    hasGrendes += item.value;
                    if (hasGrendes > maxHasGrendes)
                        hasGrendes = maxHasGrendes;
                    break;
            }

            //획득한 아이템은 디스트로이
            Destroy(other.gameObject);
        }
        else if(other.tag == "EnemyBullet")
        {
            if (!isDamage) //데미지를 받고 있는 중이 아니라면 실행
            {
                //적에게 공격받을 경우
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;

                //맞은 대상이 보스라면
                bool isBossAtk = other.name == "Boss Melee Area";

                //피격시 코루틴 시작
                StartCoroutine(onDamage(isBossAtk));

            }

            //데미지를 받고 있는 중이는 아니는 그냥 맞으면 없애기
            //rigidBody가 있으면 해당 bullet 삭제
            if (other.GetComponent<Rigidbody>() != null)
            {
                Destroy(other.gameObject);
            }

        }
    }

    IEnumerator onDamage(bool isBossAtk)
    {
        //피격시 1초동안 노란색으로 무적 상태
        isDamage = true;
        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.yellow;
        }

        //만약 보스의 공격이라면
        if (isBossAtk)
        {
            //공격받고 뒤로 물러나기
            rigid.AddForce(transform.forward * -25, ForceMode.Impulse);
        }
        yield return new WaitForSeconds(1f);

        //다시 원래대로 돌아옴
        isDamage = false;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }

        //보스 공격시 주었던 힘 다시 초기화
        if (isBossAtk)
        {
            rigid.velocity = Vector3.zero;
        }
    }
}
