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
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI timerText;

    [Header("게임 진행")]
    public Transform player;
    public List<GameObject> arrowGroups; // Stage1Arrows, Stage2Arrows, Stage3Arrows
    public int maxLife = 3;
    public float stageTimeLimit = 10f;

    [Header("스테이지 이미지")]
    public Image stageImage;
    public List<Sprite> stageSprites;

    private List<List<Image>> stageArrowImages = new();
    private List<KeyCode[]> stageAnswers = new();
    private List<Image> currentArrowImages;

    private int currentStage = 0;
    private int currentInputIndex = 0;
    private int currentLife;
    private float currentTimer;
    private bool isGameStarted = false;

    void Awake()
    {
        // arrowGroups의 자식 Image 수집
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

        // 스테이지별 정답 시퀀스 설정
        stageAnswers.Add(new KeyCode[] {
            KeyCode.D, KeyCode.D, KeyCode.W, KeyCode.W,
            KeyCode.D, KeyCode.D, KeyCode.D, KeyCode.Space
        });

        stageAnswers.Add(new KeyCode[] {
            KeyCode.A, KeyCode.A, KeyCode.A,
            KeyCode.S, KeyCode.S,
            KeyCode.D, KeyCode.D,
            KeyCode.S,
            KeyCode.A, KeyCode.A, KeyCode.A, KeyCode.A,
            KeyCode.W, KeyCode.W, KeyCode.W,
            KeyCode.A,
            KeyCode.Space
        });

        stageAnswers.Add(new KeyCode[] {
            KeyCode.D, KeyCode.D, KeyCode.D,
            KeyCode.S,
            KeyCode.D, KeyCode.D,
            KeyCode.W,
            KeyCode.D, KeyCode.D,
            KeyCode.S, KeyCode.S, KeyCode.S,
            KeyCode.A, KeyCode.A, KeyCode.A,
            KeyCode.S,
            KeyCode.D, KeyCode.D, KeyCode.D, KeyCode.D, KeyCode.D, KeyCode.D,
            KeyCode.W, KeyCode.W,
            KeyCode.A,
            KeyCode.W,
            KeyCode.D, KeyCode.D,
            KeyCode.Space
        });

    }

    void Start()
    {
        stageImage.gameObject.SetActive(false); // 처음엔 꺼두기
        currentLife = maxLife;
        isGameStarted = false;

        startButton.onClick.AddListener(() =>
        {
            startPopupPanel.SetActive(false);
            isGameStarted = true;
            StartStage();
        });
    }

    void Update()
    {
        if (!isGameStarted) return;
        if (currentStage >= stageAnswers.Count) return;

        currentTimer -= Time.deltaTime;
        timerText.text = $"TIME: {Mathf.Ceil(currentTimer)}";

        if (currentTimer <= 0f)
        {
            HandleWrongInput();
            return;
        }

        if (Input.anyKeyDown)
        {
            if (currentInputIndex >= stageAnswers[currentStage].Length ||
                currentInputIndex >= currentArrowImages.Count)
            {
                Debug.LogWarning("시퀀스 또는 화살표 이미지 범위를 초과함");
                return;
            }

            KeyCode expected = stageAnswers[currentStage][currentInputIndex];
            if (Input.GetKeyDown(expected))
            {
                MovePlayer();
                StartCoroutine(FadeOutImage(currentArrowImages[currentInputIndex]));
                currentInputIndex++;

                if (currentInputIndex >= stageAnswers[currentStage].Length)
                {
                    Debug.Log($"스테이지 {currentStage + 1} 클리어!");
                    MoveForward();
                }
            }
            else if (IsArrowKeyDown())
            {
                HandleWrongInput();
            }
        }
    }

    // 변경 부분만 표시: StartStage()
    void StartStage()
    {
        Debug.Log($"[DEBUG] StartStage: {currentStage}");

        if (currentStage >= arrowGroups.Count)
        {
            Debug.LogError($"[ERROR] currentStage {currentStage}가 arrowGroups.Count {arrowGroups.Count}보다 큼");
            return;
        }

        currentInputIndex = 0;
        currentTimer = stageTimeLimit;

        // 스테이지 UI 설정
        Debug.Log("[DEBUG] arrowGroups 활성화 상태 설정 시작");
        for (int i = 0; i < arrowGroups.Count; i++)
        {
            bool active = i == currentStage;
            Debug.Log($"arrowGroups[{i}].SetActive({active})");
            arrowGroups[i].SetActive(active);
        }

        if (currentStage >= stageArrowImages.Count)
        {
            Debug.LogError($"[ERROR] currentStage {currentStage}가 stageArrowImages.Count {stageArrowImages.Count}보다 큼");
            return;
        }

        currentArrowImages = stageArrowImages[currentStage];

        foreach (var img in currentArrowImages)
        {
            img.color = new Color(1, 1, 1, 1);
        }

        lifeText.text = $"LIFE: {currentLife}";
    }


    // 변경 부분만 표시: MoveForward()
    void MoveForward()
    {
        currentStage++;

        if (currentStage >= stageAnswers.Count)
        {
            Debug.Log("모든 스테이지 클리어!");
            // SceneManager.LoadScene("ClearScene");
            return;
        }

        // 다음 스테이지 이미지 보여주기
        if (currentStage < stageSprites.Count)
        {
            if (stageSprites[currentStage] != null)
            {
                stageImage.sprite = stageSprites[currentStage];
                stageImage.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"[WARN] stageSprites[{currentStage}] is null");
            }
        }

        StartCoroutine(HideStageImageAndStartNextStage());
    }


    IEnumerator HideStageImageAndStartNextStage()
    {
        yield return new WaitForSeconds(1.5f);
        stageImage.gameObject.SetActive(false);
        StartStage();
    }

    void MovePlayer()
    {
        Vector3 pos = currentArrowImages[currentInputIndex].rectTransform.position;
        pos.z = player.position.z;
        pos.y += 0.5f;
        player.position = pos;
    }

    void HandleWrongInput()
    {
        if (currentInputIndex < currentArrowImages.Count)
        {
            StartCoroutine(ShakeImage(currentArrowImages[currentInputIndex]));
        }

        currentLife--;
        lifeText.text = $"LIFE: {currentLife}";

        if (currentLife <= 0)
        {
            Debug.Log("실패! 다시 도전!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    bool IsArrowKeyDown()
    {
        return Input.GetKeyDown(KeyCode.W) ||
               Input.GetKeyDown(KeyCode.A) ||
               Input.GetKeyDown(KeyCode.S) ||
               Input.GetKeyDown(KeyCode.D) ||
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
        Vector2 origin = rt.anchoredPosition;

        float time = 0f;
        float duration = 0.15f;
        float intensity = 3f;

        while (time < duration)
        {
            float offset = Mathf.Sin(time * 50f) * intensity;
            rt.anchoredPosition = origin + new Vector2(offset, 0);
            time += Time.deltaTime;
            yield return null;
        }

        rt.anchoredPosition = origin;
    }
}
