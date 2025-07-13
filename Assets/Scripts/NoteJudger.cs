using UnityEngine;

public class NoteJudger : MonoBehaviour
{
    public int lineIndex;
    public float targetY = -3f;
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
