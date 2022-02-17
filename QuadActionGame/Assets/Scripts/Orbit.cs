using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform target; //따라다니는 타겟
    public float orbitSpeed; //회전 속도
    Vector3 offset; //패딩

    // Start is called before the first frame update
    void Start()
    {
        //타겟과 자신의 거리
        offset = transform.position - target.position;
    }

    // Update is called once per frame
    void Update()
    {
        //타겟과의 거리를 유지하기 위해
        transform.position = target.position + offset;
        //타겟 주위를 회전하는 함수(타겟의 위치, 회전 축, 회전 수치)
        transform.RotateAround(target.position, Vector3.up, orbitSpeed * Time.deltaTime);
        //타겟과의 거리를 유지하기 위해
        offset = transform.position - target.position;
    }
}
