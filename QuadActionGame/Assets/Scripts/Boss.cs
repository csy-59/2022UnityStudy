using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    //�̻��� ����
    public GameObject missile; //�̻��� �������� ���� ��
    public Transform missilePortA; //�̻����� ���� ��Ʈ 1
    public Transform missilePortB; //�̻����� ���� ��Ʈ 2

    //���� Enemy�� bullet ���
    //������ ������� �������� �����ϰ� �ϱ� ���� ����
    Vector3 lookVec;
    Vector3 tauntVec; //��� ���� �� ���� ����

    public bool isLook; //�÷��̾ �ٶ󺸰� �ִ���

    private void Awake()
    {
        /*��� �� Awake�� �ڽ� ��ũ��Ʈ�� �ִ� �����θ� ����
         ��, �ڽĽ�ũ��Ʈ�� ��� ���� ������ �������*/
        //�ʱ�ȭ
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        //material�� MeshRenderer������ ������ �� ����
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        //�Ѿư��� �ʱ�
        nav.isStopped = true;

        StartCoroutine(Think());
    }

    private void Update()
    {
        //������
        if (isDead)
        {
            //��� �ൿ ����
            StopAllCoroutines();
            return;
        }

        if (isLook)
        {
            //�÷��̾� �Է°��� �����Ͽ� ���� ���� ����
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5f; //���� ���� �����ϱ�
            //���� ���͸� ����Ͽ� �÷��̾� ���� �������� �ٶ󺸱�
            transform.LookAt(target.position + lookVec); //target�� Enemy�� target
        } else
        {
            nav.SetDestination(tauntVec);
        }
    }

    IEnumerator Think()
    {
        //�ൿ ���� �������ֱ� 
        //�ణ �ð��� �ְ� ����
        yield return new WaitForSeconds(0.1f);

        //� ������ ������� �������� ���� ����(0���� 4����)
        int ranAction = Random.Range(0, 5);
        switch (ranAction)
        {
            case 0:
            case 1:
                //�̻��� ����
                StartCoroutine(MissileShot());
                break;
            case 2:
            case 3:
                //�� ����
                StartCoroutine(RockShot());
                break;
            case 4:
                //��� ����
                StartCoroutine(Taunt());
                break;
        }
    }

    IEnumerator MissileShot()
    {
        //�̻��� �߻�(�ϳ��� 0.2�� �ڿ�, �ϳ��� 0.5���Ŀ�)
        anim.SetTrigger("doShot"); //�ִϸ��̼�

        yield return new WaitForSeconds(0.2f); //���ϸ��̼��� ���߱� ����
        //�̻��� ������
        GameObject instantMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation);
        //�̻����� ��ũ��Ʈ�� ���� ��ǥ�� ����
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
        bossMissileA.target = target;

        yield return new WaitForSeconds(0.3f); //���ϸ��̼��� ���߱� ����
        //�̻��� ������
        GameObject instantMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation);
        //�̻����� ��ũ��Ʈ�� ���� ��ǥ�� ����
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        bossMissileB.target = target;

        yield return new WaitForSeconds(2f); //���ϸ��̼��� ���߱� ����

        StartCoroutine(Think());//�ٽ� �߰� ����
    }

    IEnumerator RockShot()
    {
        //���� ������ ���� ������⸦ �Ҷ� �ٶ󺸱� ����
        isLook = false;
        anim.SetTrigger("doBigShot");//�ִϸ��̼�
        //�� ����(���� �Ŀ��� �� ��ũ��Ʈ�� ��)
        Instantiate(bullet, transform.position, transform.rotation);
        yield return new WaitForSeconds(3f);//���ϸ��̼��� ���߱� ����
        isLook = true;
        StartCoroutine(Think());//�ٽ� �߰� ����
    }

    IEnumerator Taunt()
    {
        //�������� ��ġ ����
        tauntVec = target.position + lookVec;

        isLook = false;
        nav.isStopped = false; //�Ѿư��� ����
        //��� ���� �ڽ� �ݶ��̴��� �÷��̾ ���� �ʵ���
        boxCollider.enabled = false;
        anim.SetTrigger("doTaunt");//�ִϸ��̼�

        yield return new WaitForSeconds(1.5f); // ���� ���� Ȱ��ȭ�� ���� ��
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f); // ���� ���� ��
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);//���ϸ��̼��� ���߱� ����

        //�ʱ�ȭ
        isLook = true;
        nav.isStopped = true; //�Ѿư��� ����
        boxCollider.enabled = true;
        StartCoroutine(Think());//�ٽ� �߰� ����
    }
}

