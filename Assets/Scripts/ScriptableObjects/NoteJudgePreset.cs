using UnityEngine;

[CreateAssetMenu(fileName = "NoteJudgePreset", menuName = "Scriptable Objects/NoteJudgePreset")]
public class NoteJudgePreset : ScriptableObject
{
    public float perfectRange = 0.3f;
    public float goodRange = 0.7f;
    public float rangeOffset = 0.0f;
}
