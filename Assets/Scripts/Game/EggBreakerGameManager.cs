using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class EggBreakerGameManager : MonoBehaviour
{
    [Header("UI")]
    public RectTransform powerBar;
    public RectTransform targetZone;
    public RectTransform movingCursor;
    public Image eggImage;
    public Sprite[] eggStages;

    public GameObject introPanel;
    public Button startButton;

    public GameObject resultPanel;
    public TMP_Text resultText;

    [Header("하트 UI")]
    public Image[] heartImages;
    public Sprite heartFull;
    public Sprite heartBroken;

    [Header("사운드")]
    public AudioSource lifeBreakSound;
    public AudioSource eggCrackSound;
    public AudioSource bgmAudioSource;

    [Header("설정")]
    public int maxTries = 5;
    public int maxFails = 3;
    public float[] speeds;

    private int currentTry = 0;
    private int failCount = 0;
    private float cursorSpeed;
    private bool movingRight = true;
    private bool isPlaying = false;

    private void Start()
    {
        introPanel.SetActive(true);
        resultPanel.SetActive(false);
        isPlaying = false;

        cursorSpeed = speeds[0];

        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].sprite = heartFull;
        }

        startButton.onClick.AddListener(StartGame);

        bgmAudioSource.loop = true;
        bgmAudioSource.Play();
    }

    public void StartGame()
    {
        introPanel.SetActive(false);
        isPlaying = true;
    }

    private void Update()
    {
        if (!isPlaying) return;

        MoveCursor();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckHit();
        }
    }

    private void MoveCursor()
    {
        float step = cursorSpeed * Time.deltaTime;
        Vector3 newPos = movingCursor.localPosition;

        if (movingRight)
            newPos.x += step;
        else
            newPos.x -= step;

        float halfBar = powerBar.rect.width / 2f;
        float halfCursor = movingCursor.rect.width / 2f;

        if (newPos.x > halfBar - halfCursor)
        {
            newPos.x = halfBar - halfCursor;
            movingRight = false;
        }
        else if (newPos.x < -halfBar + halfCursor)
        {
            newPos.x = -halfBar + halfCursor;
            movingRight = true;
        }

        movingCursor.localPosition = newPos;
    }

    private void CheckHit()
    {
        float cursorX = movingCursor.position.x;
        float targetMinX = targetZone.position.x - targetZone.rect.width / 2f;
        float targetMaxX = targetZone.position.x + targetZone.rect.width / 2f;

        if (cursorX >= targetMinX && cursorX <= targetMaxX)
        {
            SuccessHit();
        }
        else
        {
            FailHit();
        }
    }

    private void SuccessHit()
    {
        currentTry++;

        if (currentTry < eggStages.Length)
        {
            eggImage.sprite = eggStages[currentTry];
            eggCrackSound.Play();                        // 🥚 효과음
            StartCoroutine(ShakeImage(eggImage));        // 🥚 흔들림
        }

        if (currentTry >= maxTries)
        {
            GameClear();
        }
        else
        {
            cursorSpeed = speeds[currentTry];
            RandomizeTargetZone();
        }
    }

    private void FailHit()
    {
        if (failCount < heartImages.Length)
        {
            heartImages[failCount].sprite = heartBroken;                // 💔 이미지 변경
            lifeBreakSound.Play();                                      // 💔 효과음
            StartCoroutine(ShakeImage(heartImages[failCount]));         // 💔 흔들림
        }

        failCount++;

        if (failCount >= maxFails)
        {
            GameOver();
        }
    }

    private void RandomizeTargetZone()
    {
        float barWidth = powerBar.rect.width;
        float targetWidth = Random.Range(60f, 160f);
        float maxRange = (barWidth - targetWidth) / 2f;
        float targetX = Random.Range(-maxRange, maxRange);

        targetZone.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetWidth);
        targetZone.localPosition = new Vector3(targetX, targetZone.localPosition.y, 0f);
    }

    private IEnumerator ShakeImage(Image img, float duration = 0.3f, float magnitude = 10f)
    {
        RectTransform rt = img.rectTransform;
        Vector3 originalPos = rt.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            rt.localPosition = originalPos + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        rt.localPosition = originalPos;
    }

    private void GameClear()
    {
        isPlaying = false;
        resultText.text = "알 깨기 성공!";
        resultPanel.SetActive(true);
    }

    private void GameOver()
    {
        isPlaying = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
