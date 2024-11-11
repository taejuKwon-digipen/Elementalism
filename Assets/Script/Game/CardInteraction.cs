using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardInteraction : MonoBehaviour { 
    public string CardName;

    public void OnPointClick()
    {
        Debug.Log("Card clicked" + CardName);

    }
}
