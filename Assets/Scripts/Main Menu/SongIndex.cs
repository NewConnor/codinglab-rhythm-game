using System.Collections.Generic;
using UnityEngine;
public class SongIndex : MonoBehaviour
{
    public static SongIndex Instance;
    public int songIndex;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }
}