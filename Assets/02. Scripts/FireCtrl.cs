using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(AudioSource))]
[System.Serializable]

public struct PlayerSfx
{
    public AudioClip[] fire;
    public AudioClip[] reload;
}

public class FireCtrl : MonoBehaviour
{
    public GameObject bullet;
    public Transform firePos;

    // 사운드
    public AudioClip fireSfx;
    private AudioSource source = null;

    // 총구 화염
    public MeshRenderer muzzleFlash;


    // 탄창 이미지 
    public Image magazineImg;
    // 남은 총알 수 Text
    public Text magazineText;

    // 최대 총알 수
    public int maxBullet = 10;
    //남은 총알 수
    public int remainingBullet = 10;

    // 재장전 시간
    public float reloadTime = 2.0f;
    //재장전 여부를 판단할 변수
    private bool isReloading = false;


    void Start()
    {
        source = GetComponent<AudioSource>();
        muzzleFlash.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }

        // 마우스 왼쪽 버튼 클릭시 Fire함수 호출
        if (!isReloading && Input.GetMouseButtonDown(0))
        {
            //총알 수 하나 감수
            --remainingBullet;

        }

        //남은 총알이 없을 경우 재장전 코루틴 호출
        if (remainingBullet == 0)
        {
            StartCoroutine(Reloading());
        }
    }

    void Fire()
    {
        CreateBullet();
        // 사운드 발생 함수
        //source.PlayOneShot(fireSfx, 0.9f);
        GameMgr.instance.PlaySfx(firePos.position, fireSfx);

        StartCoroutine(this.ShowMuzzleFlash());

        //재장전 이미지 fillAmount 속성값 지정
        magazineImg.fillAmount = (float)remainingBullet / (float)maxBullet;
        UpdateBulletText();
    }

    IEnumerator Reloading()
    {
        isReloading = true;
        //_audio.PlayOnOneShot(playerSfx.reload[(int)currWeapon], 1.0f);

        // 재장전 오디오의 길이 +0.3초 동안 대기
        yield return new WaitForSeconds(0.3f);

        // 각종 변숫값 초기화
        isReloading = false;
        magazineImg.fillAmount = 1.0f;
        remainingBullet = maxBullet;
        // 남은 총알 수 갱신
        UpdateBulletText();
    }

    void UpdateBulletText()
    {
        // (남은 총알 수 / 최대 총알 수) 표시
        magazineText.text = string.Format("<color=#ff0000>{0}</color>/{1}", remainingBullet, maxBullet);
    }

    IEnumerator ShowMuzzleFlash()
    {
        float scale = Random.Range(1.0f, 2.0f);
        muzzleFlash.transform.localScale = Vector2.one * scale;

        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360));
        muzzleFlash.transform.localRotation = rot;

        muzzleFlash.enabled = true;
        yield return new WaitForSeconds(Random.Range(0.05f, 0.3f)) ;
        muzzleFlash.enabled = false;
    }

    void CreateBullet()
    {
        Instantiate(bullet, firePos.position, firePos.rotation);
    }
}
