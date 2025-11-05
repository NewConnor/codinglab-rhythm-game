using UnityEngine;

public class JudgeEffectManager : MonoBehaviour
{
    public static JudgeEffectManager Instance;

    [Header("이펙트 프리팹")]
    public GameObject perfectEffectPrefab;
    public GameObject goodEffectPrefab;
    public GameObject missEffectPrefab;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // 판정 결과에 따라 이펙트 표시
    public void ShowJudgeEffect(string judgement, Vector3 position)
    {
        GameObject effectPrefab = null;

        switch (judgement.ToLower())
        {
            case "perfect":
                effectPrefab = perfectEffectPrefab;
                break;
            case "good":
                effectPrefab = goodEffectPrefab;
                break;
            case "miss":
                effectPrefab = missEffectPrefab;
                break;
        }

        if (effectPrefab != null)
        {
            // 이펙트 생성
            GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);
            // 1초 후 자동 삭제
            Destroy(effect, 1f);
        }
    }
}