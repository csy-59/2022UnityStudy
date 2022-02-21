using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    //���� Ÿ��
    public enum Type { A, B, C, D };
    public Type enemyType;

    //ä��
    public int maxHealth;
    public int curHealth;

    //����
    public int score;

    //��� ó���� ���� GameManager
    public GameManager manager;

    //���� ����
    public GameObject[] coins;

    //�Ѱ� �ִ���
    public bool isChase;
    //�׾�����
    public bool isDead;

    //��ǥ(ĳ����)
    public Transform target;

    //���� ���� ����ȭ
    public BoxCollider meleeArea;
    //���Ÿ� ����(CŸ��) �̻����� ���� ������Ʈ
    public GameObject bullet;
    //���� ������
    public bool isAttack;

    //���� ����
    public Rigidbody rigid;
    public BoxCollider boxCollider;
    //�ǰ� ȿ��
    public MeshRenderer[] meshs;

    //����Ƽ���� �����ϴ� AI
    public NavMeshAgent nav;
    //NavMesh: NavMeshAgent�� ��θ� �׸��� ���� ����
    //NavMesh�� ����Ƽ�� Window > AI > Navigation���� bake �ǿ��� bake�� �������Ѵ�.
    //�ִϸ��̼�
    public Animator anim;

    private void Awake()
    {
        //�ʱ�ȭ
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        //material�� MeshRenderer������ ������ �� ����
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        //���� �� ��� �Ѿư��� ����
        if(enemyType != Type.D)
        {
            //�����ǰ� 2�� �ڿ� �Ѿư��� ����
            Invoke("ChaseStart", 2);
        }
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
        if (nav.enabled && enemyType != Type.D) //nav�� Ȱ��ȭ �Ǿ� ���� ����(��ǥ�� ã�� �� ���� ����) ������
        {
            //��ǥ�� ���󰡵��� ����
            nav.SetDestination(target.position);
            //�Ѿư��� ���� �� ����
            nav.isStopped = !isChase;
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
        foreach(MeshRenderer mesh in meshs)
            //�ǰݽ� ����������
            mesh.material.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        if(curHealth > 0)
        {
            //���� ���� �ʾҴٸ� �ٽ� ���� ������
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.white;
        } else
        {
            isDead = true;
            //������ ȸ������
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray;
            //������ ���̻� �ǰ� ���� �ʵ���
            gameObject.layer = 12;
            //�ѱ� ����
            isChase = false;
            //�˹� ȿ���� �����ϱ� ���Ͽ� NavAgent ��Ȱ��ȭ
            nav.enabled = false;
            //�ִϸ��̼� ȿ��
            anim.SetTrigger("doDie");
            //�÷��̾�� ���� �ֱ�
            Player player = target.GetComponent<Player>();
            player.score += score;
            //���� ���
            int ranCoin = Random.Range(0, 3);
            Instantiate(coins[ranCoin], transform.position, Quaternion.identity);

            //�� Ÿ�Կ� ���� ���ó��(���� ����)
            switch (enemyType)
            {
                case Type.A:
                    manager.enemyCntA--;
                    break;
                case Type.B:
                    manager.enemyCntB--;
                    break;
                case Type.C:
                    manager.enemyCntC--;
                    break;
                case Type.D:
                    manager.enemyCntD--;
                    break;
            }

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

    void Targeting()
    {
        //������ �ƴ� ���, ����ִ� ��쿡�� �߰���
        if (enemyType != Type.D && !isDead)
        {
            //Ÿ���� : lay ���, ������ �����ؾ��ϱ� ������ SphereCastAll ���
            float targetRadius = 0f;//lay ������
            float targetRange = 0f; //lay ����

            //���� Ÿ�� ���� Ÿ���� lay ���� �������ֱ�
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
                    //��Ȯ���� ���Ͽ� ������ �۰�
                    targetRadius = 0.5f;
                    targetRange = 25f;
                    break;
            }

            //SphereCastAll(<lay ����>, <lay ������>, <lay ����>, <lay ����>, <lay ���� ���>)
            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 
                targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

            //ray�� �÷��̾ �����ϰ� �̹� �������� �ƴ϶��
            if (rayHits.Length > 0 && !isAttack)
            {
                //����!
                StartCoroutine(Attack());
            }

        }
    }

    IEnumerator Attack()
    {
        //���� ����!
        isChase = false;//�߰� ����
        isAttack = true;//���� ����
        anim.SetBool("isAttack", true);//�ִϸ��̼� ����

        //���� ��������
        switch (enemyType)
        {
            case Type.A:
                //�ִϸ��̼ǿ����� �����̸� �����Ͽ� �Ŀ� ���� Ȱ��ȭ
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;//���� ���� Ȱ��ȭ(player���� �̿� �ε�ġ�� ������)

                //������ ���� ���� ��Ȱ��ȭ
                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;

                //���� �� ��� ��
                yield return new WaitForSeconds(1f);
                break;

            case Type.B:
                //���� ����
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                meleeArea.enabled = true;

                //�����ؼ� ����
                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero; //�ӵ� ���߱�
                meleeArea.enabled = false;

                //���� �� ��� ��
                yield return new WaitForSeconds(2f);
                break;

            case Type.C:
                //���Ÿ� ����
                yield return new WaitForSeconds(0.5f);
                //������ ���� �̻��� ��ü ����
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                //�̻����� rigidbody�� �����ͼ� ����
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;

                //���� �� ��� ��
                yield return new WaitForSeconds(2f);
                break;
        }

        //���� �� ó��
        isChase = true;//�߰� ����
        isAttack = false;//���� ����
        anim.SetBool("isAttack", false);//�ִϸ��̼� ����
    }

    private void FixedUpdate()
    {
        //Ÿ���� ���� ���� ���� ������ Ÿ������ ��
        Targeting();

        //FixedUpdate: ������ ������ ������������ ȣ��Ǵ� �Լ�
        FreezeVelocity();
    }
}
