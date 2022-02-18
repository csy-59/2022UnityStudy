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
    public BoxCollider meleeArea; //�ٰŸ� ����
    public TrailRenderer trailEffect; //���ݽ� ����Ʈ

    public void Use()
    {
        if(type == Type.Melee)
        {
            StopCoroutine("Swing");//�ڷ�ƾ ����. ȣ�� ���� �ҷ��ͼ� ������ ������ �ʵ��� ��
            StartCoroutine("Swing");//�ڷ�ƾ ȣ��
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

    //Use() ���� ��ƾ���� SWing() ���� ��ƾ���� ���� �� ��ƾ�� ������ Use ���η�ƾ���� ���ư�
    //�ڷ�ƾ�� �̿� �޸� ���� ��ƾ�� ���� ��ƾ(���⿡���� �ڷ�ƾ)�� ���ÿ� ����ȴ�.
}
