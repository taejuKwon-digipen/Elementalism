using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DisplayCard : MonoBehaviour
{
    CardDtatBase carddatabase;

    public List<Card> DisplayCardList = new List<Card>();
    public int displayID = 0;

    public string cardname;
    public int id;
    public string description;
    public int attack;
    public bool usemagic = false;

    public Text NameText;
    public Text IDText;
    public Text DesctiptionText;
    public Text PowerLeftText;
    public Text PowerRightText;

    // Start is called before the first frame update
    void Start()
    {
        DisplayCardList[displayID] = carddatabase.CardForPlay[displayID];
        cardname = carddatabase.CardForPlay[displayID].CardName;
        id = carddatabase.CardForPlay[displayID].ID;
        description = carddatabase.CardForPlay[displayID].CardDescription;
        attack = carddatabase.CardForPlay[displayID].Attack;
        

    }

    // Update is called once per frame
    void Update()
    {
        NameText.text = "" + cardname;
        IDText.text = "" + id;
        PowerRightText.text = "" + attack;

    }
}
