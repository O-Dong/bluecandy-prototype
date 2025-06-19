using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class NameInputHandler : MonoBehaviour
{
    public TMP_InputField nameInputField;

    [Header("사운드")]
    public AudioSource submitSound;
    public AudioSource bgmSound;

    private string playerNameToSave;

    void Start()
    {
        // BGM 시작
        if (bgmSound != null)
        {
            bgmSound.loop = true;
            bgmSound.Play();
        }
    }

    public void OnSubmit()
    {
        string name = nameInputField.text;

        if (!string.IsNullOrEmpty(name))
        {
            playerNameToSave = name;

            if (submitSound != null)
                submitSound.Play();

            Invoke("LoadNextScene", 0.5f);
        }
        else
        {
            Debug.Log("이름이 비어있습니다!");
        }
    }

    void LoadNextScene()
    {
        GlobalGameManager.Instance.playerName = playerNameToSave;
        SceneManager.LoadScene("DialogueScene");
    }
}
