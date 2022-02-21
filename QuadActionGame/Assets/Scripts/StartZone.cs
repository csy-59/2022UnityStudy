using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartZone : MonoBehaviour
{
    public GameManager manager;

    private void OnTriggerEnter(Collider other)
    {
        //시작하기 존에 밟으면 
        if(other.gameObject.tag == "Player")
        {
            //스테이지 시작
            manager.StageStart();
        }
    }
}
