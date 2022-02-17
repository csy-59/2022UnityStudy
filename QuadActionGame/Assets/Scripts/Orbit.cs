using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform target; //����ٴϴ� Ÿ��
    public float orbitSpeed; //ȸ�� �ӵ�
    Vector3 offset; //�е�

    // Start is called before the first frame update
    void Start()
    {
        //Ÿ�ٰ� �ڽ��� �Ÿ�
        offset = transform.position - target.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Ÿ�ٰ��� �Ÿ��� �����ϱ� ����
        transform.position = target.position + offset;
        //Ÿ�� ������ ȸ���ϴ� �Լ�(Ÿ���� ��ġ, ȸ�� ��, ȸ�� ��ġ)
        transform.RotateAround(target.position, Vector3.up, orbitSpeed * Time.deltaTime);
        //Ÿ�ٰ��� �Ÿ��� �����ϱ� ����
        offset = transform.position - target.position;
    }
}
