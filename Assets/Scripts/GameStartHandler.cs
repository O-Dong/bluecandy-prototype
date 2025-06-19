using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartHandler : MonoBehaviour
{
    [Header("사운드")]
    public AudioSource clickSound;
    public AudioSource bgmSound;  // BGM용 추가

    void Start()
    {
        // 시작하자마자 BGM 재생
        if (bgmSound != null)
        {
            bgmSound.loop = true;
            bgmSound.Play();
        }
    }

    public void StartGame()
    {
        Debug.Log("StartGame 호출됨");

        if (clickSound != null)
            clickSound.Play();

        Invoke("LoadNextScene", 0.5f);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene("NameInputScene");
    }
}
