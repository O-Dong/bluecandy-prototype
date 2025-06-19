using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ArrowGameManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject startPopupPanel;
    public Button startButton;

    [Header("게임 진행")]
    public Transform player;
    public List<GameObject> arrowGroups;
    public int maxLife = 3;
    public float stageTimeLimit = 10f;

    [Header("스테이지 이미지")]
    public Image stageImage;
    public List<Sprite> stageSprites;

    [Header("사운드")]
    public AudioSource moveSound;
    public AudioSource damageSound;
    public AudioSource bgmAudioSource;
    public AudioSource clockAlarmSound;

    [Header("하트 UI")]
    public Image[] heartImages;
    public Sprite heartFull;
    public Sprite heartBroken;

    [Header("타이머 시계 UI")]
    public Image clockImage;

    private List<List<Image>> stageArrowImages = new();
    private List<KeyCode[]> stageAnswers = new();
    private List<Image> currentArrowImages;

    private int currentStage = 0;
    private int currentInputIndex = 0;
    private int currentLife;
    private float currentTimer;
    private bool isGameStarted = false;
    private bool isClockShaking = false;
    public float clockAlarmThreshold = 3f;
    private bool canCountTime = false;

    private SpriteRenderer playerSpriteRenderer;

    void Awake()
    {
        foreach (GameObject group in arrowGroups)
        {
            List<Image> images = new();
            foreach (Transform child in group.transform)
            {
                Image img = child.GetComponent<Image>();
                if (img != null) images.Add(img);
            }
            stageArrowImages.Add(images);
        }

        stageAnswers.Add(new KeyCode[] { KeyCode.D, KeyCode.D, KeyCode.W, KeyCode.W, KeyCode.D, KeyCode.D, KeyCode.Space });
        stageAnswers.Add(new KeyCode[] { KeyCode.A, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.D, KeyCode.D, KeyCode.S, KeyCode.S, KeyCode.A, KeyCode.A, KeyCode.A, KeyCode.A, KeyCode.A, KeyCode.W, KeyCode.W, KeyCode.W, KeyCode.Space });
        stageAnswers.Add(new KeyCode[] { KeyCode.D, KeyCode.D, KeyCode.D, KeyCode.S, KeyCode.D, KeyCode.D, KeyCode.W, KeyCode.D, KeyCode.D, KeyCode.S, KeyCode.S, KeyCode.S, KeyCode.A, KeyCode.A, KeyCode.A, KeyCode.A, KeyCode.S, KeyCode.A, KeyCode.A, KeyCode.A, KeyCode.W, KeyCode.W, KeyCode.A, KeyCode.A, KeyCode.Space });
    }

    void Start()
    {
        Screen.SetResolution(1920, 1080, false);
        currentLife = maxLife;
        isGameStarted = false;

        foreach (var group in arrowGroups)
            group.SetActive(false);

        if (bgmAudioSource && !bgmAudioSource.isPlaying)
        {
            bgmAudioSource.loop = true;
            bgmAudioSource.Play();
        }

        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();

        startButton.onClick.AddListener(() =>
        {
            startPopupPanel.SetActive(false);
            isGameStarted = true;

            if (stageSprites[currentStage] != null)
            {
                stageImage.sprite = stageSprites[currentStage];
                stageImage.gameObject.SetActive(true);
                StartCoroutine(HideStageImageThenStartStage());
            }
            else
            {
                StartStage();
            }
        });

        UpdateHearts();
    }

    void Update()
    {
        if (!isGameStarted || currentStage >= stageAnswers.Count) return;

        if (canCountTime)
        {
            currentTimer -= Time.deltaTime;

            if (clockImage != null)
                clockImage.fillAmount = currentTimer / stageTimeLimit;

            if (currentTimer <= clockAlarmThreshold && !isClockShaking)
            {
                isClockShaking = true;
                StartCoroutine(ShakeClock(clockImage.rectTransform));

                if (clockAlarmSound != null && !clockAlarmSound.isPlaying)
                    clockAlarmSound.Play();
            }

            if (currentTimer <= 0f)
            {
                HandleWrongInput();
                return;
            }
        }

        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.A)) playerSpriteRenderer.flipX = true;
            if (Input.GetKeyDown(KeyCode.D)) playerSpriteRenderer.flipX = false;

            if (currentInputIndex >= stageAnswers[currentStage].Length ||
                currentInputIndex >= currentArrowImages.Count) return;

            KeyCode expected = stageAnswers[currentStage][currentInputIndex];
            if (Input.GetKeyDown(expected))
            {
                MovePlayer();
                StartCoroutine(FadeOutImage(currentArrowImages[currentInputIndex]));
                currentInputIndex++;

                if (currentInputIndex >= stageAnswers[currentStage].Length)
                    MoveForward();
            }
            else if (IsArrowKeyDown())
            {
                HandleWrongInput();
            }
        }
    }

    void MoveForward()
    {
        currentStage++;
        if (currentStage >= stageAnswers.Count)
        {
            SceneManager.LoadScene("Talk4_1");
            return;
        }

        if (stageSprites[currentStage] != null)
        {
            stageImage.sprite = stageSprites[currentStage];
            stageImage.gameObject.SetActive(true);
            StartCoroutine(HideStageImageThenStartStage());
        }
        else
        {
            StartStage();
        }
    }

    IEnumerator HideStageImageThenStartStage()
    {
        yield return new WaitForSeconds(1.5f);
        stageImage.gameObject.SetActive(false);
        StartStage();
    }

    void StartStage()
    {
        currentInputIndex = 0;
        isClockShaking = false;
        canCountTime = false;

        if (clockAlarmSound != null && clockAlarmSound.isPlaying)
            clockAlarmSound.Stop();

        currentTimer = stageTimeLimit;
        canCountTime = true;

        for (int i = 0; i < arrowGroups.Count; i++)
            arrowGroups[i].SetActive(i == currentStage);

        currentArrowImages = stageArrowImages[currentStage];
        foreach (var img in currentArrowImages)
            img.color = new Color(1, 1, 1, 1);
    }

    void MovePlayer()
    {
        Vector3 targetPos = currentArrowImages[currentInputIndex].rectTransform.position;
        targetPos.z = player.position.z;
        targetPos.y += 0.5f;

        StopAllCoroutines();
        StartCoroutine(SmoothMoveTo(player, targetPos, 0.15f));
        if (moveSound) moveSound.Play();
    }

    IEnumerator SmoothMoveTo(Transform target, Vector3 destination, float duration)
    {
        Vector3 start = target.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            target.position = Vector3.Lerp(start, destination, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        target.position = destination;
    }

    void HandleWrongInput()
    {
        if (currentInputIndex < currentArrowImages.Count)
            StartCoroutine(ShakeImage(currentArrowImages[currentInputIndex]));

        if (damageSound) damageSound.Play();

        currentLife--;
        UpdateHearts();

        if (currentLife <= 0)
            StartCoroutine(RestartSceneAfterDelay());
    }

    void UpdateHearts()
    {
        for (int i = 0; i < heartImages.Length; i++)
            heartImages[i].sprite = i < currentLife ? heartFull : heartBroken;
    }

    IEnumerator RestartSceneAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    bool IsArrowKeyDown()
    {
        return Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
               Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) ||
               Input.GetKeyDown(KeyCode.Space);
    }

    IEnumerator FadeOutImage(Image img)
    {
        Color original = img.color;
        for (float a = 1f; a >= 0.3f; a -= 0.1f)
        {
            img.color = new Color(original.r, original.g, original.b, a);
            yield return new WaitForSeconds(0.02f);
        }
    }

    IEnumerator ShakeImage(Image img)
    {
        RectTransform rt = img.GetComponent<RectTransform>();
        Vector2 originalPos = rt.anchoredPosition;
        float elapsed = 0f;
        float duration = 0.3f;
        float intensity = 0.2f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;
            rt.anchoredPosition = originalPos + new Vector2(x, y);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rt.anchoredPosition = originalPos;
    }

    IEnumerator ShakeClock(RectTransform rt)
    {
        Vector2 originalPos = rt.anchoredPosition;
        float elapsed = 0f;
        float duration = 1f;
        float intensity = 5f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;
            rt.anchoredPosition = originalPos + new Vector2(x, y);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rt.anchoredPosition = originalPos;
    }
}
