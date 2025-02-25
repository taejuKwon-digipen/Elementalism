using UnityEngine;
using UnityEngine.UI;

public class ScrollViewLock : MonoBehaviour
{
    private ScrollRect scrollRect;

    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    void Update()
    {
        // ����ڰ� ��ġ/���콺 Ŭ���� �� �ϸ� �ӵ��� 0���� ����
        if (Input.GetMouseButton(0))
        {
            scrollRect.velocity = Vector2.zero;
        }
    }
}

