using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMissile : Bullet
{
    //목표
    public Transform target;
    //목표를 추격하기 위한 NavMeshAgent
    NavMeshAgent nav;

    // Start is called before the first frame update
    void Awake()
    {
        //초기화
        nav = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        //미사일이 목표(캐릭터)를 추적할 수 있도록 설정
        nav.SetDestination(target.position);
    }
}
