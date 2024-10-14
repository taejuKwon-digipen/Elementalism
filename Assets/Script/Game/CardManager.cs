using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Inst { get; private set; }
    void Awake() => Inst = this;

    [SerializeField] CardItemSO carditemso;

    List<CardItem> ItemBuffer;

    public CardItem PopCard()
    {
        if(ItemBuffer.Count == 0)
        {
            SetCardBuffer();
        }

        CardItem card = ItemBuffer[0];
        ItemBuffer.RemoveAt(0);
        return card;
    }

    void SetCardBuffer()
    {
        ItemBuffer = new List<CardItem>();

        for(int i = 0; i < carditemso.items.Length; i++)
        {
            CardItem cardditem = carditemso.items[i]; ;
            for (int j = 0; j < cardditem.Percent; j++)
            {
                ItemBuffer.Add(cardditem);
            }
        }

        for(int i = 0; i <ItemBuffer.Count; i++)
        {
            int rand = Random.Range(i, ItemBuffer.Count);
            CardItem temp = ItemBuffer[i];
            ItemBuffer[i] = ItemBuffer[rand];
            ItemBuffer[rand] = temp;
        }
    }
    public void Start()
    {
       /* if (ItemBuffer != null)
        {*/
            SetCardBuffer(); //�̰ű��� �� -> Ȥ���� �� ���ٰ� �������� �ɱ�?
       /* }
        else
        {
            string result = "fuckyou"; //�� ���ϱ� ī��Ŵ����� �� ���µ� ������
            print(result);
        }*/
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            print(PopCard().CardName);
        }
    }
}
