using UnityEngine;
using System.Collections;

public class Talk6_1Controller : MonoBehaviour
{
    private DialogueManager dialogueManager;
    private bool hasShaken = false;

    [Header("흔들고 싶은 UI 오브젝트 (RectTransform)")]
    public RectTransform spriteToShake; // 배경 또는 UI 이미지

    [Header("효과음")]
    public AudioSource shakeSFX; // Inspector에 오디오 소스 넣기 (효과음)

    [Header("설정")]
    public int targetIndex = 14;         // 몇 번째 대사에서 흔들릴지
    public float shakeDuration = 0.3f;   // 흔들리는 시간
    public float shakeMagnitude = 10f;   // 흔들림 세기

    void Start()
    {
        dialogueManager = GetComponent<DialogueManager>();
    }

    void Update()
    {
        if (!hasShaken && dialogueManager.CurrentIndex == targetIndex)
        {
            hasShaken = true;

            if (spriteToShake != null)
                StartCoroutine(ShakeUI(spriteToShake, shakeDuration, shakeMagnitude));

            if (shakeSFX != null)
                shakeSFX.Play();
        }
    }

    IEnumerator ShakeUI(RectTransform target, float duration, float magnitude)
    {
        Vector2 originalPos = target.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;
            target.anchoredPosition = originalPos + new Vector2(offsetX, offsetY);

            elapsed += Time.deltaTime;
            yield return null;
        }

        target.anchoredPosition = originalPos;
    }
}
