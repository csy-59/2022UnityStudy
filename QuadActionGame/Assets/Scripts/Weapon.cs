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
    public BoxCollider meleeArea; //근거리 범위
    public TrailRenderer trailEffect; //공격시 이펙트

    public void Use()
    {
        if(type == Type.Melee)
        {
            StopCoroutine("Swing");//코루틴 멈춤. 호출 전에 불러와서 로직이 꼬이지 않도록 함
            StartCoroutine("Swing");//코루틴 호출
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

    //Use() 메인 루틴에서 SWing() 서브 루틴으로 가고 이 루틴이 끝나야 Use 메인루틴으로 돌아감
    //코루틴은 이와 달리 메인 루틴과 서브 루틴(여기에서는 코루틴)이 동시에 실행된다.
}
