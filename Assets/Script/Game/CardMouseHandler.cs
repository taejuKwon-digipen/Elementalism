
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening; // DoTween ���

public class CardMouseHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    public float scaleMultiplier = 1.2f; // Ȯ�� ����
    public float duration = 0.2f;       // �ִϸ��̼� ���� �ð�

    // �ʱ� ����
    public void Initialize(float multiplier, float animDuration)
    {
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
}

