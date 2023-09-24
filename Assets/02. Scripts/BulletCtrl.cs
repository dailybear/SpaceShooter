using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    public int damage = 20;  // 총알 파괴력
    public float speed = 1000.0f;  // 총알 발사속도

    void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * speed);
    }

}
