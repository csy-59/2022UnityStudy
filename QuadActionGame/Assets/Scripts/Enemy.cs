using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //채력
    public int maxHealth;
    public int curHealth;

    //물리 관련
    Rigidbody rigid;
    BoxCollider boxCollider;
    //피격 효과
    Material mat;

    private void Awake()
    {
        //초기화
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        //material은 MeshRenderer에서만 가져올 수 있음
        mat = GetComponent<MeshRenderer>().material;
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        //수류탄 피격처리
        curHealth -= 100;
        //피격 받은 상대적 방향 구하기
        Vector3 reactVec = transform.position - explosionPos;
        //구한 방향을 기반으로 
        StartCoroutine(OnDamage(reactVec, true));
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee")
        {
            //근접 공격
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            //넉백에 관한 방향(공격 받은 방향)
            Vector3 reactVec = transform.position - other.transform.position;
            //피격 코루틴 시작
            StartCoroutine(OnDamage(reactVec, false));
        }
        else if(other.tag == "Bullet")
        {
            //원거리 공격
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            //넉백에 관한 방향(공격 받은 방향)
            Vector3 reactVec = transform.position - other.transform.position;
            //총알은 삭제해서 통과하지 못하도록
            Destroy(other.gameObject);
            //피격 코루틴 시작
            StartCoroutine(OnDamage(reactVec, false));
        }
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        //피격시 빨강색으로
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if(curHealth > 0)
        {
            //아직 죽지 않았다면 다시 원래 색으로
            mat.color = Color.white;
        } else
        {
            //죽으면 회색으로
            mat.color = Color.gray;
            //죽으면 더이상 피격 받지 않도록
            gameObject.layer = 12;


            if (isGrenade)
            {
                //방향 통일
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3; //위로도 넉백 주기

                //수류탄으로 죽으면 더 회전하는 이펙트를 주기 위해 freeze 수정
                rigid.freezeRotation = false;
                //넉백 주기
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);
            } else
            {
                //방향 통일
                reactVec = reactVec.normalized;
                reactVec += Vector3.up; //위로도 넉백 주기
                //넉백 주기
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
            }
            

            Destroy(gameObject, 4);
        }
    }
}
