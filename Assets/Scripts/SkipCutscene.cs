using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class SkipCutscene : MonoBehaviour
{
    [Header("이동할 다음 씬 이름 (씬마다 설정!)")]
    public string nextSceneName = "Talk1"; // Inspector에서 Talk3 등으로 바꿔서 사용

    private VideoPlayer videoPlayer;
    private bool isSkipped = false;

    void Start()
    {
        // VideoPlayer 자동으로 찾기 (같은 오브젝트 또는 씬 내 아무거나)
        videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer == null)
        {
            videoPlayer = FindObjectOfType<VideoPlayer>();
        }

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
        }
        else
        {
            Debug.LogError("[SkipCutscene] VideoPlayer를 찾을 수 없습니다.");
        }

        Debug.Log($"[SkipCutscene] 이동할 씬: {nextSceneName}");
    }

    public void Skip()
    {
        if (!isSkipped && !string.IsNullOrEmpty(nextSceneName))
        {
            isSkipped = true;
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        if (!isSkipped && !string.IsNullOrEmpty(nextSceneName))
        {
            isSkipped = true;
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
