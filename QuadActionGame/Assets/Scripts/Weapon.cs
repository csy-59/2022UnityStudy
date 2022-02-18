using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //무기 타입, 데미지, 공속, 범위 효과 변수 등
    public enum Type { Melee, Range}; //무기 타입 종류
    public Type type;   //무기 타입
    public int damage; //데미지
    public float rate; //공속

    //총알 갯수
    public int maxAmmo;
    public int curAmmo;

    public BoxCollider meleeArea; //근거리 범위
    public TrailRenderer trailEffect; //공격시 이펙트
    public Transform bulletPos; //총알이 나오는 위치
    public GameObject bullet; //총알 프리펩을 저장할 변수
    public Transform bulletCasePos; //탄피이 나오는 위치
    public GameObject bulletCase; //탄피 프리펩을 저장할 변수

    public void Use()
    {
        if(type == Type.Melee)
        {
            StopCoroutine("Swing");//코루틴 멈춤. 호출 전에 불러와서 로직이 꼬이지 않도록 함
            StartCoroutine("Swing");//코루틴 호출
        }

        else if(type == Type.Range && curAmmo > 0) //총알 갯수가 있으면
        {
            curAmmo--; //총알 하나 감소
            StopCoroutine("Shot");//코루틴 멈춤. 호출 전에 불러와서 로직이 꼬이지 않도록 함
            StartCoroutine("Shot");//코루틴 호출
        }
    }

    IEnumerator Swing()
    {
        //코루틴!!

        //1구역: 코드 실행
        yield return new WaitForSeconds(0.1f);//0.1초 대기. yield로 결과 전달.
        meleeArea.enabled = true; //콜라이더 실행
        trailEffect.enabled = true; //이펙트 실행

        //2구역 : 코드 실행
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false; //콜라이더 중지


        //3구역 : 코드 실행
        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false; //이펙트 중지

        //4구역 : 코드 실행
        //yield break; // 코루틴 종료. 여기 밑에 있는 코드는 실행되지 않는다.
    }

    IEnumerator Shot()
    {
        //총알 발사
        GameObject intantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);//총알 생성
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50; //총알 속도

        yield return null;

        //탄피 배출
        GameObject intantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);//총알 생성
        Rigidbody caseRigid = intantCase.GetComponent<Rigidbody>();
        //탄피에 약간의 반동주기. 뒤로 줘야하기 때문에 forward에 음수값, 위쪽으로 힘을 줌
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse); //반동 적용
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);//회전 주기
    }

    //Use() 메인 루틴에서 SWing() 서브 루틴으로 가고 이 루틴이 끝나야 Use 메인루틴으로 돌아감
    //코루틴은 이와 달리 메인 루틴과 서브 루틴(여기에서는 코루틴)이 동시에 실행된다.
}
