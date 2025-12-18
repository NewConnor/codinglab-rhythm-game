using System.Collections.Generic;
using UnityEngine;


public class JudgeManager : MonoBehaviour
{
    public static JudgeManager Instance;
    public GameObject judgeEffectManager;

    private JudgeEffectManager judem;
    public NoteJudgePreset noteJudgePreset;

    public ScoreManager scoreManager;

    public AudioSource hitAudioSource;
    public float minPitch = 0.95f;
    public float maxPitch = 1.05f;

    // 롱노트 시작 시간을 기록하기 위한 배열
    private float[] longNoteStartTimes = new float[4];
    private NoteJudger[] activeLongNotes = new NoteJudger[4];

    public float noteDeadOffset = 12f;
    public float judgeEffectYOffset = 0f;

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

        if (hitAudioSource == null)
        {
            hitAudioSource = GetComponent<AudioSource>();
        }
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
            KeyCode key = GetKeyForLine(line);

            // ------------------ 판정 파트 ------------------
            
            // 1. 키를 누르는 순간
            if (Input.GetKeyDown(key))
            {
                NoteJudger closest = GetClosestNoteInLine(line);
                if (closest != null && !closest.judged)
                {
                    int type = closest.GetNoteType();
                    if (type == 0) // 단노트: 기존 거리 판정
                    {
                        NoteJudge(closest, line);
                    }
                    else // 롱노트: 누르기 시작 (시간 기록)
                    {
                        activeLongNotes[line] = closest;
                        longNoteStartTimes[line] = Time.time;
                        PlayHitSound();
                    }
                }
            }

            // 2. 키를 떼는 순간 (롱노트 시간 검사)
            if (Input.GetKeyUp(key))
            {
                if (activeLongNotes[line] != null)
                {
                    float pressDuration = Time.time - longNoteStartTimes[line];
                    int type = activeLongNotes[line].GetNoteType();
                    string judgement = "miss";

                    if (type == 1) // 4박자
                    {
                        judgement = (pressDuration >= 0.4f) ? "perfect" : "miss";
                    }
                    else if (type == 2) // 8박자
                    {
                        judgement = (pressDuration >= 0.8f) ? "perfect" : "miss";
                    }

                    FinalizeLongNote(activeLongNotes[line], line, judgement);
                    activeLongNotes[line] = null;
                }
            }

            // 3. 지나간 노트 및 처리 안 된 롱노트 미스 처리
            for (int i = notePool[line].Count - 1; i >= 0; i--)
            {
                NoteJudger note = notePool[line][i];
                if (!note.judged && note.transform.position.y <= -noteDeadOffset)
                {
                    if (activeLongNotes[line] == note) activeLongNotes[line] = null;
                    
                    Debug.Log($"Miss Line {line}");
                    note.judged = true;
                    if (scoreManager != null) scoreManager.ProcessJudgement("miss");
                    Vector3 judgeEffectPosition = new Vector3(note.transform.position.x, judgeEffectYOffset, note.transform.position.z);
                    judem.ShowJudgeEffect("miss", judgeEffectPosition);
                    Destroy(note.gameObject);
                    notePool[line].RemoveAt(i);
                }
            }
        }
    }

    private void FinalizeLongNote(NoteJudger note, int line, string judgement)
    {
        if (scoreManager != null) scoreManager.ProcessJudgement(judgement);
        Vector3 judgeEffectPosition = new Vector3(note.transform.position.x, judgeEffectYOffset, note.transform.position.z);
        judem.ShowJudgeEffect(judgement, judgeEffectPosition);
        
        note.judged = true;
        Destroy(note.gameObject);
    }

    private void PlayHitSound()
    {
        if (hitAudioSource != null)
        {
            hitAudioSource.pitch = Random.Range(minPitch, maxPitch);
            hitAudioSource.PlayOneShot(hitAudioSource.clip);
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
        float distance = closest.DistanceToJudgeLine() + noteJudgePreset.rangeOffset;
        Vector3 position = new Vector3(closest.transform.position.x, closest.transform.position.y + noteJudgePreset.rangeOffset, closest.transform.position.z);
        string judgement = "";
        int noteType = closest.GetNoteType();

        switch (noteType)
        {
            case 0: // 1박자 노트
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
                break;
            
        }

        if (judgement != "miss" && hitAudioSource != null)
        {
            PlayHitSound();
        }

        if (scoreManager != null)
        {
            scoreManager.ProcessJudgement(judgement);
        }

        Vector3 judgeEffectPosition = new Vector3(position.x, judgeEffectYOffset, position.z);
        judem.ShowJudgeEffect(judgement, judgeEffectPosition);

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