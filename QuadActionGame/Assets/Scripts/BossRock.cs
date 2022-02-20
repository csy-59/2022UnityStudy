using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet
{
    //돌이 굴러가유
    Rigidbody rigid;
    float angularPower = 2; //회전 파워
    float scaleValue = 0.1f; //돌 크기
    bool isShoot; //쏘고 있는지

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTimer());
        StartCoroutine(GainPower());
    }

    IEnumerator GainPowerTimer()
    {
        //돌을 굴리기까지 기를 모으는 구간
        yield return new WaitForSeconds(2.2f);
        isShoot = true;
    }

    IEnumerator GainPower()
    {
        while (!isShoot)
        {
            //기를 모으기
            angularPower += 0.2f;
            scaleValue += 0.002f;
            transform.localScale = Vector3.one * scaleValue; //크기 커지게
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration); //회전 방향(점점 가속하기 때문에 Acceleration
            //while문에서는 꼭 yield return null 써줘야함
            yield return null;
        }
    }
}
