using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speaker;      // "주인공", "꾸요", "바닐라", "나레이션"
        public string text;         // 대사
        public Sprite faceSprite;   // 표정 이미지
    }

    public List<DialogueLine> dialogueLines;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;
    public Button nextButton;

    public Image leftCharacterImage;   // 주인공 전용
    public Image rightCharacterImage;  // 꾸요/바닐라 공용

    [Header("대화가 끝난 후 이동할 씬 이름")]
    public string nextSceneName;

    private int currentIndex = 0;
    private Coroutine typingCoroutine;

    void Start()
    {
        leftCharacterImage.color = new Color(1, 1, 1, 0);
        rightCharacterImage.color = new Color(1, 1, 1, 0);

        ShowLine();
        nextButton.onClick.AddListener(NextLine);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextLine();
        }
    }

    void ShowLine()
    {
        if (currentIndex >= dialogueLines.Count)
        {
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
            return;
        }

        DialogueLine line = dialogueLines[currentIndex];

        // 이름 치환
        string speakerName = line.speaker == "주인공"
            ? GlobalGameManager.Instance.playerName
            : line.speaker;
        nameText.text = speakerName == "나레이션" ? "" : speakerName;

        // 텍스트 타이핑 처리
        string lineText = line.text.Replace("{playerName}", GlobalGameManager.Instance.playerName);
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(lineText));

        // 이미지 기본 어둡게
        leftCharacterImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        rightCharacterImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);

        if (line.speaker == "주인공" || line.speaker == "꾸요.")
        {
            leftCharacterImage.sprite = line.faceSprite;
            leftCharacterImage.color = Color.white;
            leftCharacterImage.transform.SetAsLastSibling();
        }
        else if (line.speaker == "꾸요" ||
                 line.speaker == "??" ||
                 line.speaker == "바닐라" ||
                 line.speaker == "딸기" ||
                 line.speaker == "초코" ||
                 line.speaker == "녹차")
        {
            rightCharacterImage.sprite = line.faceSprite;
            rightCharacterImage.color = Color.white;
            rightCharacterImage.transform.SetAsLastSibling();
        }
    }

    void NextLine()
    {
        currentIndex++;
        ShowLine();
    }

    IEnumerator TypeText(string line)
    {
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.05f); // 타이핑 속도 조절
        }
    }

    public int CurrentIndex => currentIndex;
}
