using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    // Text UI 항목 연결을 위한 변수
    public Text txtScore;
    // 누적 점수를 기록하기 위한 변수
    private int totScore = 0;

    void Start()
    {
        //totScore = PlayerPrefs.GetInt("TOT_SCORE", 0); // 이거랑

        DispScore(0);
    }

    public void DispScore(int score)
    {
        totScore += score;
        txtScore.text = "score <color=#ff0000>" + totScore.ToString() + "</color>";
        if (totScore > 200)
        {
            // 승리 화면으로 전환 ( 씬 전환)
            SceneManager.LoadScene("scWin");
        }

        //PlayerPrefs.SetInt("TOT_SCORE", totScore); // 이거 없애면 점수가 누적되지 않고 실행시 점수 0점으로 초기화됌
    }

    void Update()
    {
        
    }
}
