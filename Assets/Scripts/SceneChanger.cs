using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void Song1()
    {
        PlayerPrefs.SetInt("SongIndex", 1);
        SceneManager.LoadScene("Game");
    }

    public void Song2()
    {
        PlayerPrefs.SetInt("SongIndex", 2);
        SceneManager.LoadScene("Game");
    }

    public void Song3()
    {
        PlayerPrefs.SetInt("SongIndex", 3);
        SceneManager.LoadScene("Game");
    }

    public void Song4()
    {
        PlayerPrefs.SetInt("SongIndex", 4);
        SceneManager.LoadScene("Game");
    }
}
