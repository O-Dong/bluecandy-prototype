using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class NameInputHandler : MonoBehaviour
{
    public TMP_InputField nameInputField;

    public void OnSubmit()
    {
        string name = nameInputField.text;

        if (!string.IsNullOrEmpty(name))
        {
            GlobalGameManager.Instance.playerName = name;
            SceneManager.LoadScene("DialogueScene");
        }
        else
        {
            Debug.Log("이름이 비어있습니다!");
        }
    }
}
