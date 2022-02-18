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
            StartCoroutine(OnDamage(reactVec));
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
            StartCoroutine(OnDamage(reactVec));
        }
    }

    IEnumerator OnDamage(Vector3 reactVec)
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

            //���� ����
            reactVec = reactVec.normalized;
            reactVec += Vector3.up; //���ε� �˹� �ֱ�
            //�˹� �ֱ�
            rigid.AddForce(reactVec * 5, ForceMode.Impulse);

            Destroy(gameObject, 4);
        }
    }
}
