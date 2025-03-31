using UnityEngine;

public class LineUpdater : MonoBehaviour
{
    public RectTransform content; // Scroll View의 Content (RectTransform)
    private Vector2 lastContentAnchoredPos;
    private LineRenderer[] lines;

    void Start()
    {
        lastContentAnchoredPos = content.anchoredPosition;
        lines = GetComponentsInChildren<LineRenderer>();

        foreach (LineRenderer line in lines)
        {
            line.useWorldSpace = false; // 로컬 공간에서 움직이도록 설정
        }
    }


    void Update()
    {
        Vector2 delta = content.anchoredPosition - lastContentAnchoredPos; // Content가 이동한 거리 계산
        Debug.Log("Delta: " + delta); // 확인용 로그

        foreach (LineRenderer line in lines)
        {
            Vector3 newPos = line.transform.position + new Vector3(delta.x, delta.y, 0);
            line.transform.position = newPos; // 라인 이동
        }

        lastContentAnchoredPos = content.anchoredPosition; // 현재 위치 업데이트
    }
}
