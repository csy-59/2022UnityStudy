using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    //열거형 타입: 아이템 타입 이름(변수 아님)
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon};
    //변수
    public Type type;
    public int value;

    //아이템이 다른 물체와 충돌하는 것을 방지하기 위한 변수
    Rigidbody rigid;
    SphereCollider sphereCollider;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        //가장 위의 SphereCollider만 가져옴(중력 담당)
        sphereCollider = GetComponent<SphereCollider>();
    }

    private void Update()
    {
        //아이템 효과
        transform.Rotate(Vector3.up * 30 * Time.deltaTime);//돌기
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
