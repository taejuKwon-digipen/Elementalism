
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening; // DoTween ���

public class CardMouseHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private CardManager cardManager;

    private Vector3 originalScale;
    public float scaleMultiplier = 1.2f; // Ȯ�� ����
    public float duration = 0.2f;       // �ִϸ��̼� ���� �ð�

    // �ʱ� ����
    public void Initialize(float multiplier, float animDuration)
    {
        cardManager = FindObjectOfType<CardManager>(); // CardManager ��������

        scaleMultiplier = multiplier;
        duration = animDuration;
        originalScale = transform.localScale; // �ʱ� ũ�� ����
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


        // Ŭ���� ī�� ����
        Card selectedCard = GetClickedCard();
        //Ŭ���� ī�尡 �����ϴ��� Ȯ��, ī�� �Ŵ����� �����ϴ��� Ȯ��
        //-> ī�� Ŭ�� && ī��Ŵ��� ������ ���� �Ʒ� �ڵ� ����
        if (selectedCard != null && cardManager != null)
        {
            cardManager.SelectCardForSwitch(selectedCard);
        }
    }
    private Card GetClickedCard()
    {
        // ���콺 ��ġ�� ���� ��ǥ�� ��ȯ
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

