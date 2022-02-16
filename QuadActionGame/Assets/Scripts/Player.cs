using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    //���� ����
    public GameObject[] weapons; //���� ����
    public bool[] hasWeapons; //���� �ִ���

    float hAxis;
    float vAxis;
    bool wDown;
    bool jDown;

    //������ ��ȣ�ۿ�
    bool iDown;
    //���� ������
    bool isJump = false;
    //ȸ�� ������
    bool isDodge = false;

    //����
    Vector3 moveVec; //�̵��� ����
    Vector3 dodgeVec; //ȸ�ǽ� ����

    //���� ȿ���� ���� ����
    Rigidbody rigid;
    //�ִϸ��̼� ����
    Animator anim;

    //������ ����
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
        //�� �޾ƿ���
        GetInput();
        //�̵�
        Move();
        //ȸ��
        Turn();
        //����
        Jump();
        //ȸ��
        Dodge();
        //��ȣ�ۿ�
        Interation();
    }

    void GetInput()
    {
        //�̵����� �� �޾ƿ���
        hAxis = Input.GetAxisRaw("Horizontal"); //�¿�
        vAxis = Input.GetAxisRaw("Vertical"); //�յ�
        wDown = Input.GetButton("Walk"); //�ȱ�
        jDown = Input.GetButtonDown("Jump"); //����
        iDown = Input.GetButtonDown("Interation"); //��ȣ�ۿ�(������)
    }

    void Move()
    {
        //�밢������ �̵��� �ణ ���̰� �� ���� 1�� �ٲ��ִ� ��: normalized
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        //ȸ�Ǹ� �ϰ� ���� ��� ȸ�� �������θ� �̵��� �� �ֵ���
        if (isDodge)
            moveVec = dodgeVec;

        //�̵����� �ȱ� �Ҷ��� �ӵ� ���߱�(0.3���)
        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;


        //�ִϸ��̼� ����
        anim.SetBool("isRun", moveVec != Vector3.zero); //�޸���(�̵� ���� 0�� �ƴ� ��)
        anim.SetBool("isWalk", wDown); //�ȱ�(left shift�� ������ ��)
    }

    void Turn()
    {
        //ȸ���ϱ�: �ٶ󺸴� �������� �� ȸ��
        transform.LookAt(transform.position + moveVec);
    }
    void Jump()
    {
        //����Ű�� ������ ������/ȸ������ �ƴϰ�, �������� ���� ��� ����
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge)
        {
            //������ ���� �������� 15��ŭ�� ���� ������(Impulse)���� ��
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            //�ִϸ��̼� ȿ��
            anim.SetBool("isJump", true);//������ true��(land �ִϸ��̼��� ����)
            anim.SetTrigger("doJump");//���� �ִϸ��̼� Ʈ���ŷ� �ҷ�����
            isJump = true;
        }
    }

    void Dodge()
    {
        //����Ű�� ������ �������� �ƴϰ�, �����̴� ���� �� ȸ��
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge)
        {
            dodgeVec = moveVec;
            //ȸ������ ���� �̵��ӵ� 2��
            speed *= 2;
            anim.SetTrigger("doDodge");//ȸ�� �ִϸ��̼� Ʈ���ŷ� �ҷ�����
            isDodge = true;

            //�ð����� �ΰ� ȸ�� ����
            Invoke("DodgeOut", 0.6f);
        }
    }

    void DodgeOut()
    {
        //ȸ�� ����
        speed /= 2;
        isDodge = false;
    }    

    void Interation()
    {
        //��ȣ�ۿ�
        //��ȿ�ۿ� Ű ����, ��ó�� ������Ʈ�� ����, ������ ȸ�� ���� �ƴϸ� ��ȣ�ۿ�
        if(iDown && nearObject != null && !isJump && !isDodge)
        {
            //������ ���
            if(nearObject.tag == "Weapon")
            {
                //�ֺ��� ������ ���⸦ �޾ƿ�
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value; //�ش� ���� ��ȣ
                hasWeapons[weaponIndex] = true;

                //����� ȹ���� �� �����
                Destroy(nearObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //�ɸ��Ͱ� �ٴڰ� ����ִٸ� '������'�� false��
        if(collision.gameObject.tag == "Floor")
        {
            //�ִϸ��̼� ȿ��(land �ִϸ��̼� ��������)
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //������ ����
        if (other.tag == "Weapon") //����
            nearObject = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        //������ ���� ����
        if (other.tag == "Weapon") //����
            nearObject = null;
    }
}
