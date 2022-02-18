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

    //아이템 관련
    public int ammo; //총알
    public int coin; //코인
    public int health; //채력

    public int maxAmmo; //최대 총알
    public int maxCoin; //최대 코인
    public int maxHealth; //최대 채력
    public int maxHasGrendes; //최대 수류탄

    float hAxis;
    float vAxis;
    bool wDown;
    bool jDown;
    
    //공격하기
    bool fDwon; 
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

    //벡터
    Vector3 moveVec; //이동시 벡터
    Vector3 dodgeVec; //회피시 백터

    //물리 효과를 위한 변수
    Rigidbody rigid;
    //애니메이션 관련
    Animator anim;

    //아이템 감지
    GameObject nearObject;
    //장착중인 무기
    Weapon equipWeapon;
    int equipWeaponIndex = -1;
    //공격 딜레이
    float fireDelay; 

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
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
        //공격
        Attack();
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
        fDwon = Input.GetButtonDown("Fire1");
    }

    void Move()
    {
        //대각선으로 이동시 약간 길이가 긴 것을 1로 바꿔주는 것: normalized
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        //회피를 하고 있을 경우 회피 방향으로만 이동할 수 있도록
        if (isDodge)
            moveVec = dodgeVec;

        //무기 변경 중이라면, 공격중이 아니라면 이동하지 않음
        if (isSwap || !isFireReady)
            moveVec = Vector3.zero;

        //이동관련 걷기 할때는 속도 늦추기(0.3배로)
        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;


        //애니메이션 관련
        anim.SetBool("isRun", moveVec != Vector3.zero); //달리기(이동 값이 0이 아닐 때)
        anim.SetBool("isWalk", wDown); //걷기(left shift가 눌렸을 때)
    }

    void Turn()
    {
        //회전하기: 바라보는 방향으로 몸 회전
        transform.LookAt(transform.position + moveVec);
    }
    void Jump()
    {
        //점프키가 눌리고 점프중/회피중이 아니고, 움직이지 않을 경우 점프
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && isSwap)
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
        if(fDwon && isFireReady && !isDodge && !isSwap)
        {
            equipWeapon.Use();
            anim.SetTrigger("doSwing");//애니메이션 처리
            fireDelay = 0;//공격 대기 시간 초기화
        }
    }

    void Dodge()
    {
        //점프키가 눌리고 점프중이 아니고, 움직이는 중일 때 회피
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && isSwap)
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
        if((sDown1 || sDown2 || sDown3) && !isJump && !isDodge && !isSwap)
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
        if(iDown && nearObject != null && !isJump && !isDodge)
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
        }
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
        if (other.tag == "Weapon") //무기
            nearObject = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        //아이템 감지 종료
        if (other.tag == "Weapon") //무기
            nearObject = null;
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
    }
}
