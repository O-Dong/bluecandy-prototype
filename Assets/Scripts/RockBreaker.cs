using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class RockBreaker : MonoBehaviour
{
    public int rockHP = 10;

    [Header("오브젝트 연결")]
    public GameObject rockObject;
    public GameObject trappedKkuyo;
    public GameObject freedKkuyo;

    [Header("UI 요소")]
    public SpriteRenderer hpNumberRenderer;
    public Sprite[] numberSprites; // index 0 ~ 10 순서로 넣기
    public TextMeshProUGUI messageText;

    [Header("카메라 진동")]
    public CameraShake cameraShake;

    [Header("설정")]
    public float nextSceneDelay = 2f;
    public string nextSceneName = "Talk2"; // 다음 씬 이름

    void Start()
    {
        UpdateNumberSprite();

        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (rockHP > 0 && Input.GetKeyDown(KeyCode.Space))
        {
            rockHP--;
            UpdateNumberSprite();

            if (cameraShake != null)
                cameraShake.Shake();

            if (rockHP <= 0)
                BreakRock();
        }
    }

    void UpdateNumberSprite()
    {
        if (rockHP >= 0 && rockHP < numberSprites.Length)
        {
            hpNumberRenderer.sprite = numberSprites[rockHP];
        }
    }

    void BreakRock()
    {
        rockObject.SetActive(false);
        trappedKkuyo.SetActive(false);
        freedKkuyo.SetActive(true);

        if (messageText != null)
        {
            messageText.text = "고마워! 살았다구!";
            messageText.gameObject.SetActive(true);
        }

        Invoke("GoToNextScene", nextSceneDelay);
    }

    void GoToNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
