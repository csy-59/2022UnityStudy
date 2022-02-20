using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMissile : Bullet
{
    //��ǥ
    public Transform target;
    //��ǥ�� �߰��ϱ� ���� NavMeshAgent
    NavMeshAgent nav;

    // Start is called before the first frame update
    void Awake()
    {
        //�ʱ�ȭ
        nav = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        //�̻����� ��ǥ(ĳ����)�� ������ �� �ֵ��� ����
        nav.SetDestination(target.position);
    }
}
