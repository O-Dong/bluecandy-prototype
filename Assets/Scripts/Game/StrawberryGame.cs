using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StrawberryGame : MonoBehaviour
{
    public int targetCount = 30;
    private int currentCount = 0;

    public float totalTime = 30f;
    private float timer;

    [Header("UI 요소")]
    public Image fruitImage;
    public Sprite normalStrawberry;
    public Sprite rottenStrawberry;
    public TMP_Text timerText;
    public TMP_Text countText;
    public GameObject fakeText;
    public TMP_Text resultText;
    public GameObject retryButton;

    [Header("게임 시작 전 패널")]
    public GameObject startPanel;
    public Button startButton;

    public string nextSceneName = "NextScene";

    private bool isFake = false;
    private bool isFakeActive = false;
    private float fakeTimer = 0f;
    private float fakeDuration = 1f;
    private bool waitingForInput = false;

    private bool gameActive = false;

    void Start()
    {
        timer = totalTime;
        fakeText.SetActive(false);
        resultText.gameObject.SetActive(false);
        retryButton.SetActive(false);
        countText.text = $"Score: {currentCount}/{targetCount}";

        // 시작 패널 활성화
        startPanel.SetActive(true);
        startButton.onClick.AddListener(StartGame);
    }

    void Update()
    {
        if (!gameActive) return;

        timer -= Time.deltaTime;
        timerText.text = timer.ToString("F1");

        if (timer <= 0)
        {
            EndGame();
            return;
        }

        if (isFakeActive)
        {
            fakeTimer += Time.deltaTime;
            if (fakeTimer >= fakeDuration)
            {
                NextStrawberry();
            }
        }
        else if (waitingForInput)
        {
            fakeTimer += Time.deltaTime;
            if (fakeTimer >= fakeDuration)
            {
                NextStrawberry();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isFakeActive)
            {
                AddScore(-3);
                NextStrawberry();
            }
            else if (waitingForInput)
            {
                AddScore(1);
                ShakeFruit();
                NextStrawberry();
            }
        }
    }

    public void StartGame()
    {
        gameActive = true;
        startButton.gameObject.SetActive(false); // 버튼도 꺼!
        startPanel.SetActive(false);             // 패널 꺼!
        GenerateStrawberry();
    }

    void GenerateStrawberry()
    {
        isFake = Random.value < 0.3f;

        if (isFake)
        {
            fakeText.SetActive(true);
            fruitImage.sprite = rottenStrawberry;
            isFakeActive = true;
            fakeTimer = 0f;
        }
        else
        {
            fakeText.SetActive(false);
            fruitImage.sprite = normalStrawberry;
            waitingForInput = true;
            fakeTimer = 0f;
        }
    }

    void NextStrawberry()
    {
        isFakeActive = false;
        waitingForInput = false;
        fakeText.SetActive(false);
        GenerateStrawberry();
    }

    void AddScore(int amount)
    {
        currentCount += amount;
        if (currentCount < 0) currentCount = 0;
        countText.text = $"{currentCount}/{targetCount}";

        if (currentCount >= targetCount)
        {
            Success();
        }
    }

    void ShakeFruit()
    {
        fruitImage.rectTransform.localRotation = Quaternion.Euler(0, 0, Random.Range(-15f, 15f));
        Invoke("ResetShake", 0.1f);
    }

    void ResetShake()
    {
        fruitImage.rectTransform.localRotation = Quaternion.identity;
    }

    void Success()
    {
        gameActive = false;

        fruitImage.gameObject.SetActive(false);
        fakeText.SetActive(false);
        resultText.gameObject.SetActive(true);
        resultText.text = $"성공!\n최종 딸기 개수: {currentCount}/{targetCount}";

        Invoke("LoadNextScene", 1.5f);
    }

    void EndGame()
    {
        gameActive = false;

        fruitImage.gameObject.SetActive(false);
        fakeText.SetActive(false);
        resultText.gameObject.SetActive(true);
        retryButton.SetActive(true);
        resultText.text = $"실패!\n최종 딸기 개수: {currentCount}/{targetCount}";
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    public void OnRetry()
    {
        Debug.Log("재시도 버튼 눌림!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
