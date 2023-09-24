using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    // 폭발 효과 파티클 연결 변수
    public GameObject expEffect;
    private Transform tr;
    public Texture[] textures;

    // 총알 맞은 횟수를 누적시킬 변수
    private int hitCount = 0;

    void Start()
    {
        tr = GetComponent<Transform>();

        int idx = Random.Range(0, textures.Length);
        GetComponentInChildren<MeshRenderer>().material.mainTexture = textures[idx];    // 드럼통 색 랜덤으로 변환
    }

    // 충돌시 발생하는 콜백함수 (CallBack Function)
    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.tag == "BULLET")  // coll.GameObject.tag가 더 상위이며 이것도 사용 가능, 하지만 collider는 좀더 정확하고 빠름
        {
            // 충돌한 총알 제거
            Destroy(coll.gameObject);
            // 총알 맞은 횟수를 증가시키고 3회 이상이면 폭발처리
            if(++hitCount >= 3)
            {
                ExpBarrel();
            }
        }
    }

    void ExpBarrel()
    {
        // 폭발 효과 파티클 생성
        Instantiate(expEffect, tr.position, Quaternion.identity);
        //지정한 원점을 중심으로 10.0f 반경 내에 들어와 있는 Collider 객체 추출
        Collider[] colls = Physics.OverlapSphere(tr.position, 10.0f);

        // 추출한 Collider 객체에 폭발력 전달
        foreach(Collider coll in colls)
        {
            Rigidbody rbody = coll.GetComponent<Rigidbody>();
            if(rbody != null)
            {
                rbody.mass = 1.0f;
                rbody.AddExplosionForce(1000.0f, tr.position, 10.0f, 300.0f);
            }
        }
        // 5초후 드럼통 제거
        Destroy(gameObject, 5.0f);
    }

    void Update()
    {
        
    }
}
