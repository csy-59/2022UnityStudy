using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //총알의 데미지
    public int damage;
    //근접 공격인지
    public bool isMelee;
    //돌인지
    public bool isRock;

    private void OnCollisionEnter(Collision collision)
    {
        //탄피가 땅에 떨어지면 3초뒤 사라짐(돌이 아닐 경우)
        if(!isRock && collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3);
        }
        //총알이 벽에 박히면 바로 사라짐
        else if (collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //총알이 벽에 박히면 바로 사라짐(근접 공격이 아니면)
        if (!isMelee && other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
