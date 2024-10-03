// 이 스크립트는 게임에서 사용되는 ShapeSquare 클래스의 동작을 정의합니다.
// 퍼즐 조각의 개별 블록(square)에 대한 상태를 관리하며,
// 블록이 점유되었을 때 표시할 이미지를 제어합니다.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeSquare : MonoBehaviour
{
    public Image occupiedImage; // 블록이 점유되었을 때 표시할 이미지

    void Start()
    {
        occupiedImage.gameObject.SetActive(false); // 게임 시작 시 이미지를 비활성화하여 초기 상태를 설정
    }
}
