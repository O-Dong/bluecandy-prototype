using UnityEngine;
using System.Collections;

public class Talk2Controller : MonoBehaviour
{
    private DialogueManager dialogueManager;
    private bool hasShaken = false;

    [Header("흔들고 싶은 UI 오브젝트 (RectTransform)")]
    public RectTransform spriteToShake; // ← 배경 이미지(RectTransform) 넣기

    void Start()
    {
        dialogueManager = GetComponent<DialogueManager>();
    }

    void Update()
    {
        if (!hasShaken && dialogueManager.CurrentIndex == 20)
        {
            hasShaken = true;

            if (spriteToShake != null)
                StartCoroutine(ShakeUI(spriteToShake, 0.3f, 10f)); // 지속시간 0.3초, 세기 10f
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
