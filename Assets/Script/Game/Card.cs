using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Card : MonoBehaviour
{
    //[SerializeField] SpriteRenderer card;
    //[SerializeField] SpriteRenderer character; //스프라이트는 나중에 넣는걸로 하구요
    [SerializeField] TMP_Text nameTMP;
    [SerializeField] TMP_Text PowerLeftTMP;
    [SerializeField] TMP_Text PowerRightTMP;
    [SerializeField] TMP_Text CardDescriptionTMP;
    [SerializeField] TMP_Text IDTMP;

    public CardItem carditem;
    public bool isFront = true;
  

    public void Setup(CardItem carditem_, bool isFront_)
    {
        carditem = carditem_;
        isFront = isFront_;

        if(isFront)
        {
            nameTMP.text = carditem.CardName;
            PowerLeftTMP.text = carditem.PowerLeft.ToString();
            PowerRightTMP.text = carditem.PowerRight.ToString();
            CardDescriptionTMP.text = carditem.CardDescription;
            IDTMP.text = carditem.ID.ToString();
        }
    

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
