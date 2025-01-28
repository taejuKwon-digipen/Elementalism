
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening; // DoTween 사용

public class CardMouseHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private CardManager cardManager;

    private Vector3 originalScale;
    public float scaleMultiplier = 1.2f; // 확대 배율
    public float duration = 0.2f;       // 애니메이션 지속 시간

    // 초기 설정
    public void Initialize(float multiplier, float animDuration)
    {
        cardManager = FindObjectOfType<CardManager>(); // CardManager 가져오기

        scaleMultiplier = multiplier;
        duration = animDuration;
        originalScale = transform.localScale; // 초기 크기 저장
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"Mouse Enter: {gameObject.name}");
        transform.DOScale(originalScale * scaleMultiplier, duration).SetEase(Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log($"Mouse Exit: {gameObject.name}");
        transform.DOScale(originalScale, duration).SetEase(Ease.OutQuad);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Mouse Clicked on: {gameObject.name}");


        // 클릭한 카드 감지
        Card selectedCard = GetClickedCard();
        //클릭한 카드가 존재하는지 확인, 카드 매니저가 존재하는지 확인
        //-> 카드 클릭 && 카드매니저 존재할 때만 아래 코드 실행
        if (selectedCard != null && cardManager != null)
        {
            cardManager.SelectCardForSwitch(selectedCard);
        }
    }
    private Card GetClickedCard()
    {
        // 마우스 위치를 월드 좌표로 변환
        Vector3 mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null)
        {
            Card clickedCard = hit.collider.GetComponent<Card>();
            if (clickedCard != null)
            {
                Debug.Log($"Clicked Card: {clickedCard.name}");
                return clickedCard;
            }
        }
        return null;
    }

}

