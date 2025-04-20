using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnButton : MonoBehaviour
{
    private Button button;
    private bool interactionsEnabled = true;

    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnTurnEndButtonClick);
        }
    }

    public void SetInteractionsEnabled(bool enabled)
    {
        interactionsEnabled = enabled;
        if (button != null)
        {
            button.interactable = enabled;
        }
        Debug.Log($"[TurnButton] 상호작용 {(enabled ? "활성화" : "비활성화")} 완료");
    }

    private void OnTurnEndButtonClick()
    {
        if (!interactionsEnabled) return;

        if (TurnManager.Inst != null)
        {
            TurnManager.Inst.OnTurnEndButtonPressed();
            if (GridChecker.inst != null)
            {
                GridChecker.inst.OnTurnEnd();
            }

            if (TutorialManager.Instance != null && TutorialManager.Instance.IsCurrentStepCardLimit())
            {
                Debug.Log("[TurnButton] CardLimit 튜토리얼 단계 완료, 다음 단계로 진행");
                TutorialManager.Instance.NextStep();
            }
        }
        else
        {
            Debug.LogError("[TurnButton] TurnManager를 찾을 수 없습니다!");
        }
    }
}