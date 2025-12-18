using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("UI 연결")]
    public TMP_Text scoreText;      
    public TMP_Text comboText;      

    private int currentScore = 0;
    private int comboCount = 0;

    void Start()
    {
        UpdateUI();
    }

    public void ProcessJudgement(string judgement)
    {
        switch (judgement.ToLower())
        {
            case "perfect":
                currentScore += 2;
                comboCount++; // Perfect일 때만 콤보 증가
                break;

            case "good":
                currentScore += 1;
                comboCount = 0; // Good일 때도 콤보 초기화 (혹은 유지하고 싶다면 이 줄 삭제)
                break;

            case "miss":
                comboCount = 0; // Miss일 때 콤보 초기화
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
}