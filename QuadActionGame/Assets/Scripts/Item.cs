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

    private void Update()
    {
        //������ ȿ��
        transform.Rotate(Vector3.up * 30 * Time.deltaTime);//����

    }
}