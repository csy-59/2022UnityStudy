using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //�Ѿ��� ������
    public int damage;
    //���� ��������
    public bool isMelee;

    private void OnCollisionEnter(Collision collision)
    {
        //ź�ǰ� ���� �������� 3�ʵ� �����
        if(collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3);
        }
        //�Ѿ��� ���� ������ �ٷ� �����
        else if (collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //�Ѿ��� ���� ������ �ٷ� �����(���� ������ �ƴϸ�)
        if (!isMelee && other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
