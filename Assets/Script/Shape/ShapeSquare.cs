// �� ��ũ��Ʈ�� ���ӿ��� ���Ǵ� ShapeSquare Ŭ������ ������ �����մϴ�.
// ���� ������ ���� ���(square)�� ���� ���¸� �����ϸ�,
// ����� �����Ǿ��� �� ǥ���� �̹����� �����մϴ�.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeSquare : MonoBehaviour
{
    public Image occupiedImage; // ����� �����Ǿ��� �� ǥ���� �̹���

    void Start()
    {
        occupiedImage.gameObject.SetActive(false); // ���� ���� �� �̹����� ��Ȱ��ȭ�Ͽ� �ʱ� ���¸� ����
    }
}
