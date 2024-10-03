// 이 스크립트는 게임에서 원소 블록의 동작과 시각적 표현을 관리합니다.
// 원소의 타입에 따른 이미지 설정, 블록의 선택 및 활성화 상태,
// 그리고 충돌 시의 반응 등을 제어하여 퍼즐 게임의 인터랙션을 구현합니다.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 원소 타입을 정의하는 열거형 Enum
public enum ElementType
{
    Fire,   // 불
    Water,  // 물
    Air,    // 공기
    Earth   // 흙
}

public class Elemental : MonoBehaviour
{
    // 이미지 컴포넌트들
    public Image hooverImage;    // 마우스 오버 시 나타나는 이미지
    public Image activeImage;    // 활성화된 상태의 이미지
    public Image nomalImage;     // 기본 상태의 이미지

    // 각 원소에 해당하는 스프라이트들
    public Sprite fireSprite;    // 불 스프라이트
    public Sprite waterSprite;   // 물 스프라이트
    public Sprite airSprite;     // 공기 스프라이트
    public Sprite earthSprite;   // 흙 스프라이트

    // 프로퍼티들
    public bool Selected { get; set; }          // 선택되었는지 여부
    public int ElementalIndex { get; set; }     // 원소의 인덱스
    public bool SqareOccupied { get; set; }     // 해당 칸이 점유되었는지 여부

    // 이 칸을 사용할 수 있는지 확인하는 메서드
    public bool CanWeUseThisSquare()
    {
        return hooverImage.gameObject.activeSelf;   // hooverImage가 활성화되어 있으면 true 반환
    }

    // 칸을 활성화하는 메서드
    public void ActivateSquare()
    {
        hooverImage.gameObject.SetActive(false);    // hooverImage 비활성화
        activeImage.gameObject.SetActive(true);     // activeImage 활성화
        Selected = true;                            // 선택됨으로 설정
        SqareOccupied = true;                       // 점유됨으로 설정
    }

    private void Start()
    {
        Selected = false;       // 선택 상태 초기화
        SqareOccupied = false;  // 점유 상태 초기화
    }

    // 원소 타입에 따라 이미지 설정하는 메서드
    public void SetRandomElementImage(ElementType elementType)
    {
        switch (elementType)
        {
            case ElementType.Fire:
                nomalImage.sprite = fireSprite;     // 불 스프라이트로 설정
                break;
            case ElementType.Water:
                nomalImage.sprite = waterSprite;    // 물 스프라이트로 설정
                break;
            case ElementType.Air:
                nomalImage.sprite = airSprite;      // 공기 스프라이트로 설정
                break;
            case ElementType.Earth:
                nomalImage.sprite = earthSprite;    // 흙 스프라이트로 설정
                break;
        }
    }

    // 충돌 중일 때 호출되는 메서드
    private void OnTriggerStay2D(Collider2D collision)
    {
        hooverImage.gameObject.SetActive(true);     // hooverImage 활성화
    }

    // 충돌이 종료되었을 때 호출되는 메서드
    private void OnTriggerExit2D(Collider2D collision)
    {
        hooverImage.gameObject.SetActive(false);    // hooverImage 비활성화
    }

    // 충돌이 시작될 때 호출되는 메서드
    private void OnTriggerEnter2D(Collider2D collision)
    {
        hooverImage.gameObject.SetActive(true);     // hooverImage 활성화
    }
}
