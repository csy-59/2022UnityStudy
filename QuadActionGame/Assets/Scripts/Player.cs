using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    float hAxis;
    float vAxis;
    bool wDown;

    Vector3 moveVec;

    //에니메이션 관련
    Animator anim;

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //이동관련 값 받아오기
        hAxis = Input.GetAxisRaw("Horizontal"); //좌우
        vAxis = Input.GetAxisRaw("Vertical"); //앞뒤
        wDown = Input.GetButton("Walk");

        //대각선으로 이동시 약간 길이가 긴 것을 1로 바꿔주는 것: normalized
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        //이동관련 걷기 할때는 속도 늦추기(0.3배로)
        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;


        //에니메이션 관련
        anim.SetBool("isRun", moveVec != Vector3.zero); //달리기(이동 값이 0이 아닐 때)
        anim.SetBool("isWalk", wDown); //걷기(left shift가 눌렸을 때)

        //회전하기: 바라보는 방향으로 몸 회전
        transform.LookAt(transform.position + moveVec);
    }
}
