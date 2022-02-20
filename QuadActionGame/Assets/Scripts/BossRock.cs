using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet
{
    //���� ��������
    Rigidbody rigid;
    float angularPower = 2; //ȸ�� �Ŀ�
    float scaleValue = 0.1f; //�� ũ��
    bool isShoot; //��� �ִ���

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTimer());
        StartCoroutine(GainPower());
    }

    IEnumerator GainPowerTimer()
    {
        //���� ��������� �⸦ ������ ����
        yield return new WaitForSeconds(2.2f);
        isShoot = true;
    }

    IEnumerator GainPower()
    {
        while (!isShoot)
        {
            //�⸦ ������
            angularPower += 0.2f;
            scaleValue += 0.002f;
            transform.localScale = Vector3.one * scaleValue; //ũ�� Ŀ����
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration); //ȸ�� ����(���� �����ϱ� ������ Acceleration
            //while�������� �� yield return null �������
            yield return null;
        }
    }
}
