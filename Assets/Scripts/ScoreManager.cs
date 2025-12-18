using UnityEngine;
using TMPro; // TextMeshPro 사용

public class ScoreManager : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI scoreText;      // 점수 표시용 (예: 1250)
    public TextMeshProUGUI comboText;      // 콤보 표시용 (예: COMBO x 10)

    private int currentScore = 0;
    private int comboCount = 0;

    void Start()
    {
        UpdateUI();
    }

    // 외부에서 판정 결과를 매개변수로 호출 (예: ProcessJudgement("perfect"))
    public void ProcessJudgement(string judgement)
    {
        switch (judgement.ToLower())
        {
            case "perfect":
                currentScore += 2;
                comboCount++;
                break;

            case "good":
                currentScore += 1;
                comboCount++;
                break;

            case "miss":
                // 점수 가산 없음
                comboCount = 0; // 콤보 초기화
                break;
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        // 점수 텍스트 업데이트
        scoreText.text = currentScore.ToString();

        // 콤보가 있을 때만 표시 (0콤보일 땐 숨김)
        if (comboCount > 0)
        {
            comboText.text = $"COMBO x {comboCount}";
        }
        else
        {
            comboText.text = ""; 
        }
    }
}