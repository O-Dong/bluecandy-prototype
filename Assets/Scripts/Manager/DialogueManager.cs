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
        public string speaker;      // "주인공", "꾸요", "나레이션"
        public string text;         // 대사
        public Sprite faceSprite;   // 표정 이미지
    }

    public List<DialogueLine> dialogueLines;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Image characterImage;
    public GameObject dialoguePanel;
    public Button nextButton;

    [Header("대화가 끝난 후 이동할 씬 이름")]
    public string nextSceneName; // 인스펙터에서 설정 가능

    private int currentIndex = 0;

    void Start()
    {
        ShowLine();
        nextButton.onClick.AddListener(NextLine);
    }

    void ShowLine()
    {
        if (currentIndex >= dialogueLines.Count)
        {
            // 패널 비활성화 생략 → 한 프레임 깜빡임 방지
            // dialoguePanel.SetActive(false);

            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }

            return;
        }

        DialogueLine line = dialogueLines[currentIndex];

        // 이름 출력
        string speakerName = line.speaker.Replace("{playerName}", GlobalGameManager.Instance.playerName);
        nameText.text = speakerName == "나레이션" ? "" : speakerName;

        // 대사 출력 (playerName 치환 포함)
        string lineText = line.text.Replace("{playerName}", GlobalGameManager.Instance.playerName);
        dialogueText.text = lineText;

        // 캐릭터 이미지 처리
        if (line.faceSprite != null && line.speaker != "나레이션")
        {
            characterImage.sprite = line.faceSprite;
            characterImage.color = Color.white;
        }
        else
        {
            characterImage.color = new Color(1, 1, 1, 0); // 투명하게 처리
        }
    }

    void NextLine()
    {
        currentIndex++;
        ShowLine();
    }
}
