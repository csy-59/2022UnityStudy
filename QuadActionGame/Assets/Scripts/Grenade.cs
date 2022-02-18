using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    //����ź ������Ʈ�� ���� ������
    public GameObject meshObject;
    public GameObject effectObject;
    public Rigidbody rigid;

    // Start is called before the first frame update
    void Start()
    {
        //���� �ڷ�ƾ ����
        StartCoroutine(Explosion());
    }

    IEnumerator Explosion()
    {
        //3�ʵ� �����ϰ�
        yield return new WaitForSeconds(3f);
        //������ �ӵ�, ȸ�� ��� �ʱ�ȭ
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        //���� �� ȿ���� �޽� ������Ʈ ����
        meshObject.SetActive(false);
        effectObject.SetActive(true);

        //�ֺ��� ������ ������ �ֱ�: laycast ���
        //�����̱� ������ �迭�� ����. SphereCastAll�� �ֺ� ��� ���� �� �޾ƿ�
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 15, Vector3.up, 0f, LayerMask.GetMask("Enemy"));
        foreach(RaycastHit hitObj in rayHits) //�ֺ��� ������ ��� ������ ����ź ���� �ֱ�
        {
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }

        Destroy(gameObject, 5);
    }
}
