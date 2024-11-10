using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using System.Collections;

public class Card : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] TMP_Text nameTMP;
    [SerializeField] TMP_Text PowerLeftTMP;
    [SerializeField] TMP_Text PowerRightTMP;
    [SerializeField] TMP_Text CardDescriptionTMP;
    [SerializeField] TMP_Text IDTMP;

    public CardItem carditem;
    public bool isFront = true;

    private bool isDragging = false;
    private const float deleteThresholdY = -.0f; // 카드가 삭제될 위치 기준 Y 좌표

    public void Setup(CardItem carditem_, bool isFront_)
    {
        carditem = carditem_;
        isFront = isFront_;

        if (isFront)
        {
            nameTMP.text = carditem.CardName;
            PowerLeftTMP.text = carditem.PowerLeft.ToString();
            PowerRightTMP.text = carditem.PowerRight.ToString();
            CardDescriptionTMP.text = carditem.CardDescription;
            IDTMP.text = carditem.ID.ToString();
        }
    }

    // IPointerDownHandler 인터페이스 구현 - 마우스 버튼을 누를 때 호출
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
    }

    // IPointerUpHandler 인터페이스 구현 - 마우스 버튼을 놓을 때 호출
    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;

        // 드래그가 끝난 후 카드가 삭제 기준 Y 위치보다 낮으면 삭제
        if (transform.position.y < deleteThresholdY)
        {
            CardManager.Inst.NotifyCardRemoved(this); // CardManager에 카드가 삭제되었음을 알림
            Destroy(gameObject); // 카드 즉시 삭제
        }
    }

    // IDragHandler 인터페이스 구현 - 드래그 중에 호출
    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
        }
    }
}
