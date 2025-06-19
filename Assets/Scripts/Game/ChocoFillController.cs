using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public class ChocoFillGameManager : MonoBehaviour
{
    [Header("UI")]
    public Image chocoImage;
    public TMP_Text messageText;

    [Header("게임 파라미터")]
    public float targetValue = 0.752f;

    // 내부 상태
    private bool isFilling = false;
    private float fillAmount = 0f;
    private float fillSpeed = 0.2f;
    private float minRange;
    private float maxRange;

    private int currentStage = 0;

    private bool isInputEnabled = true;

    // 스테이지 데이터 (통합)
    private List<(float minRange, float maxRange, float fillSpeed)> stageList = new();

    void Start()
    {
        InitStages();
        ApplyStage(currentStage);
        messageText.text = "";  // 처음엔 메시지 안보이게
    }

    void Update()
    {
        if (!isInputEnabled) return;

        if (Input.GetKeyDown(KeyCode.Space))
            isFilling = true;

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isFilling = false;
            CheckResult();
        }

        if (isFilling)
        {
            fillAmount += fillSpeed * Time.deltaTime;
            fillAmount = Mathf.Clamp(fillAmount, 0f, 1f);
            chocoImage.fillAmount = fillAmount;
        }
    }

    void InitStages()
    {
        stageList.Clear();

        // Stage 1
        stageList.Add((targetValue - 0.07f, targetValue + 0.07f, 0.5f));
        // Stage 2
        stageList.Add((targetValue - 0.06f, targetValue + 0.06f, 0.7f));
        // Stage 3
        stageList.Add((targetValue - 0.05f, targetValue + 0.05f, 0.9f));
        // Stage 4
        stageList.Add((targetValue - 0.03f, targetValue + 0.03f, 1.0f));
        // Stage 5 (극악)
        stageList.Add((targetValue - 0.02f, targetValue + 0.02f, 2.0f));

        stageList.Add((targetValue - 0.01f, targetValue + 0.01f, 2.2f));

    }

    void ApplyStage(int stage)
    {
        fillSpeed = stageList[stage].fillSpeed;
        minRange = stageList[stage].minRange;
        maxRange = stageList[stage].maxRange;

        fillAmount = 0f;
        chocoImage.fillAmount = 0f;

        Debug.Log($"=== Stage {stage + 1} 시작 ===");
    }

    void CheckResult()
    {
        isInputEnabled = false;

        if (fillAmount >= minRange && fillAmount <= maxRange)
        {
            Debug.Log("Perfect!");
            StartCoroutine(StageClear());
        }
        else
        {
            Debug.Log("Fail!");
            StartCoroutine(StageFail());
        }
    }

    IEnumerator StageClear()
    {
        messageText.text = "성공!";
        yield return new WaitForSeconds(1.0f);  // 1초 대기

        currentStage++;
        if (currentStage >= stageList.Count)
        {
            messageText.text = "초코컵을 다 채웠다!!!";
            yield break;
        }
        else
        {
            messageText.text = "";
            ApplyStage(currentStage);
            isInputEnabled = true;
        }
    }

    IEnumerator StageFail()
    {
        messageText.text = "실패!";
        yield return new WaitForSeconds(1.0f);  // 1초 대기

        messageText.text = "";
        fillAmount = 0f;
        chocoImage.fillAmount = 0f;
        isInputEnabled = true;
    }
}
