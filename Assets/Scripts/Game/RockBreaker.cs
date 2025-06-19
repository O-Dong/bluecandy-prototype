using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class RockBreaker : MonoBehaviour
{
    public int rockHP = 10;

    [Header("오브젝트 연결")]
    public GameObject rockObject;
    public GameObject freedKkuyo;

    [Header("UI 요소")]
    public SpriteRenderer hpNumberRenderer;
    public Sprite[] numberSprites;
    public TextMeshProUGUI messageText;

    [Header("카메라 진동")]
    public CameraShake cameraShake;

    [Header("사운드")]
    public AudioSource breakSound;

    [Header("설정")]
    public float nextSceneDelay = 2f;
    public string nextSceneName = "Talk2";

    [Header("설명 패널")]
    public GameObject instructionPanel; // ← 설명 패널 (UI 이미지 + 버튼)

    private bool gameStarted = false;

    void Start()
    {
        UpdateNumberSprite();

        if (messageText != null)
            messageText.gameObject.SetActive(false);

        // 설명 패널 먼저 띄우고 게임 멈춤
        if (instructionPanel != null)
            instructionPanel.SetActive(true);

        gameStarted = false;
    }

    void Update()
    {
        if (!gameStarted) return; // 설명 안 끝났으면 아무것도 안함

        if (rockHP > 0 && Input.GetKeyDown(KeyCode.Space))
        {
            rockHP--;
            UpdateNumberSprite();

            if (cameraShake != null)
                cameraShake.Shake();

            if (breakSound != null)
                breakSound.Play();

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

    // 👇 버튼에서 이걸 연결해주면 게임 시작
    public void StartGame()
    {
        if (instructionPanel != null)
            instructionPanel.SetActive(false);

        gameStarted = true;
    }
}
