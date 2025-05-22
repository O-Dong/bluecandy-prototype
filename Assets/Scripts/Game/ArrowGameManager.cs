using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ArrowGameManager : MonoBehaviour
{
    [Header("UI 연출용")]
    public List<Image> arrowImages; // 정답 화살표 이미지들
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI timerText;

    [Header("게임 진행")]
    public Transform[] boardSpots;
    public Transform player;
    public int maxLife = 3;
    public float stageTimeLimit = 5f;

    private int currentStage = 0;
    private int currentInputIndex = 0;
    private int currentLife;
    private float currentTimer;

    private List<KeyCode[]> stageAnswers = new List<KeyCode[]>();

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(null); // 포커스 제거

        currentLife = maxLife;

        // 정답 키 시퀀스 (WASD 기반)
        stageAnswers.Add(new KeyCode[] {
            KeyCode.W, KeyCode.D, KeyCode.D, KeyCode.W,
            KeyCode.D, KeyCode.D, KeyCode.D, KeyCode.Space
        });

        StartStage();
    }

    void Update()
    {
        if (currentStage >= stageAnswers.Count) return;

        currentTimer -= Time.deltaTime;
        timerText.text = $"TIME: {Mathf.Ceil(currentTimer)}";

        if (currentTimer <= 0f)
        {
            HandleWrongInput();
        }

        if (Input.anyKeyDown)
        {
            KeyCode expected = stageAnswers[currentStage][currentInputIndex];

            if (Input.GetKeyDown(expected))
            {
                Debug.Log("정답 입력됨: " + expected);

                // UI → 월드 좌표로 변환
                Vector3 screenPos = arrowImages[currentInputIndex].rectTransform.position;
                // 월드 스페이스 Canvas에서는 그대로 사용
                Vector3 worldPos = arrowImages[currentInputIndex].rectTransform.position;

                // Z는 캐릭터 위치 유지
                worldPos.z = player.position.z;

                // 살짝 위로 띄우기
                worldPos.y += 0.5f;

                player.position = worldPos;
                StartCoroutine(FadeOutImage(arrowImages[currentInputIndex]));
                currentInputIndex++;

                if (currentInputIndex >= stageAnswers[currentStage].Length)
                {
                    MoveForward();
                }
            }
            else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
                     Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log("오답 입력됨");
                HandleWrongInput();
            }
        }

        if (currentStage == stageAnswers.Count && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("게임 클리어!");
            // SceneManager.LoadScene("NextScene"); // 다음 씬 이름 설정 필요
        }
    }

    void StartStage()
    {
        currentTimer = stageTimeLimit;
        currentInputIndex = 0;

        foreach (Image img in arrowImages)
        {
            img.color = new Color(1, 1, 1, 1);
        }

        lifeText.text = $"LIFE: {currentLife}";
    }

    void MoveForward()
    {
        currentStage++;
        if (currentStage < boardSpots.Length)
        {
            player.position = boardSpots[currentStage].position;
            StartStage();
        }
        else
        {
            Debug.Log("도착 완료!");
        }
    }

    void HandleWrongInput()
    {
        if (currentInputIndex < arrowImages.Count)
        {
            StartCoroutine(ShakeImage(arrowImages[currentInputIndex]));
        }

        currentLife--;
        lifeText.text = $"LIFE: {currentLife}";

        if (currentLife <= 0)
        {
            Debug.Log("실패! 다시 도전!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
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
        Vector3 origin = rt.localPosition;

        float duration = 0.3f;
        float time = 0f;
        float intensity = 10f;

        while (time < duration)
        {
            float offsetX = Random.Range(-intensity, intensity);
            rt.localPosition = origin + new Vector3(offsetX, 0, 0);
            time += Time.deltaTime;
            yield return null;
        }

        rt.localPosition = origin;
    }
}