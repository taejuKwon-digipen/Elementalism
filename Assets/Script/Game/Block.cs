using UnityEngine;
using UnityEngine.UI;

// 원소 타입을 정의하는 Enum
public enum ElementType
{
    None,
    Fire,   // 불
    Water,  // 물
    Air,    // 바람
    Earth,  // 땅
    Void,   // 공허
    Random  // 랜덤
}

public class Block : MonoBehaviour
{
    // 원소 타입에 따른 이미지를 설정하는 메서드
    public Image hooverImage;    // 콤보 판넬에 있는 원소 이미지
    public Image activeImage;    // 활성화된 원소 이미지
    public Image nomalImage;     // 기본 원소 이미지
    public Image oraImage;     // 오라 원소 이미지

    // 원소 타입에 따른 스프라이트를 설정하는 변수
    public Sprite fireSprite;    // 불 스프라이트
    public Sprite waterSprite;   // 물 스프라이트
    public Sprite airSprite;     // 바람 스프라이트
    public Sprite earthSprite;   // 땅 스프라이트

    public ElementType elementType = ElementType.None; // 원소 타입을 저장하는 변수 (초기화)
    public ElementType currentCollidedBlock = ElementType.None;

    public ElementType originalElementType = ElementType.None;
    private bool isColliding = false;

    // 원소 타입에 따른 이미지를 설정하는 메서드
    public bool Selected { get; set; }          // 선택된 상태
    public int BlockIndex { get; set; }     // 블록의 인덱스
    public bool SquareOccupied { get; set; }     // 사용중인 블록인지 여부

    private void Start()
    {
        Selected = false;       // 선택 상태 초기화
        SquareOccupied = false;  // 사용중인 블록인지 여부 초기화

    }

    private void Update()
    {
        SetBlockImage(elementType);
    }

    // 사용할 수 있는 블록인지 확인하는 메서드
    public bool CanWeUseThisSquare()
    {
        return hooverImage.gameObject.activeSelf;   // hooverImage가 활성화되어 있으면 true 반환
    }

    public void RestoreState()
    {
        // originalElementType가 설정되어 있으면 원소 타입 설정
        if (originalElementType != ElementType.None)
        {
            // 복원 시점에도 GameManager의 규칙을 적용할지 여부 결정 필요
            // 여기서는 원본 그대로 복원한다고 가정, 필요시 GetModifiedElementType 적용
            SetElementType(originalElementType, true); // isOriginalSetup 플래그 추가하여 무한 루프 방지
            originalElementType = ElementType.None;
        }
        else if (elementType != ElementType.None) // originalElementType가 없을 경우 현재 elementType을 기반으로 초기화
        {
             // 초기화 시에도 GameManager 규칙 적용
            SetElementType(elementType, true);
        }


        // 원소 타입 초기화
        Selected = false;
        SquareOccupied = false;
        
        // 이미지 초기화
        if (hooverImage != null)
            hooverImage.gameObject.SetActive(false);
        if (activeImage != null)
            activeImage.gameObject.SetActive(false);
            
        currentCollidedBlock = ElementType.None;
    }
    
    public void PlaceShapeOnBoard()
    {
        ActivateSquare();
    }

    // 블록을 활성화하는 메서드
    public void ActivateSquare()
    {
        if (currentCollidedBlock != ElementType.None)
        {
            if(originalElementType == ElementType.None)
            {
                originalElementType = elementType;
            }   
            
            hooverImage.gameObject.SetActive(false);    // hooverImage 비활성화
            activeImage.gameObject.SetActive(true);     // activeImage 활성화

            elementType = currentCollidedBlock;

            Selected = true;                            // 선택된 상태로 설정
            SquareOccupied = true;                       // 사용중인 블록으로 설정
        }
    }

    public void DisactivateActiveImage()
    {
        activeImage.gameObject.SetActive(false);
    }

    public void ActivateOraImage()
    {
        oraImage.gameObject.SetActive(true);
    }
    public void DisactivateOraImage()
    {
        oraImage.gameObject.SetActive(false);
    }

