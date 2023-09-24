using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class GameMgr : MonoBehaviour
{
    #region 멤버변수
    public Transform[] points;
    public GameObject monsterPrefab;
    public float createTime = 2.0f;
    public int maxMonster = 10;
    public bool isGameOver = false;

    // 싱글턴 패턴을 위한 인스턴스 함수
    public static GameMgr instance = null;

    public List<GameObject> monsterPool = new List<GameObject>();

    public float sfxVolume = 1.0f;  // 사운드 볼륨 설정 변수
    public bool isSfxMute = false; // 사운드의 뮤트 기능
    #endregion

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // 하이라키 뷰의 스판포인트를 차아 하위에 있는 모든 Trasnform 컴포넌트를 찾아옴;
        points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();

        for(int i = 0; i<maxMonster; i++)
        {
            GameObject monster = (GameObject)Instantiate(monsterPrefab);
            monster.name = "MONSTER_" + i.ToString();
            monster.SetActive(false);
            monsterPool.Add(monster);
        }

        if (points.Length > 0)
        {
            // 몬스터 생성 코루틴 함수 호출
            StartCoroutine(this.CreateMonster());
        }
    }

    IEnumerator CreateMonster()
    {
        // 게임 종료시까지 무한 루프
        while (!isGameOver)
        {
            /*
            // 현재 생성된 몬스터 개수 산출
            int monsterCount = (int)GameObject.FindGameObjectsWithTag("MONSTER").Length;

            // 몬스터의 최대 생성 개수보다 작을 때만 몬스터 생성
            if (monsterCount < maxMonster)
            {
                yield return new WaitForSeconds(createTime);
                int idx = Random.Range(1, points.Length);
                Instantiate(monsterPrefab, points[idx].position, points[idx].rotation);
            }
            else
            {
                yield return null;
            }
            */

            yield return new WaitForSeconds(createTime);

            if (isGameOver) yield break;

            foreach(GameObject monster in monsterPool)
            {
                // 비활성화 여부러 사용 가능한 몬스터 판단
                if(!monster.activeSelf)
                {
                    // 몬스터를 출현시킬 위치 인덱스 값 추출
                    int idx = Random.Range(1, points.Length);
                    // 몬스터의 출현 위치 설정
                    monster.transform.position = points[idx].position;
                    // 몬스터 활성화
                    monster.SetActive(true);
                    // 오브젝트 풀에서 몬스터 프리팹 하나를 활성화한 후 for 루프 빠져나감
                    break;

                }
            }

        }
    }

    public void PlaySfx(Vector3 pos, AudioClip sfx)
    {
        // 음소거 옵션 설정시 바로 빠져나감
        if (isSfxMute) return;
        // 게임 오브젝트를 동적으로 생성
        GameObject soundObj = new GameObject("Sfx");
        soundObj.transform.position = pos;  // 사운드 발생 위치 저장
        // 생성한 게임 오브젝트에 오디오소스 컴포넌트 추가
        AudioSource audioSource = soundObj.AddComponent<AudioSource>();
        // AudioSource 속성 지정
        audioSource.clip = sfx;
        audioSource.minDistance = 10.0f;
        audioSource.maxDistance = 30.0f;
        audioSource.volume = sfxVolume;
        audioSource.Play();

        Destroy(soundObj, sfx.length);
    }

    void Update()
    {

    }
}
