using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    //열거형 타입: 아이템 타입 이름(변수 아님)
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon};
    //변수
    public Type type;
    public int value;

    private void Update()
    {
        //아이템 효과
        transform.Rotate(Vector3.up * 30 * Time.deltaTime);//돌기

    }
}
