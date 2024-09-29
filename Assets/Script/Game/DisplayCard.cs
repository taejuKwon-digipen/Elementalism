using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DisplayCard : MonoBehaviour
{
    CardDtatBase carddatabase;

    public List<Card> DisplayCardList = new List<Card>();
    public int displayID;

    public int id;
    public string description;
    public int attack;
    public bool usemagic;

    public Text NameText;
    public Text IDText;
    public Text DesctiptionText;
    public Text PowerLeftText;
    public Text PowerRightText;

    // Start is called before the first frame update
    void Start()
    {
        DisplayCardList = carddatabase.CardForPlay;
    }

    // Update is called once per frame
    void Update()
    {
 
    }
}
