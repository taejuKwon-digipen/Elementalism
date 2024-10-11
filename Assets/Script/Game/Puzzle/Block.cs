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
    None,
    Fire,   // 불
    Water,  // 물
    Air,    // 공기
    Earth   // 흙
}

public class Block : MonoBehaviour
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

    public ElementType elementType = ElementType.None; // 원소 타입을 저장하는 변수 (초기화)
    public ElementType currentCollidedBlock = ElementType.None;
    private bool isColliding = false;

    // 프로퍼티들
    public bool Selected { get; set; }          // 선택되었는지 여부
    public int BlockIndex { get; set; }     // 원소의 인덱스
    public bool SqareOccupied { get; set; }     // 해당 칸이 점유되었는지 여부

    private void Start()
    {
        Selected = false;       // 선택 상태 초기화
        SqareOccupied = false;  // 점유 상태 초기화

        // 초기화 시 랜덤 원소 타입으로 설정
        elementType = (ElementType)Random.Range(1, 5);
        SetRandomBlockImage(elementType);
    }

    // 이 칸을 사용할 수 있는지 확인하는 메서드
    public bool CanWeUseThisSquare()
    {
        return hooverImage.gameObject.activeSelf;   // hooverImage가 활성화되어 있으면 true 반환
    }

    // 칸을 활성화하는 메서드
    // 칸을 활성화하는 메서드
    public void ActivateSquare()
    {
        hooverImage.gameObject.SetActive(false);    // hooverImage 비활성화
        activeImage.gameObject.SetActive(true);     // activeImage 활성화

        // 활성화된 이미지의 스프라이트를 원소 타입에 맞게 설정
        switch (currentCollidedBlock)
        {
            case ElementType.Fire:
                activeImage.sprite = fireSprite;     // 불 스프라이트로 설정
                break;
            case ElementType.Water:
                activeImage.sprite = waterSprite;    // 물 스프라이트로 설정
                break;
            case ElementType.Air:
                activeImage.sprite = airSprite;      // 공기 스프라이트로 설정
                break;
            case ElementType.Earth:
                activeImage.sprite = earthSprite;    // 흙 스프라이트로 설정
                break;
            default:
                activeImage.sprite = null;
                break;
        }

        Selected = true;                            // 선택됨으로 설정
        SqareOccupied = true;                       // 점유됨으로 설정
    }

    // 원소 타입에 따라 이미지 설정하는 메서드
    public void SetRandomBlockImage(ElementType elementType)
    {
        this.elementType = elementType; // 원소 타입 설정
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
            default:
                nomalImage.sprite = null;
                break;
        }
    }
    // 충돌 시작 시 호출되는 메서드
    private void OnTriggerEnter2D(Collider2D collision)
    {
        hooverImage.gameObject.SetActive(true); // hooverImage 활성화

        ShapeSquare collidedSquare = collision.GetComponent<ShapeSquare>();

        if (collidedSquare != null)
        {
            currentCollidedBlock = collidedSquare.elementType; // 충돌된 원소 타입 저장
            isColliding = true; // 충돌 상태 설정
            Debug.Log("충돌된 원소 타입: " + currentCollidedBlock);
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
        ShapeSquare collidedSquare = collision.GetComponent<ShapeSquare>();

        if (collidedSquare != null && currentCollidedBlock == collidedSquare.elementType)
        {
            hooverImage.gameObject.SetActive(false);
            currentCollidedBlock = ElementType.None; // 조건에 따라 초기화
            isColliding = false; // 충돌 상태 해제
            Debug.Log("충돌 종료: 원소 타입 초기화됨");
        }
    }



}
