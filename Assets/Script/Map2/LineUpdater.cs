using UnityEngine;

public class LineUpdater : MonoBehaviour
{
    public Transform content; // Scroll View�� Content
    private Vector3 lastContentPos;
    private LineRenderer[] lines;

    void Start()
    {
        lastContentPos = content.position;
        lines = GetComponentsInChildren<LineRenderer>(); // ��� ���� ��������
    }

    void Update()
    {
        Vector3 delta = content.position - lastContentPos; // Content�� �̵��� �Ÿ� ���
        foreach (LineRenderer line in lines)
        {
            line.transform.position += delta; // ���ε� ���� �Ÿ���ŭ �̵�
        }
        lastContentPos = content.position; // ���� ��ġ ������Ʈ
    }
}
