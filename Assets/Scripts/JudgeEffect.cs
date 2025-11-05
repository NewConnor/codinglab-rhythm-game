using UnityEngine;
using TMPro;

public class JudgeEffect : MonoBehaviour
{
    public TextMeshProUGUI judgementText;
    public float fadeSpeed = 2f;

    void Start()
    {
        if (judgementText == null)
            judgementText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        // 페이드아웃만 (위로 이동 코드 삭제)
        if (judgementText != null)
        {
            Color color = judgementText.color;
            color.a -= fadeSpeed * Time.deltaTime;
            judgementText.color = color;
        }
    }
}