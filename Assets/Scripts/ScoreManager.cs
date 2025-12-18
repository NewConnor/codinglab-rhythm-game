using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // 1. SceneManager 사용을 위해 필요
using System.Collections;         // 2. IEnumerator 사용을 위해 필요

public class ScoreManager : MonoBehaviour
{
    [Header("UI 연결")]
    public TMP_Text scoreText;      
    public TMP_Text comboText;      
    public TMP_Text resultText;

    private int currentScore = 0;
    private int comboCount = 0;
    private int missCount = 0;
    private bool isGameEnded = false; 

    public AudioSource fail;

    void Start()
    {
        if (resultText != null) resultText.gameObject.SetActive(false);
        UpdateUI();
    }

    public void ProcessJudgement(string judgement)
    {
        if (isGameEnded) return; // 게임이 끝났다면 판정 처리를 무시

        switch (judgement.ToLower())
        {
            case "perfect":
                if (comboCount >= 10) { currentScore += 5; }
                else if (comboCount >= 5)
                {
                    currentScore += 3;
                    if (missCount >= 1) {missCount--;}
                }
                else { currentScore += 2; }
                comboCount++;
                break;

            case "good":
                currentScore += 1;
                comboCount = 0;
                break;

            case "miss":
                comboCount = 0;
                missCount++;
                if (missCount >= 5 && !isGameEnded) 
                {
                    StartCoroutine(EndGameSequence());
                }
                break;
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null) scoreText.text = currentScore.ToString();

        if (comboText != null)
        {
            if (comboCount > 0) comboText.text = $"COMBO x {comboCount}";
            else comboText.text = "";
        }
    }

    IEnumerator EndGameSequence()
    {
        isGameEnded = true;
        fail.Play();

        if (resultText != null)
        {
            resultText.text = "FAIL";
            resultText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(3.0f);

        SceneManager.LoadScene("MainMenu");
    }
}