    // Ora 활성화 상태 확인
    public bool IsOraActive()
    {
        return oraImage != null && oraImage.gameObject.activeSelf;
    }

    public void SetElementType(ElementType newType, bool isOriginalSetup = false)
    {
        ElementType finalType = newType;
        if (!isOriginalSetup && GameManager.Instance != null) // isOriginalSetup으로 무한 재귀 호출 방지
        {
            finalType = GameManager.Instance.GetModifiedElementType(newType);
        }
        
        elementType = finalType;
        // SetBlockImage를 직접 호출하기보다 elementType만 설정하고 Update에서 처리하도록 유도할 수 있으나,
        // 즉각적인 반영을 위해 여기서도 이미지 설정을 호출 (Update와 중복될 수 있으므로 주의)
        // 또는 SetBlockImage 내부에서만 GameManager 규칙을 적용하고, 여기서는 elementType = newType; 만 수행
        // 현재는 SetBlockImage가 Update에서 호출되므로, 여기서는 elementType만 변경.
        // 만약 즉시 이미지가 바뀌어야 한다면 SetBlockImage(finalType) 호출.
        // 혼란을 줄이기 위해 SetBlockImage 내부에서만 GameManager 규칙을 적용하도록 변경
    }

    // 원소 타입에 따른 이미지를 설정하는 메서드
    public void SetBlockImage(ElementType typeToSet)
    {
        ElementType finalType = typeToSet;
        if (GameManager.Instance != null)
        {
            finalType = GameManager.Instance.GetModifiedElementType(typeToSet);
        }

        this.elementType = finalType; // 실제 블록의 타입을 최종 타입으로 설정

        switch (finalType) // 최종 변환된 타입으로 이미지 설정
        {
            case ElementType.Fire:
                nomalImage.sprite = fireSprite;     // 불 스프라이트로 설정
                break;
            case ElementType.Water:
                nomalImage.sprite = waterSprite;    // 물 스프라이트로 설정
                break;
            case ElementType.Air:
                nomalImage.sprite = airSprite;      // 바람 스프라이트로 설정
                break;
            case ElementType.Earth: // ChallengeLevel 1에서는 Fire로 이미 변환되었을 것임
                nomalImage.sprite = earthSprite;    // 땅 스프라이트로 설정
                break;
            case ElementType.Void:
                nomalImage.sprite = null;           // 공허는 이미지 없음
                break;
            case ElementType.Random:
                // Random인 경우 랜덤한 원소 타입 선택 (None과 Random 제외)
                ElementType randomType = (ElementType)Random.Range(1, (int)ElementType.Void); // Void도 제외하려면 (int)ElementType.Void
                // 랜덤 생성된 타입에도 GameManager 규칙 적용
                if (GameManager.Instance != null)
                {
                    randomType = GameManager.Instance.GetModifiedElementType(randomType);
                }
                SetBlockImage(randomType); // 재귀 호출로 최종 타입 이미지 설정
                return; // 중요: 재귀 호출 후 현재 호출 종료
            default:
                nomalImage.sprite = null;
                break;
        }
    }

    // 블록을 사용하는 중일 때 확인하는 메서드
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Selected = true;
        hooverImage.gameObject.SetActive(true); // hooverImage 활성화
        
        ShapeSquare collidedSquare = collision.GetComponent<ShapeSquare>();

        if (collidedSquare != null)
        {
            currentCollidedBlock = collidedSquare.elementType; // 블록의 원소 타입 설정
            isColliding = true; // 블록 사용 중
            //Debug.Log("블록의 원소 타입: " + currentCollidedBlock);
        }
    }

    // 블록을 사용하는 중일 때 확인하는 메서드
    private void OnTriggerStay2D(Collider2D collision)
    {
        Selected = true;
        hooverImage.gameObject.SetActive(true);     // hooverImage 활성화
    }

    // 블록을 사용하지 않을 때 확인하는 메서드
    private void OnTriggerExit2D(Collider2D collision)
    {
        Selected = false;
        hooverImage.gameObject.SetActive(false);
        isColliding = false; // 블록 사용 중
    }
}
