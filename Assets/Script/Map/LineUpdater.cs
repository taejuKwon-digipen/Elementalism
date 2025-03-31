using UnityEngine;

public class LineUpdater : MonoBehaviour
{
    public RectTransform content; // Scroll View�� Content (RectTransform)
    private Vector2 lastContentAnchoredPos;
    private LineRenderer[] lines;

    void Start()
    {
        lastContentAnchoredPos = content.anchoredPosition;
        lines = GetComponentsInChildren<LineRenderer>();

        foreach (LineRenderer line in lines)
        {
            line.useWorldSpace = false; // ���� �������� �����̵��� ����
        }
    }


    void Update()
    {
        Vector2 delta = content.anchoredPosition - lastContentAnchoredPos; // Content�� �̵��� �Ÿ� ���
        Debug.Log("Delta: " + delta); // Ȯ�ο� �α�

        foreach (LineRenderer line in lines)
        {
            Vector3 newPos = line.transform.position + new Vector3(delta.x, delta.y, 0);
            line.transform.position = newPos; // ���� �̵�
        }

        lastContentAnchoredPos = content.anchoredPosition; // ���� ��ġ ������Ʈ
    }
}
