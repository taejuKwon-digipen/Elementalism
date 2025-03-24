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
            elementType = originalElementType;
            originalElementType = ElementType.None;
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

    public void SetElementType(ElementType newType)
    {
        elementType = newType;
    }

    // 원소 타입에 따른 이미지를 설정하는 메서드
    public void SetBlockImage(ElementType elementType)
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
                nomalImage.sprite = airSprite;      // 바람 스프라이트로 설정
                break;
            case ElementType.Earth:
                nomalImage.sprite = earthSprite;    // 땅 스프라이트로 설정
                break;
            case ElementType.Void:
                nomalImage.sprite = null;           // 공허는 이미지 없음
                break;
            case ElementType.Random:
                // Random인 경우 랜덤한 원소 타입 선택 (None과 Random 제외)
                ElementType randomType = (ElementType)Random.Range(1, (int)ElementType.Random);
                SetBlockImage(randomType);
                break;
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
