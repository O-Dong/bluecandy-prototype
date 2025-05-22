using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipCutscene : MonoBehaviour
{
    [Header("이동할 다음 씬 이름")]
    public string nextSceneName = "Talk1"; // ← 기본값은 Talk1, 인스펙터에서 바꿀 수 있음

    public void Skip()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
