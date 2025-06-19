using UnityEngine;

[System.Serializable]
public class MapPoint
{
    public Transform point;         // 위치 포인트 (Point0, Point1 등)
    public bool isUnlocked = false; // 열려있는지 여부
    public string destinationScene; // 스페이스바 눌렀을 때 이동할 씬 이름
    public string description;      // (선택) UI로 보여줄 마을 설명
}

