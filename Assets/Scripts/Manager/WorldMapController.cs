using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldMapController : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 5f;

    public Transform[] mapPoints;

    [Header("시작 포인트 인덱스")]
    public int startingIndex = 0;

    [Header("이동 경로 키 배열 (예: S→D→W→D)")]
    public KeyCode[] moveKeys;

    [Header("목적지 관련 설정")]
    public int enterablePointIndex;
    public string destinationScene;

    [Header("잠긴 마을 팝업")]
    public GameObject lockedPopupPanel;
    public float popupDuration = 2f;

    [Header("이동 효과음")]
    public AudioSource moveSFX;

    private int currentIndex = 0;
    private bool isMoving = false;

    void Start()
    {
        currentIndex = startingIndex;

        if (mapPoints != null && mapPoints.Length > currentIndex)
        {
            player.position = mapPoints[currentIndex].position;
        }

        // 시작 시 잠긴 팝업은 비활성화
        if (lockedPopupPanel != null)
        {
            lockedPopupPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (isMoving) return;

        if (currentIndex < moveKeys.Length && Input.GetKeyDown(moveKeys[currentIndex]))
        {
            TryMoveTo(currentIndex + 1);
        }
        else if (currentIndex > 0 && Input.GetKeyDown(GetReverseKey(moveKeys[currentIndex - 1])))
        {
            TryMoveTo(currentIndex - 1);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            TryEnterScene();
        }
    }

    void TryMoveTo(int targetIndex)
    {
        if (targetIndex < 0 || targetIndex >= mapPoints.Length) return;

        StartCoroutine(MoveTo(targetIndex));
    }

    System.Collections.IEnumerator MoveTo(int targetIndex)
    {
        isMoving = true;
        Vector3 start = player.position;
        Vector3 end = mapPoints[targetIndex].position;
        float elapsed = 0f;
        float duration = Vector3.Distance(start, end) / moveSpeed;

        if (moveSFX != null)
            moveSFX.Play();

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            player.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        player.position = end;
        currentIndex = targetIndex;
        isMoving = false;
    }

    void TryEnterScene()
    {
        if (currentIndex != enterablePointIndex)
        {
            if (lockedPopupPanel != null)
            {
                lockedPopupPanel.SetActive(true);
                Invoke(nameof(HideLockedPopup), popupDuration);
            }
            return;
        }

        if (!string.IsNullOrEmpty(destinationScene))
        {
            SceneManager.LoadScene(destinationScene);
        }
    }

    void HideLockedPopup()
    {
        if (lockedPopupPanel != null)
        {
            lockedPopupPanel.SetActive(false);
        }
    }

    private KeyCode GetReverseKey(KeyCode key)
    {
        return key switch
        {
            KeyCode.S => KeyCode.W,
            KeyCode.W => KeyCode.S,
            KeyCode.D => KeyCode.A,
            KeyCode.A => KeyCode.D,
            _ => KeyCode.None
        };
    }
}
