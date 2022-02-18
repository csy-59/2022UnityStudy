using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //총알의 데미지
    public int damage;

    private void OnCollisionEnter(Collision collision)
    {
        //탄피가 땅에 떨어지면 3초뒤 사라짐
        if(collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3);
        }
        //총알이 벽에 박히면 바로 사라짐
        else if (collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
