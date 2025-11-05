using System.Collections.Generic;
using UnityEngine;


public class JudgeManager : MonoBehaviour
{
    public static JudgeManager Instance;
    public GameObject judgeEffectManager;

    private JudgeEffectManager judem;
    public NoteJudgePreset noteJudgePreset;

    // 라인별로 노트들을 관리하는 딕셔너리
    private Dictionary<int, List<NoteJudger>> notePool = new Dictionary<int, List<NoteJudger>>();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < 4; i++)
        {
            notePool[i] = new List<NoteJudger>();
        }

        judem = judgeEffectManager.GetComponent<JudgeEffectManager>();
    }

    public void RegisterNote(NoteJudger note)
    {
        if (!notePool.ContainsKey(note.lineIndex)) return;
        notePool[note.lineIndex].Add(note);
    }

    public void UnregisterNote(NoteJudger note)
    {
        if (!notePool.ContainsKey(note.lineIndex)) return;
        notePool[note.lineIndex].Remove(note);
    }

    void Update()
    {
        for (int line = 0; line < 4; line++)
        {
            if (notePool[line].Count == 0) continue;

            NoteJudger closest = GetClosestNoteInLine(line);
            if (closest == null || closest.judged) continue;

            KeyCode key = GetKeyForLine(line);

            // ------------------ 판정 파트 ------------------
            if (Input.GetKeyDown(key))
            {
                NoteJudge(closest, line);
            }
            for (int i = notePool[line].Count - 1; i >= 0; i--)
            {
                NoteJudger note = notePool[line][i];
                if (!note.judged && note.transform.position.y <= -4f)
                {
                    Debug.Log($"Miss Line {line}");
                    note.judged = true;
                    Destroy(note.gameObject);
                    notePool[line].RemoveAt(i);
                }

            }
        }
    }

    // 가장 가까운 노트를 반환
    NoteJudger GetClosestNoteInLine(int line)
    {
        float minDistance = float.MaxValue;
        NoteJudger closest = null;

        foreach (NoteJudger note in notePool[line])
        {
            if (note.judged) continue;

            float dist = note.DistanceToJudgeLine();
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = note;
            }
        }

        return closest;
    }

    // 거리를 판단하여 판단 결과를 출력하는 파트를 따로 함수로 작성하였습니다.
    // 판단 결과에 따라 실행되는 특정 코드는 아래 NoteJudge 함수에서 처리됩니다.
    // 판단 결과에 따른 이펙트는 ShowJudgeEffect 함수에서 처리됩니다.
    private void NoteJudge(NoteJudger closest, int line)
    {
        float distance = closest.DistanceToJudgeLine();
        string judgement = "";

        if (distance <= noteJudgePreset.perfectRange)
        {
            judgement = "perfect";
        }
        else if (distance <= noteJudgePreset.goodRange)
        {
            judgement = "good";
        }
        else
        {
            judgement = "miss";
        }

        judem.ShowJudgeEffect(judgement, closest.transform.position);

        closest.judged = true;
        Destroy(closest.gameObject);
    }

    // 라인 번호에 따라 키 매핑
    KeyCode GetKeyForLine(int line)
    {
        switch (line)
        {
            case 0: return KeyCode.S;
            case 1: return KeyCode.D;
            case 2: return KeyCode.K;
            case 3: return KeyCode.L;
            default: return KeyCode.None;
        }
    }
}
