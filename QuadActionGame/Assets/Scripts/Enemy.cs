using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //ä��
    public int maxHealth;
    public int curHealth;

    //���� ����
    Rigidbody rigid;
    BoxCollider boxCollider;
    //�ǰ� ȿ��
    Material mat;

    private void Awake()
    {
        //�ʱ�ȭ
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        //material�� MeshRenderer������ ������ �� ����
        mat = GetComponent<MeshRenderer>().material;
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
}
