using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    //ä��
    public int maxHealth;
    public int curHealth;

    //�Ѱ� �ִ���
    public bool isChase;

    //��ǥ(ĳ����)
    public Transform target;

    //���� ����
    Rigidbody rigid;
    BoxCollider boxCollider;
    //�ǰ� ȿ��
    Material mat;

    //����Ƽ���� �����ϴ� AI
    NavMeshAgent nav;
    //NavMesh: NavMeshAgent�� ��θ� �׸��� ���� ����
    //NavMesh�� ����Ƽ�� Window > AI > Navigation���� bake �ǿ��� bake�� �������Ѵ�.
    //�ִϸ��̼�
    Animator anim;

    private void Awake()
    {
        //�ʱ�ȭ
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        //material�� MeshRenderer������ ������ �� ����
        mat = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        //�����ǰ� 2�� �ڿ� �Ѿư��� ����
        Invoke("ChaseStart", 2);
    }

    void ChaseStart()
    {
        //�Ѿư��� ����
        isChase = true;
        //�ִϸ��̼� ����
        anim.SetBool("isWalk", true);
    }

    private void Update()
    {
        if (isChase) //�Ѿư��� �߿���
        {
            //��ǥ�� ���󰡵��� ����
            nav.SetDestination(target.position);
        }
        
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        //����ź �ǰ�ó��
        curHealth -= 100;
        //�ǰ� ���� ����� ���� ���ϱ�
        Vector3 reactVec = transform.position - explosionPos;
        //���� ������ ������� 
        StartCoroutine(OnDamage(reactVec, true));
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee")
        {
            //���� ����
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            //�˹鿡 ���� ����(���� ���� ����)
            Vector3 reactVec = transform.position - other.transform.position;
            //�ǰ� �ڷ�ƾ ����
            StartCoroutine(OnDamage(reactVec, false));
        }
        else if(other.tag == "Bullet")
        {
            //���Ÿ� ����
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            //�˹鿡 ���� ����(���� ���� ����)
            Vector3 reactVec = transform.position - other.transform.position;
            //�Ѿ��� �����ؼ� ������� ���ϵ���
            Destroy(other.gameObject);
            //�ǰ� �ڷ�ƾ ����
            StartCoroutine(OnDamage(reactVec, false));
        }
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        //�ǰݽ� ����������
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if(curHealth > 0)
        {
            //���� ���� �ʾҴٸ� �ٽ� ���� ������
            mat.color = Color.white;
        } else
        {
            //������ ȸ������
            mat.color = Color.gray;
            //������ ���̻� �ǰ� ���� �ʵ���
            gameObject.layer = 12;
            //�ѱ� ����
            isChase = false;
            //�˹� ȿ���� �����ϱ� ���Ͽ� NavAgent ��Ȱ��ȭ
            nav.enabled = false;
            //�ִϸ��̼� ȿ��
            anim.SetTrigger("doDie");


            if (isGrenade)
            {
                //���� ����
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3; //���ε� �˹� �ֱ�

                //����ź���� ������ �� ȸ���ϴ� ����Ʈ�� �ֱ� ���� freeze ����
                rigid.freezeRotation = false;
                //�˹� �ֱ�
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);
            } else
            {
                //���� ����
                reactVec = reactVec.normalized;
                reactVec += Vector3.up; //���ε� �˹� �ֱ�
                //�˹� �ֱ�
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
            }
            

            Destroy(gameObject, 4);
        }
    }

    void FreezeVelocity()
    {
        //ĳ���Ͱ� �ٸ� rigidBody�� �浹���� �� �Ͼ�� ȸ���°� �ݵ� 0���� �ʱ�ȭ
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
        
    }

    private void FixedUpdate()
    {
        //FixedUpdate: ������ ������ ������������ ȣ��Ǵ� �Լ�
        FreezeVelocity();
    }
}
