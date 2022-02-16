using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    
    public Transform target; //카메라가 따라가는 대상
    public Vector3 offset; // 카메라의 패딩...?

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + offset;
    }
}
