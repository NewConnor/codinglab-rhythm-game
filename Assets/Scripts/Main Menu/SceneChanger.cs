using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void Song_1()
    {
        SongIndex.Instance.songIndex = 0;
        SceneManager.LoadScene("Game");
    }

    public void Song_2()
    {
        SongIndex.Instance.songIndex = 1;
        SceneManager.LoadScene("Game");
    }

    public void Song_3()
    {
        SongIndex.Instance.songIndex = 2;
        SceneManager.LoadScene("Game");
    }

    public void Song_4()
    {
        SongIndex.Instance.songIndex = 3;
        SceneManager.LoadScene("Game");
    }
}
