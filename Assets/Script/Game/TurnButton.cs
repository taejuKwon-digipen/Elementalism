using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnButton : MonoBehaviour
{
    public EnemyManager manager;

    private Button button;

    private void Awake()
    {
        button = this.GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        button.interactable = !manager.isEnemyTurn;
    }
}