using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    //무기 관련
    public GameObject[] weapons; //무기 종류
    public bool[] hasWeapons; //갖고 있는지

    float hAxis;
    float vAxis;
    bool wDown;
    bool jDown;

    //아이템 상호작용
    bool iDown;
    //점프 중인지
    bool isJump = false;
    //회피 중인지
    bool isDodge = false;

    //벡터
    Vector3 moveVec; //이동시 벡터
    Vector3 dodgeVec; //회피시 백터

    //물리 효과를 위한 변수
    Rigidbody rigid;
    //애니메이션 관련
    Animator anim;

    //아이템 감지
    GameObject nearObject;

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
        //회피
        Dodge();
        //상호작용
        Interation();
    }

    void GetInput()
    {
        //이동관련 값 받아오기
        hAxis = Input.GetAxisRaw("Horizontal"); //좌우
        vAxis = Input.GetAxisRaw("Vertical"); //앞뒤
        wDown = Input.GetButton("Walk"); //걷기
        jDown = Input.GetButtonDown("Jump"); //점프
        iDown = Input.GetButtonDown("Interation"); //상호작용(아이템)
    }

    void Move()
    {
        //대각선으로 이동시 약간 길이가 긴 것을 1로 바꿔주는 것: normalized
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        //회피를 하고 있을 경우 회피 방향으로만 이동할 수 있도록
        if (isDodge)
            moveVec = dodgeVec;

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
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge)
        {
            //벡터의 위쪽 방향으로 15만큼의 힘을 순간적(Impulse)으로 줌
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            //애니메이션 효과
            anim.SetBool("isJump", true);//점프중 true로(land 애니메이션을 위해)
            anim.SetTrigger("doJump");//점프 애니메이션 트리거로 불러오기
            isJump = true;
        }
    }

    void Dodge()
    {
        //점프키가 눌리고 점프중이 아니고, 움직이는 중일 때 회피
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge)
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
}
