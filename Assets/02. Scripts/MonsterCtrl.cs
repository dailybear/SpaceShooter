using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MonsterCtrl : MonoBehaviour
{
    #region 멤버변수
    public enum MonsterState { idle, trace, attack, die };
    public MonsterState monsterState;
    //public MonsterState monsterState = MonsterState.idle;

    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent nvAgent;
    private Animator animator;

    public float traceDist = 10.0f;
    public float attackDist = 2.0f;
    private bool isDie = false;

    public GameObject bloodEffect;  // 상체
    public GameObject bloodDecal; // 바닥

    //생명게이지

    private float hp = 100.0f;
    private float initHp = 100.0f;
    // 생명게이지 프리팹을 저장할 변수
    public GameObject hpBarPrefab;
    // 생명 게이지의 위치를 보정할 오프셋
    public Vector3 hpBarOffset = new Vector3(0, 2.2f, 0);
    // 부모가 될 Canvas 객체
    private Canvas uiCanvas;
    // 생명 수치에 따라 fillAmount 속성을 변경할 이미지
    private Image hpBarImage;

    private GameUI gameUI;

    #endregion

    void Start ()
    {
        SetHpBar();
    }

    void SetHpBar()
    {
        uiCanvas = GameObject.Find("UICanvas").GetComponent<Canvas>();
        // UI Canvas 하위로 생명 게이지 생성
        GameObject hpBar = Instantiate<GameObject>(hpBarPrefab, uiCanvas.transform);
        // fillAmount 속성을 변경할 Image 추출
        hpBarImage = hpBar.GetComponentsInChildren<Image>()[1];

        // 생명게이지가 따라가야 할 대상과 오프셋 값 설정
        var _hpBar = hpBar.GetComponent<EnemyHpbar>();
        _hpBar.targetTr = this.gameObject.transform;
        _hpBar.offset = hpBarOffset;
    }

    void Awake()
    {
        monsterTr = this.gameObject.GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("PLAYER").GetComponent<Transform>();
        nvAgent = this.gameObject.GetComponent<NavMeshAgent>();
        animator = this.gameObject.GetComponent<Animator>();

        // GameUI 게임오브젝트의 GameUI 스크립트 할당
        gameUI = GameObject.Find("GameUI").GetComponent<GameUI>();

        monsterState = MonsterState.idle;

        // nvAgent.destination = playerTr.position;
    }

    void OnEnable()
    {
        PlayerCtrl.OnPlayerDie += this.OnPlayerDie;

        StartCoroutine(this.CheckMonsterState());
        StartCoroutine(this.MonsterAction());
    }

    void OnDisable()
    {
        PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
    }

    IEnumerator CheckMonsterState() 
    {
        while(!isDie)
        {
            yield return new WaitForSeconds(0.2f);

            float dist = Vector3.Distance(playerTr.position, monsterTr.position);

            if(dist <= attackDist)  // 거리에 따라 상태를 바꿔준다.
            {
                monsterState = MonsterState.attack;
            }
            else if (dist <= traceDist)
            {
                monsterState = MonsterState.trace;
            }
            else
            {
                monsterState = MonsterState.idle;
            }
        }
    }
    IEnumerator MonsterAction() 
    {
        while (!isDie) // 죽지않으면
        {
            switch(monsterState)
            {
                case MonsterState.idle:
                    nvAgent.Stop(); // 메쉬에이젼트컴포넌트에 대한 처리 :: 따라가기 멈춤
                    animator.SetBool("IsTrace", false);  // 애니메이션에 대한 처리 :: 따라가기를 멈춤
                    break;

                case MonsterState.trace:
                    nvAgent.destination = playerTr.position;
                    nvAgent.Resume();
                    animator.SetBool("IsAttack", false);
                    animator.SetBool("IsTrace", true);  
                    break;

                case MonsterState.attack:
                    nvAgent.Stop();
                    animator.SetBool("IsAttack", true);
                    break;
            }
            yield return null;
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject.tag == "BULLET")
        {
            CreateBloodEffect(coll.transform.position);

            hp -= coll.gameObject.GetComponent<BulletCtrl>().damage;

            // 생명 게이지의 fillAmount 속성을 변경
            hpBarImage.fillAmount = hp / initHp;

            if (hp <= 0)
            {
                MonsterDie();
                // 적 캐릭터가 사망한 이후 생명게이지를 투명 처리
                hpBarImage.GetComponentsInParent<Image>()[1].color = Color.clear;
            }

            Destroy(coll.gameObject);
            animator.SetTrigger("IsHit");

        }

    }



    void CreateBloodEffect(Vector3 pos)
    {
        // 상체
        GameObject blood1 = Instantiate(bloodEffect, pos, Quaternion.identity) as GameObject;   // == (GameObject)Instantiate(bloodEffect, pos, Quaternion.identity);
        Destroy(blood1, 2.0f);

        // 바닥에 뿌려지는 혈흔효과
        Vector3 decalPos = monsterTr.position + (Vector3.up * 0.05f);  // (0, 1, 0) * 0.05
        Quaternion decalRot = Quaternion.Euler(90, 0, Random.Range(0, 360));
        GameObject blood2 = Instantiate(bloodDecal, pos, decalRot) as GameObject;

        float scale = Random.Range(1.5f, 3.5f);
        blood2.transform.localScale = Vector3.one * scale;  // (1, 1, 1) // 로컬스케일은 항상 벡터 - > 벡터화 해야함 -> 항등벡터Vector3.one을 곱해줌

        Destroy(blood2, 5.0f);
    }

    void OnPlayerDie()
    {
        StopAllCoroutines();
        nvAgent.Stop();
        animator.SetTrigger("IsPlayerDie");
    }

    void MonsterDie()
    {
        // 사망한 몬스터의 태그를 Untagged로 변경
        gameObject.tag = "Untagged";

        StopAllCoroutines();
        isDie = true;
        monsterState = MonsterState.die;
        nvAgent.Stop();
        animator.SetTrigger("IsDie");

        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = false;

        foreach(Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = false;
        }

        // GameUI의 스코어 누적과 스코어 표시 함수 호출
        gameUI.DispScore(50);

        // 몬스터 오브젝트 풀로 환원시키는 코루틴 함수 호출
        StartCoroutine(this.PushObjectPool());
    }

    IEnumerator PushObjectPool()
    {
        yield return new WaitForSeconds(3.0f);

        isDie = false;
        hp = 100.0f;
        gameObject.tag = "MONSTER";
        monsterState = MonsterState.idle;

        // 몬스터에 추가된 Collider 다시 활성화
        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = true;

        foreach(Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = true;
        }

        // 몬스터 다시 비활성화
        gameObject.SetActive(false);
    }

    void Update()
    {
        
    }
}
