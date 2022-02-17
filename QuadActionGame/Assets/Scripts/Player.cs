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

    //���� �ٲٱ�
    bool sDown1; //1��
    bool sDown2; //2��
    bool sDown3; //3��
    //������ ��ȣ�ۿ�
    bool iDown;
    //���� ������
    bool isJump = false;
    //ȸ�� ������
    bool isDodge = false;
    //���� ���� ������
    bool isSwap = false;

    //����
    Vector3 moveVec; //�̵��� ����
    Vector3 dodgeVec; //ȸ�ǽ� ����

    //���� ȿ���� ���� ����
    Rigidbody rigid;
    //�ִϸ��̼� ����
    Animator anim;

    //������ ����
    GameObject nearObject;
    //�������� ����
    GameObject equipWeapon;
    int equipWeaponIndex = -1;

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
        //���� �ٲٱ�
        Swap();
    }

    void GetInput()
    {
        //�̵����� �� �޾ƿ���
        hAxis = Input.GetAxisRaw("Horizontal"); //�¿�
        vAxis = Input.GetAxisRaw("Vertical"); //�յ�
        wDown = Input.GetButton("Walk"); //�ȱ�
        jDown = Input.GetButtonDown("Jump"); //����

        iDown = Input.GetButtonDown("Interation"); //��ȣ�ۿ�(������)
        
        //���� ���� ���� �� �޾ƿ���
        sDown1 = Input.GetButtonDown("Swap1"); //���� �ٲٱ�:1��
        sDown2 = Input.GetButtonDown("Swap2"); //���� �ٲٱ�:2��
        sDown3 = Input.GetButtonDown("Swap3"); //���� �ٲٱ�:3��
    }

    void Move()
    {
        //�밢������ �̵��� �ణ ���̰� �� ���� 1�� �ٲ��ִ� ��: normalized
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        //ȸ�Ǹ� �ϰ� ���� ��� ȸ�� �������θ� �̵��� �� �ֵ���
        if (isDodge)
            moveVec = dodgeVec;

        //���� ���� ���̶�� �̵����� ����
        if (isSwap)
            moveVec = Vector3.zero;

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
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && isSwap)
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
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && isSwap)
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
    
    void Swap()
    {
        //���� ����
        //�����Ϸ��� ���Ⱑ ���ų� �̹� ���� ���� ��� �������� ����
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;

        //������ ���� ��ȣ �޾ƿ���
        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;

        //���Ⱑ ������ ������ ȸ�� ���� �ƴ� ���
        if((sDown1 || sDown2 || sDown3) && !isJump && !isDodge && !isSwap)
        {
            //�̹� �������� ���Ⱑ ������ setActive �� ������� ����
            if (equipWeapon != null)
                equipWeapon.SetActive(false);
            equipWeaponIndex = weaponIndex;//������ ���� �ε��� ����
            equipWeapon = weapons[weaponIndex]; //������ ���� ������Ű��
            weapons[weaponIndex].SetActive(true); //������ ���� Ȱ��ȭ

            //�ٲٱ� �ִϸ��̼� �ҷ�����
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
