using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Misslle : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        //스스로 공전
        transform.Rotate(Vector3.right * 30 * Time.deltaTime);
    }
}
