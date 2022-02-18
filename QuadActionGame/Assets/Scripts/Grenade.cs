using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    //수류탄 오브젝트를 담을 변수들
    public GameObject meshObject;
    public GameObject effectObject;
    public Rigidbody rigid;

    // Start is called before the first frame update
    void Start()
    {
        //폭발 코루틴 시작
        StartCoroutine(Explosion());
    }

    IEnumerator Explosion()
    {
        //3초뒤 폭발하게
        yield return new WaitForSeconds(3f);
        //물리적 속도, 회전 모두 초기화
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        //폭발 후 효과와 메시 오브젝트 꺼줌
        meshObject.SetActive(false);
        effectObject.SetActive(true);

        //주변의 적에게 데미지 주기: laycast 사용
        //범위이기 때문에 배열로 선언. SphereCastAll은 주변 모든 적을 다 받아옴
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 15, Vector3.up, 0f, LayerMask.GetMask("Enemy"));
        foreach(RaycastHit hitObj in rayHits) //주변에 감지된 모든 적에게 수류탄 피해 주기
        {
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }

        Destroy(gameObject, 5);
    }
}
