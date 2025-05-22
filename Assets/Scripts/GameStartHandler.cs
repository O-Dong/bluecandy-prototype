using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartHandler : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("StartGame 호출됨");
        SceneManager.LoadScene("NameInputScene");
    }
}
