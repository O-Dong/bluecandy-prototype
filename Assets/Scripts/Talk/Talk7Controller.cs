using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Talk7Controller : MonoBehaviour
{
    private DialogueManager dialogueManager;

    [System.Serializable]
    public class ShowEvent
    {
        public int index;
        public Sprite sprite;
        public AudioSource audioSource;
        public int duration = 1;
    }

    [Header("중앙에 띄울 이미지")]
    public Image itemImage;

    [Header("이벤트 설정")]
    public List<ShowEvent> showEvents;

    [Header("배경음악 (BGM)")]
    public AudioSource bgmAudioSource;

    private ShowEvent currentEvent = null;
    private int hideAtIndex = -1;
    private HashSet<int> triggered = new();

    void Start()
    {
        dialogueManager = GetComponent<DialogueManager>();

        if (itemImage != null)
            itemImage.gameObject.SetActive(false);

        // 🔥 BGM 시작
        if (bgmAudioSource != null && !bgmAudioSource.isPlaying)
        {
            bgmAudioSource.loop = true;
            bgmAudioSource.Play();
        }
    }

    void Update()
    {
        int currentIndex = dialogueManager.CurrentIndex;

        // 이벤트 트리거
        foreach (var evt in showEvents)
        {
            if (evt.index == currentIndex && !triggered.Contains(evt.index))
            {
                triggered.Add(evt.index);
                currentEvent = evt;
                hideAtIndex = currentIndex + evt.duration;

                if (itemImage != null && evt.sprite != null)
                {
                    itemImage.sprite = evt.sprite;
                    itemImage.gameObject.SetActive(true);
                }

                if (evt.audioSource != null)
                {
                    evt.audioSource.Play();
                }
            }
        }

        // 이미지 숨기기
        if (currentEvent != null && currentIndex >= hideAtIndex)
        {
            itemImage.gameObject.SetActive(false);
            currentEvent = null;
        }
    }
}
