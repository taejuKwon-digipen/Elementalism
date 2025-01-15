
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening; // DoTween 사용

public class CardMouseHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    public float scaleMultiplier = 1.2f; // 확대 배율
    public float duration = 0.2f;       // 애니메이션 지속 시간

    // 초기 설정
    public void Initialize(float multiplier, float animDuration)
    {
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
}

