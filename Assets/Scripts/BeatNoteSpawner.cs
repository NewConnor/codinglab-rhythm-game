using UnityEngine;
using System.Collections.Generic;

public class BeatNoteSpawner : MonoBehaviour
{
    public AudioSource music;
    public GameObject notePrefab;
    public Transform[] lines;

    public float spawnY = 6f;
    public float targetY = -3f;
    public float noteSpeed = 5f;

    public List<BeatNote> beatNotes = new List<BeatNote>();
    public KeyCode[] lineKeys = new KeyCode[4] { KeyCode.S, KeyCode.D, KeyCode.K, KeyCode.L };

    private float noteOffsetTime;
    private int nextNoteIndex = 0;
    public static BeatNoteSpawner Instance;

    void Awake()
    {
        Instance = this;

        if (lineKeys == null || lineKeys.Length < 4)
        {
            lineKeys = new KeyCode[4] { KeyCode.S, KeyCode.D, KeyCode.K, KeyCode.L };
        }


    }


    public class BeatNoteListWrapper
    {
        public List<BeatNote> beatNotes;
    }

    public void LoadNotesFromJson(string filename)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("NoteData/" + filename);

        if (jsonFile == null)
        {
            Debug.LogError("JSON 파일을 찾을 수 없습니다: " + filename);
            return;
        }

        string json = jsonFile.text;
        beatNotes = JsonUtility.FromJson<BeatNoteListWrapper>("{\"beatNotes\":" + json + "}").beatNotes;
        Debug.Log($"불러오기 완료");
    }

    void Start()
    {
        noteOffsetTime = (spawnY - targetY) / noteSpeed;

        if (music.clip == null)
        {
            Debug.LogWarning("Audio가 없습니다.");
            return;
        }

        LoadNotesFromJson("song1");
        music.Play();
    }

    void Update()
    {
        float currentTime = music.time;

        while (nextNoteIndex < beatNotes.Count &&
               beatNotes[nextNoteIndex].time <= currentTime + noteOffsetTime)
        {
            int lineIndex = beatNotes[nextNoteIndex].line;

            if (lineIndex >= 0 && lineIndex < lines.Length)
            {
                Vector3 spawnPos = new Vector3(
                    lines[lineIndex].position.x,
                    spawnY,
                    0);

                GameObject note = Instantiate(notePrefab, spawnPos, Quaternion.identity);

                NoteJudger judger = note.GetComponent<NoteJudger>();
                if (judger != null)
                {
                    judger.lineIndex = lineIndex;
                    judger.targetY = targetY;

                    if (lineIndex < lineKeys.Length)
                        judger.assignedKey = lineKeys[lineIndex];
                    else
                        judger.assignedKey = KeyCode.None;
                }
            }

            nextNoteIndex++;
        }

    }
}
