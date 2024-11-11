using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using System.Collections;

public class Card : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] TMP_Text nameTMP;
    [SerializeField] TMP_Text PowerLeftTMP;
    [SerializeField] TMP_Text PowerRightTMP;
    [SerializeField] TMP_Text CardDescriptionTMP;
    [SerializeField] TMP_Text IDTMP;

    public CardItem carditem;
    public bool isFront = true;

    public Vector3 currentMousePosition;

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

    }

    // IPointerUpHandler 인터페이스 구현 - 마우스 버튼을 놓을 때 호출
    public void OnPointerUp(PointerEventData eventData)
    {
        currentMousePosition = (Input.mousePosition);
        Debug.Log("카드 선택");
        CardManager.Inst.ViewWaitingCard(this); // CardManager에 카드가 선택됨을 알림
        //Destroy(gameObject); // 카드 즉시 삭제
    }

   
}
