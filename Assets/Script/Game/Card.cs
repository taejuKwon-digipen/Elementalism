using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using System.Collections;

public class Card : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] TMP_Text nameTMP;
    [SerializeField] TMP_Text PowerLeftTMP;
    [SerializeField] TMP_Text PowerRightTMP;
    [SerializeField] TMP_Text CardDescriptionTMP;
    [SerializeField] TMP_Text IDTMP;

    public CardItem carditem;
    public bool isFront = true;

    public bool isInSelectionPanel = false; // 이 카드가 선택 패널 안에 있는지 여부

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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isInSelectionPanel)
        {
            // 선택 패널에서 클릭된 카드
            CardManager.Inst.OnCardSelected(this);
        }
        else
        {
            Debug.Log("카드가 선택되었음");
            // 게임 화면에서 클릭된 카드
            CardManager.Inst.OnCardClicked(this);
        }
    }


   /* // IPointerUpHandler 인터페이스 구현 - 마우스 버튼을 놓을 때 호출
    public void OnPointerUp(PointerEventData eventData)
    {
        currentMousePosition = (Input.mousePosition);
        Debug.Log("카드 선택");
        CardManager.Inst.ViewWaitingCard(this); // CardManager에 카드가 선택됨을 알림
        //Destroy(gameObject); // 카드 즉시 삭제
    }*/
   
}
