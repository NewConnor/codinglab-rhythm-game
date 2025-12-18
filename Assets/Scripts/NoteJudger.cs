using UnityEngine;

public class NoteJudger : MonoBehaviour
{
    public int lineIndex;
    public float targetY = -3f;

    public KeyCode assignedKey;
    private int type;
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

    public void SetType(int noteType)
    {
        type = noteType;
    }
    public int GetNoteType() {return type;}
}
