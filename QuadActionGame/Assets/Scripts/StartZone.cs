using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartZone : MonoBehaviour
{
    public GameManager manager;

    private void OnTriggerEnter(Collider other)
    {
        //�����ϱ� ���� ������ 
        if(other.gameObject.tag == "Player")
        {
            //�������� ����
            manager.StageStart();
        }
    }
}
