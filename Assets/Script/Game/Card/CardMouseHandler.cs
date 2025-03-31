
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening; // DoTween 사용

public class CardMouseHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private CardManager cardManager;

    private Vector3 originalScale;
    public float scaleMultiplier = 1.2f; // 확대 배율
    public float duration = 0.2f;       // 애니메이션 지속 시간

    public void SetCardManager(CardManager manager)
    {
        cardManager = manager;
    }

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
    //------------------------여기까지 확대, 밑에는 클릭------------------------//

    private Card GetClickedCard() //마우스로 클릭된 카드를 찾아 리턴
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
                //클릭된 카드 리턴
                return clickedCard;
            }
        }
        ResetScale();
        return null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Mouse Clicked on: {gameObject.name}");

        Card selectedCard = GetClickedCard();

        if (selectedCard != null && cardManager != null)
        {
           cardManager.NotifyCardSelection(selectedCard);
        }
        
    }
    public void ResetScale()
    {
        transform.localScale = originalScale; // 크기를 원래대로 되돌림
    }

}

