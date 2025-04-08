using UnityEngine;
using UnityEngine.UI;
using TMPro;  // TextMeshPro 텍스트를 사용하기 위한 네임스페이스

public class EggBreaker : MonoBehaviour
{
  [Header("알 깨기 설정")]
  public int hitsToBreak = 10; // 알을 깨는 데 필요한 입력 횟수
  private int currentHits = 0; // 현재까지 입력된 횟수
  private bool isBroken = false; // 알이 깨졌는지 여부
  private bool failed = false;   // 실패(시간 초과) 여부

  [Header("타이머 설정")]
  public float timeLimit = 5f; // 제한 시간(초)
  private float timer = 0f;    // 남은 시간

  [Header("UI 연결")]
  public Slider timerBar;              // 타이머 바 (Slider 컴포넌트)
  public TextMeshProUGUI countText;    // 남은 횟수를 표시하는 텍스트 (TextMeshPro)

  void Start()
  {
    // 타이머 초기화
    timer = timeLimit;

    // 타이머 바 초기 설정
    if (timerBar != null)
    {
      timerBar.maxValue = timeLimit;
      timerBar.value = timeLimit;
    }

    // 텍스트 초기화 (hitsToBreak 숫자 그대로 표시)
    if (countText != null)
    {
      countText.text = hitsToBreak.ToString();
    }
  }

  void Update()
  {
    if (isBroken || failed) return; // 이미 끝났으면 더 이상 처리하지 않음

    // 타이머 감소
    timer -= Time.deltaTime;

    // 타이머 UI에 반영
    if (timerBar != null)
    {
      timerBar.value = timer;
    }

    // 시간이 다 됐을 경우 실패 처리
    if (timer <= 0f)
    {
      failed = true;
      Debug.Log("⏰ 시간 초과! 알 깨기 실패!");
      GetComponent<SpriteRenderer>().color = Color.red; // 실패 시 알을 빨갛게
      return;
    }

    // 스페이스바 입력 처리
    if (Input.GetKeyDown(KeyCode.Space))
    {
      currentHits++;

      // 남은 횟수 계산 및 텍스트 업데이트
      int remainingHits = hitsToBreak - currentHits;
      if (countText != null)
      {
        countText.text = Mathf.Max(remainingHits, 0).ToString(); // 0 이하로는 안 내려가게
      }

      // 알이 깨질 조건 충족 시
      if (currentHits >= hitsToBreak)
      {
        BreakEgg();
      }
    }
  }

  // 알이 깨졌을 때 호출되는 함수
  void BreakEgg()
  {
    isBroken = true;
    Debug.Log("🎉 알이 깨졌습니다!");
    GetComponent<SpriteRenderer>().color = Color.gray; // 깨진 알 표현
                                                       // 나중에: 애니메이션, 효과음, 다음 씬 전환 등 연결 가능
  }
}
