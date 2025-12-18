using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // 씬 전환을 위해 추가
using TMPro; // 텍스트 제어를 위해 추가
using System.Collections; // 코루틴을 위해 추가

public class BeatNoteSpawner : MonoBehaviour
{
    public string[] notelist;
    public AudioSource[] musicList;
    public GameObject[] prefabList;
    public Transform[] lines;

    public float spawnY = 6f;
    public float targetY = -3f;
    public float noteSpeed = 5f;

    public List<BeatNote> beatNotes = new List<BeatNote>();
    public KeyCode[] lineKeys = new KeyCode[4] { KeyCode.S, KeyCode.D, KeyCode.K, KeyCode.L };

    private float noteOffsetTime;
    private int nextNoteIndex = 0;
    public static BeatNoteSpawner Instance;

    public TMP_Text resultText;
    private bool isGameEnded = false; 

    public AudioSource victory;

    void Awake()
    {
        Instance = this;

        if (lineKeys == null || lineKeys.Length < 4)
        {
            lineKeys = new KeyCode[4] { KeyCode.S, KeyCode.D, KeyCode.K, KeyCode.L };
        }

        if (resultText != null) resultText.gameObject.SetActive(false); // 시작할 땐 끄기
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

        if (musicList[SongIndex.Instance.songIndex].clip == null)
        {
            Debug.LogWarning("Audio가 없습니다.");
            return;
        }
        LoadNotesFromJson(notelist[SongIndex.Instance.songIndex]);
        musicList[SongIndex.Instance.songIndex].Play();
    }

    void Update()
    {
        AudioSource currentMusic = musicList[SongIndex.Instance.songIndex];
        float currentTime = currentMusic.time;

        // 게임 종료 체크: 노래가 끝났고, 아직 종료 처리를 안 했을 때
        if (!isGameEnded && !currentMusic.isPlaying && currentMusic.time >= currentMusic.clip.length * 0.95f)
        {
            StartCoroutine(EndGameSequence());
        }

        while (nextNoteIndex < beatNotes.Count &&
               beatNotes[nextNoteIndex].time <= currentTime + noteOffsetTime)
        {
            int noteType = 0; 
            int lineIndex = 0;
            if (nextNoteIndex >= 0 && nextNoteIndex < beatNotes.Count && beatNotes[nextNoteIndex] != null)
            {
                lineIndex = beatNotes[nextNoteIndex].line;
                noteType = beatNotes[nextNoteIndex].type;
            }


            if (lineIndex >= 0 && lineIndex < lines.Length)
            {
                Vector3 spawnPos = new Vector3(
                    lines[lineIndex].position.x,
                    spawnY,
                    0);

                GameObject note = Instantiate(prefabList[noteType], spawnPos, Quaternion.identity);
                note.GetComponent<NoteJudger>().SetType(noteType);

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

    // --- 추가된 종료 시퀀스 ---
    IEnumerator EndGameSequence()
    {
        isGameEnded = true;
        victory.Play();

        if (resultText != null)
        {
            resultText.text = "YOU DID IT!\n";
            resultText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(3.0f); // 3초 대기

        SceneManager.LoadScene("MainMenu"); // 메인메뉴 씬으로 이동
    }
}