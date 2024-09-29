using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[System.Serializable]

public class CardDtatBase : MonoBehaviour
{
    public List<Card> CardForBlock = new List<Card>();
    public List<Card> CardForPlay = new List<Card>();

    void Awake()
    {
        CardForPlay.Add(new Card(0, 10, "Booldy hell1", false));
        CardForPlay.Add(new Card(1, 10, "Booldy hell2", false));

        CardForBlock.Add(new Card(0, 10, "Booldy hell3", true));
        CardForBlock.Add(new Card(1, 10, "Booldy hell4",true));

    }

}



