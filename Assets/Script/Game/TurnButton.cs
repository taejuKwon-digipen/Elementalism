using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnButton : MonoBehaviour
{
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnTurnEndButtonClick);
        }
    }

    private void OnTurnEndButtonClick()
    {
        if (TurnManager.Inst != null)
        {
            TurnManager.Inst.OnTurnEndButtonPressed();
            if (GridChecker.inst != null)
            {
                GridChecker.inst.OnTurnEnd();
            }
        }
        else
        {
            Debug.LogError("[TurnButton] TurnManager를 찾을 수 없습니다!");
        }
    }
}