using UnityEngine;

public class NoteJudger : MonoBehaviour
{
    public int lineIndex;
    public float targetY = -3f;

    // 아래 pefectRange와 goodRange는 실제 판정 결과에 사용되는 상수입니다.
    // 수정이 필요할 시, 인스펙터에서 수정하는 것을 권장합니다.
    public float perfectRange = 0.3f;
    public float goodRange = 0.7f;

    public KeyCode assignedKey;
    public bool judged = false;

    void Start()
    {
        JudgeManager.Instance.RegisterNote(this);
    }

    void OnDestroy()
    {
        if (JudgeManager.Instance != null)
            JudgeManager.Instance.UnregisterNote(this);
    }

    public float DistanceToJudgeLine()
    {
        return Mathf.Abs(transform.position.y - targetY);
    }
}
