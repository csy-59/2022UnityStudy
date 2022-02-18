using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    //������ Ÿ��: ������ Ÿ�� �̸�(���� �ƴ�)
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon};
    //����
    public Type type;
    public int value;

    //�������� �ٸ� ��ü�� �浹�ϴ� ���� �����ϱ� ���� ����
    Rigidbody rigid;
    SphereCollider sphereCollider;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        //���� ���� SphereCollider�� ������(�߷� ���)
        sphereCollider = GetComponent<SphereCollider>();
    }

    private void Update()
    {
        //������ ȿ��
        transform.Rotate(Vector3.up * 30 * Time.deltaTime);//����
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            rigid.isKinematic = true;
            sphereCollider.enabled = false;
        }
    }
}
