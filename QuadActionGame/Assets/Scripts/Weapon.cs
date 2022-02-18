using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //���� Ÿ��, ������, ����, ���� ȿ�� ���� ��
    public enum Type { Melee, Range}; //���� Ÿ�� ����
    public Type type;   //���� Ÿ��
    public int damage; //������
    public float rate; //����

    //�Ѿ� ����
    public int maxAmmo;
    public int curAmmo;

    public BoxCollider meleeArea; //�ٰŸ� ����
    public TrailRenderer trailEffect; //���ݽ� ����Ʈ
    public Transform bulletPos; //�Ѿ��� ������ ��ġ
    public GameObject bullet; //�Ѿ� �������� ������ ����
    public Transform bulletCasePos; //ź���� ������ ��ġ
    public GameObject bulletCase; //ź�� �������� ������ ����

    public void Use()
    {
        if(type == Type.Melee)
        {
            StopCoroutine("Swing");//�ڷ�ƾ ����. ȣ�� ���� �ҷ��ͼ� ������ ������ �ʵ��� ��
            StartCoroutine("Swing");//�ڷ�ƾ ȣ��
        }

        else if(type == Type.Range && curAmmo > 0) //�Ѿ� ������ ������
        {
            curAmmo--; //�Ѿ� �ϳ� ����
            StopCoroutine("Shot");//�ڷ�ƾ ����. ȣ�� ���� �ҷ��ͼ� ������ ������ �ʵ��� ��
            StartCoroutine("Shot");//�ڷ�ƾ ȣ��
        }
    }

    IEnumerator Swing()
    {
        //�ڷ�ƾ!!

        //1����: �ڵ� ����
        yield return new WaitForSeconds(0.1f);//0.1�� ���. yield�� ��� ����.
        meleeArea.enabled = true; //�ݶ��̴� ����
        trailEffect.enabled = true; //����Ʈ ����

        //2���� : �ڵ� ����
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false; //�ݶ��̴� ����


        //3���� : �ڵ� ����
        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false; //����Ʈ ����

        //4���� : �ڵ� ����
        //yield break; // �ڷ�ƾ ����. ���� �ؿ� �ִ� �ڵ�� ������� �ʴ´�.
    }

    IEnumerator Shot()
    {
        //�Ѿ� �߻�
        GameObject intantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);//�Ѿ� ����
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50; //�Ѿ� �ӵ�

        yield return null;

        //ź�� ����
        GameObject intantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);//�Ѿ� ����
        Rigidbody caseRigid = intantCase.GetComponent<Rigidbody>();
        //ź�ǿ� �ణ�� �ݵ��ֱ�. �ڷ� ����ϱ� ������ forward�� ������, �������� ���� ��
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse); //�ݵ� ����
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);//ȸ�� �ֱ�
    }

    //Use() ���� ��ƾ���� SWing() ���� ��ƾ���� ���� �� ��ƾ�� ������ Use ���η�ƾ���� ���ư�
    //�ڷ�ƾ�� �̿� �޸� ���� ��ƾ�� ���� ��ƾ(���⿡���� �ڷ�ƾ)�� ���ÿ� ����ȴ�.
}
