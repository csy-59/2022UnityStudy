using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    
    public Transform target; //ī�޶� ���󰡴� ���
    public Vector3 offset; // ī�޶��� �е�...?

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + offset;
    }
}
