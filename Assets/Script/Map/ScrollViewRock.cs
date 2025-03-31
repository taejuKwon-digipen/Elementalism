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
        // 사용자가 터치/마우스 클릭을 안 하면 속도를 0으로 고정
        if (Input.GetMouseButton(0))
        {
            scrollRect.velocity = Vector2.zero;
        }
    }
}

