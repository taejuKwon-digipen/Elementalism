using UnityEngine;

public class LineUpdater : MonoBehaviour
{
    public Transform content; // Scroll View의 Content
    private Vector3 lastContentPos;
    private LineRenderer[] lines;

    void Start()
    {
        lastContentPos = content.position;
        lines = GetComponentsInChildren<LineRenderer>(); // 모든 라인 가져오기
    }

    void Update()
    {
        Vector3 delta = content.position - lastContentPos; // Content가 이동한 거리 계산
        foreach (LineRenderer line in lines)
        {
            line.transform.position += delta; // 라인도 같은 거리만큼 이동
        }
        lastContentPos = content.position; // 현재 위치 업데이트
    }
}
