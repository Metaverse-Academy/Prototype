using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CutscenePlayer : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string nextSceneName;
    public GameObject backgroundMusicObject;

    void Start()
    {
    }

    public void PlayVideo()
    {
        videoPlayer.Play();
        backgroundMusicObject.SetActive(false);
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        backgroundMusicObject.SetActive(true);
        SceneManager.LoadScene(nextSceneName);
    }
}
