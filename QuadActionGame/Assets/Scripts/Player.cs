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

    //���ϸ��̼� ����
    Animator anim;

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //�̵����� �� �޾ƿ���
        hAxis = Input.GetAxisRaw("Horizontal"); //�¿�
        vAxis = Input.GetAxisRaw("Vertical"); //�յ�
        wDown = Input.GetButton("Walk");

        //�밢������ �̵��� �ణ ���̰� �� ���� 1�� �ٲ��ִ� ��: normalized
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        //�̵����� �ȱ� �Ҷ��� �ӵ� ���߱�(0.3���)
        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;


        //���ϸ��̼� ����
        anim.SetBool("isRun", moveVec != Vector3.zero); //�޸���(�̵� ���� 0�� �ƴ� ��)
        anim.SetBool("isWalk", wDown); //�ȱ�(left shift�� ������ ��)

        //ȸ���ϱ�: �ٶ󺸴� �������� �� ȸ��
        transform.LookAt(transform.position + moveVec);
    }
}
